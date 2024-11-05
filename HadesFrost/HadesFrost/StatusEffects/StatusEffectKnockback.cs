// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnHit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HadesFrost.StatusEffects
{
    public class StatusEffectKnockback : StatusEffectApplyX
    {
        [SerializeField]
        public bool postHit;
        [Header("Modify Damage")]
        [SerializeField]
        public int addDamageFactor;
        [SerializeField]
        public float multiplyDamageFactor = 1f;
        public readonly List<Hit> storedHit = new List<Hit>();

        public Entity entityBehind;

        public override void Init()
        {
            if (this.postHit)
            {
                this.PostHit += this.CheckHit;
            }
            else
            {
                this.OnHit += this.CheckHit;
            }
        }

        public override bool RunPreAttackEvent(Hit hit)
        {
            if (hit.attacker == target && this.target.alive && this.target.enabled && (bool)hit.target)
            {
                if (this.addDamageFactor != 0 || multiplyDamageFactor != 1.0)
                {
                    var flag = true;
                    foreach (var applyConstraint in this.applyConstraints)
                    {
                        if (!applyConstraint.Check(hit.target) && (!(applyConstraint is TargetConstraintHasStatus constraintHasStatus) || !constraintHasStatus.CheckWillApply(hit)))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        var amount = this.GetAmount();
                        if (this.addDamageFactor != 0)
                        {
                            hit.damage += amount * this.addDamageFactor;
                        }

                        if (multiplyDamageFactor != 1.0)
                        {
                            hit.damage = Mathf.RoundToInt(hit.damage * this.multiplyDamageFactor);
                        }
                    }
                }
                if (!hit.Offensive && (hit.damage > 0 || (bool)effectToApply && this.effectToApply.offensive))
                {
                    hit.FlagAsOffensive();
                }

                this.storedHit.Add(hit);
            }
            return false;
        }

        public override bool RunPostHitEvent(Hit hit) => this.storedHit.Contains(hit) && hit.Offensive;

        public override bool RunHitEvent(Hit hit) => storedHit.Contains(hit) && hit.Offensive;

        public IEnumerator CheckHit(Hit hit)
        {
            var behind = GetEntityBehind(hit.target);

            if (behind == null)
            {
                yield break;
            }

            this.entityBehind = behind;

            var damage = hit.damage == 0 ? 0 : (int)Math.Ceiling(hit.damage/2.0);
            var hit2 = new Hit(hit.target, behind, damage)
            {
                canRetaliate = false
            };
            yield return new WaitForSeconds(0.25f);
            yield return hit2.Process();

            if ((bool)effectToApply)
            {
                yield return this.Run(this.GetTargets(hit), hit.damage + hit.damageBlocked);
            }

            this.storedHit.Remove(hit);
        }

        private static Entity GetEntityBehind(Entity entity)
        {
            foreach (var cardContainer in entity.actualContainers.ToArray())
            {
                if (!(cardContainer is CardSlot cardSlot) || !(cardContainer.Group is CardSlotLane group))
                {
                    continue;
                }

                var index = group.slots.IndexOf(cardSlot) + 1;

                if (index >= group.slots.Count)
                {
                    return null;
                }

                var rowEntity = group.slots[index].GetTop();

                if ((bool)rowEntity)
                {
                    return rowEntity;
                }
            }

            return null;
        }
    }
}
