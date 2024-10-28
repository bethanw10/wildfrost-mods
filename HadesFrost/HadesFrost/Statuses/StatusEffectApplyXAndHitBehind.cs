// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnHit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class StatusEffectApplyXAndHitBehind : StatusEffectApplyX
{
    [SerializeField]
    public bool postHit;
    [Header("Modify Damage")]
    [SerializeField]
    public int addDamageFactor;
    [SerializeField]
    public float multiplyDamageFactor = 1f;
    public readonly List<Hit> storedHit = new List<Hit>();

    public override void Init()
    {
        if (this.postHit)
            this.PostHit += new StatusEffectData.EffectHitEventHandler(this.CheckHit);
        else
            this.OnHit += new StatusEffectData.EffectHitEventHandler(this.CheckHit);
    }

    public override bool RunPreAttackEvent(Hit hit)
    {
        if ((Object)hit.attacker == (Object)this.target && this.target.alive && this.target.enabled && (bool)(Object)hit.target)
        {
            if (this.addDamageFactor != 0 || (double)this.multiplyDamageFactor != 1.0)
            {
                bool flag = true;
                foreach (TargetConstraint applyConstraint in this.applyConstraints)
                {
                    if (!applyConstraint.Check(hit.target) && (!(applyConstraint is TargetConstraintHasStatus constraintHasStatus) || !constraintHasStatus.CheckWillApply(hit)))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    int amount = this.GetAmount();
                    if (this.addDamageFactor != 0)
                        hit.damage += amount * this.addDamageFactor;
                    if ((double)this.multiplyDamageFactor != 1.0)
                        hit.damage = Mathf.RoundToInt((float)hit.damage * this.multiplyDamageFactor);
                }
            }
            if (!hit.Offensive && (hit.damage > 0 || (bool)(Object)this.effectToApply && this.effectToApply.offensive))
                hit.FlagAsOffensive();
            this.storedHit.Add(hit);
        }
        return false;
    }

    public override bool RunPostHitEvent(Hit hit) => this.storedHit.Contains(hit) && hit.Offensive;

    public override bool RunHitEvent(Hit hit) => storedHit.Contains(hit) && hit.Offensive;

    public IEnumerator CheckHit(Hit hit)
    {
        var entityBehind = GetEntityBehind(hit.target);

        if (entityBehind == null)
        {
            yield break;
        }

        var damage = hit.damage == 0 ? 0 : (int)Math.Ceiling(hit.damage/2.0);
        var hit2 = new Hit(hit.target, entityBehind, damage)
        {
            canRetaliate = false
        };

        if ((bool)(Object)this.effectToApply)
        {
            yield return (object)this.Run(this.GetTargets(hit), hit.damage + hit.damageBlocked);
        }

        yield return hit2.Process();

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
