// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnHit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HadesFrost.StatusEffects
{
    public class StatusEffectApplyXIfStatsAreLower : StatusEffectApplyX
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
            if (postHit)
                PostHit += new EffectHitEventHandler(CheckHit);
            else
                OnHit += new EffectHitEventHandler(CheckHit);
        }

        public override bool RunPreAttackEvent(Hit hit)
        {
            if ((Object)hit.attacker == (Object)target && target.alive && target.enabled && (bool)(Object)hit.target)
            {
                if (addDamageFactor != 0 || (double)multiplyDamageFactor != 1.0)
                {
                    bool flag = true;
                    foreach (TargetConstraint applyConstraint in applyConstraints)
                    {
                        if (!applyConstraint.Check(hit.target) && (!(applyConstraint is TargetConstraintHasStatus constraintHasStatus) || !constraintHasStatus.CheckWillApply(hit)))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        int amount = GetAmount();
                        if (addDamageFactor != 0)
                            hit.damage += amount * addDamageFactor;
                        if ((double)multiplyDamageFactor != 1.0)
                            hit.damage = Mathf.RoundToInt((float)hit.damage * multiplyDamageFactor);
                    }
                }
                if (!hit.Offensive && (hit.damage > 0 || (bool)(Object)effectToApply && effectToApply.offensive))
                    hit.FlagAsOffensive();
                storedHit.Add(hit);
            }
            return false;
        }

        public override bool RunPostHitEvent(Hit hit) => storedHit.Contains(hit) && hit.Offensive;

        public override bool RunHitEvent(Hit hit) => storedHit.Contains(hit) && hit.Offensive;

        public IEnumerator CheckHit(Hit hit)
        {
            if ((bool)(Object)effectToApply)
            {
                // // var effectCount = this.effectToApply.count;
                // //
                // hit.target.hp.current + this.count <= hit.attacker.hp.current &&
                if ((hit.target.damage.current + hit.target.tempDamage + count) <= hit.attacker.damage.current + hit.attacker.tempDamage)
                {
                    yield return (object)Run(GetTargets(hit), hit.damage + hit.damageBlocked);
                }
            }
        }
    }
}
