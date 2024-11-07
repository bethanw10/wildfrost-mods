// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnHit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections;
using System.Collections.Generic;
using HadesFrost.Utils;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Specific/Apply X On Hit", fileName = "Apply X On Hit")]
public class StatusEffectApplyXOnTeamHit : StatusEffectApplyX
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
        {
            PostHit += CheckHit;
        }
        else
        {
            OnHit += CheckHit;
        }
    }

    public override bool RunPreAttackEvent(Hit hit)
    {
        Common.Log("Melinoe");
        if (hit.target?.owner != References.Player &&
            target.alive && 
            target.enabled && (bool)hit.target)
        {
        Common.Log("Melinoe");

            if (addDamageFactor != 0 || multiplyDamageFactor != 1.0)
            {
                var flag = true;
                foreach (var applyConstraint in applyConstraints)
                {
                    if (!applyConstraint.Check(hit.target) && (!(applyConstraint is TargetConstraintHasStatus constraintHasStatus) || !constraintHasStatus.CheckWillApply(hit)))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    var amount = GetAmount();
                    if (addDamageFactor != 0)
                    {
                        hit.damage += amount * addDamageFactor;
                    }

                    if (multiplyDamageFactor != 1.0)
                    {
                        hit.damage = Mathf.RoundToInt(hit.damage * multiplyDamageFactor);
                    }
                }
            }
            if (!hit.Offensive && (hit.damage > 0 || (bool)(Object)effectToApply && effectToApply.offensive))
            {
                hit.FlagAsOffensive();
            }

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
            yield return Run(GetTargets(hit), hit.damage + hit.damageBlocked);
        }

        storedHit.Remove(hit);
    }
}
