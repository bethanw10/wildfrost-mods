// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnKill
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections;
using UnityEngine;

public class StatusEffectApplyXOnKillWithContext : StatusEffectApplyX
{
    public override void Init() => this.OnEntityDestroyed += this.Check;

    public override bool RunEntityDestroyedEvent(Entity entity, DeathType deathType)
    {
        return entity.lastHit != null && entity.lastHit.attacker == this.target;
    }

    public IEnumerator Check(Entity entity, DeathType deathType)
    {
        StatusEffectApplyXOnKillWithContext xwhenUnitIsKilled = this;
        if ((bool)xwhenUnitIsKilled.contextEqualAmount)
        {
            int amount = xwhenUnitIsKilled.contextEqualAmount.Get(entity);
            Debug.Log("[hades] amoutn" + amount);
            yield return xwhenUnitIsKilled.Run(xwhenUnitIsKilled.GetTargets(entity.lastHit), amount);
        }
        else
            yield return xwhenUnitIsKilled.Run(xwhenUnitIsKilled.GetTargets(entity.lastHit));
    }
}