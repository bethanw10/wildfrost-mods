// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXWhenAnyoneTakesDamage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections;
using HadesFrost.Utils;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Specific/Apply X When Anyone Takes Damage", fileName = "Apply X When Anyone Takes Damage")]
public class StatusEffectApplyXWhenEnemyTakesDamage : StatusEffectApplyX
{
    [SerializeField]
    public string TargetDamageType = "basic";

    public bool AnyType = false;

    public override void Init() => PostHit += CheckHit;

    public override bool RunPostHitEvent(Hit hit)
    {
        Common.Log(hit.target.name);
        Common.Log(target.enabled &&
                   target.alive &&
                   hit.target.owner != References.Player &&
                   hit.Offensive &&
                   (hit.damageType == TargetDamageType || AnyType) &&
                   Battle.IsOnBoard(target));
        Common.Log(this.GetAmount());

        return target.enabled &&
               target.alive &&
               hit.target.owner != References.Player &&
               hit.Offensive &&
               (hit.damageType == TargetDamageType || AnyType) &&
               Battle.IsOnBoard(target);
    }

    public IEnumerator CheckHit(Hit hit)
    {
        return Run(GetTargets(hit), hit.damage + hit.damageBlocked);
    }
}