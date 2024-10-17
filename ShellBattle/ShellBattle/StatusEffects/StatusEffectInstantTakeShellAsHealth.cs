// Decompiled with JetBrains decompiler
// Type: StatusEffectInstantTakeHealth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Modded\Wildfrost_Data\Managed\Assembly-CSharp-Publicized.dll

using System.Collections;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Instant/Take Health", fileName = "Take Health")]
public class StatusEffectInstantTakeShellAsHealth : StatusEffectInstant
{
    [SerializeField]
    public StatusEffectData IncreaseHealthEffect;

    public override IEnumerator Process()
    {
        var instantTakeShell = this;
        //var amount = instantTakeShell.GetAmount();
        var amount = 0;
        var shell = instantTakeShell.target.statusEffects.FirstOrDefault(s => s.type == "shell");
        if (shell != null)
        {
            amount = shell.count;
            yield return shell.Remove(); 
            //instantTakeShell.target.statusEffects.Remove(shell);
        }
        instantTakeShell.target.PromptUpdate();
        var hit = new Hit(instantTakeShell.target, instantTakeShell.applier, 0)
        {
            canRetaliate = false,
            countsAsHit = false
        };
        hit.AddStatusEffect(instantTakeShell.IncreaseHealthEffect, amount);
        yield return hit.Process();
        yield return base.Process();
    }
}