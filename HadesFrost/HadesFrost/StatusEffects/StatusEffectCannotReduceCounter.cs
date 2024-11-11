// Decompiled with JetBrains decompiler
// Type: StatusEffectCannotIncreaseMaxHealth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Traits/Cannot Increase Max Health", fileName = "Cannot Increase Max Health")]
public class StatusEffectCannotReduceCounter : StatusEffectData
{
    public override bool RunApplyStatusEvent(StatusEffectApply apply)
    {
        if (apply.target == target && !target.silenced && CheckEffectType(apply.effectData))
        {
            apply.count = 0;
        }

        return false;
    }

    private static bool CheckEffectType(StatusEffectData effectData)
    {
        if (!(bool)effectData)
        {
            return false;
        }

        return effectData.type == "counter down";
    }
}