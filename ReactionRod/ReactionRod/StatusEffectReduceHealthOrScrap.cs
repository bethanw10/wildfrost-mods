// Decompiled with JetBrains decompiler
// Type: StatusEffectInstantReduceMaxHealth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Modded\Wildfrost_Data\Managed\Assembly-CSharp-Publicized.dll

using System.Collections;
using System.Linq;

public class StatusEffectReduceHealthOrScrap : StatusEffectInstantReduceMaxHealth
{
    public override IEnumerator Process()
    {
        var scrap = target.statusEffects.FirstOrDefault(s => s is StatusEffectScrap);

        if (scrap != null)
        {
            scrap.count -= 1;
        }
        target.Update();

        return base.Process();
    }
}