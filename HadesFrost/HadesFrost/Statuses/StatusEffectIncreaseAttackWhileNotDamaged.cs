﻿// Decompiled with JetBrains decompiler
// Type: StatusEffectIncreaseAttackWhileDamaged
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Specific/Increase Attack While Damaged", fileName = "Increase Attack While Damaged")]
public class StatusEffectIncreaseAttackWhileNotDamaged : StatusEffectData
{
    [SerializeField]
    public StatusEffectData effectToGain;
    public int currentAmount;
    public bool active;
    public int state;
    public override bool RunEnableEvent(Entity entity) => entity == this.target;

    public override bool HasEnableRoutine => true;

    public override IEnumerator EnableRoutine(Entity entity) => this.Check();

    public override bool RunDisableEvent(Entity entity) => entity == this.target && this.currentAmount != 0;

    public override bool HasDisableRoutine => true;

    public override IEnumerator DisableRoutine(Entity entity) => this.Deactivate();

    public override bool RunPostHitEvent(Hit hit) => hit.target == this.target;

    public override bool HasPostHitRoutine => true;

    public override IEnumerator PostHitRoutine(Hit hit) => this.Check();

    public IEnumerator Check()
    {
        var attackWhileDamaged = this;
        if (!attackWhileDamaged.target.alive)
        {
            yield break;
        }

        if (!attackWhileDamaged.active)
        {
            if (attackWhileDamaged.target.hp.current >= attackWhileDamaged.target.hp.max)
            {
                yield return attackWhileDamaged.Activate();
            }
        }
        else if (attackWhileDamaged.target.hp.current < attackWhileDamaged.target.hp.max)
        {
            yield return attackWhileDamaged.Deactivate();
        }
    }

    public IEnumerator Activate()
    {
        var attackWhileDamaged = this;

        attackWhileDamaged.currentAmount = attackWhileDamaged.GetAmount();

        this.active = true;

        yield return StatusEffectSystem.Apply(
            attackWhileDamaged.target, 
            attackWhileDamaged.target, 
            attackWhileDamaged.effectToGain, 
            attackWhileDamaged.currentAmount, 
            true);
    }

    public IEnumerator Deactivate()
    {
        var attackWhileDamaged = this;
        for (var index = attackWhileDamaged.target.statusEffects.Count - 1; index >= 0; --index)
        {
            var statusEffect = attackWhileDamaged.target.statusEffects[index];
            if ((bool)(Object)statusEffect && statusEffect.name == attackWhileDamaged.effectToGain.name)
            {
                yield return statusEffect.RemoveStacks(attackWhileDamaged.currentAmount, true);
                break;
            }
        }
        attackWhileDamaged.currentAmount = 0;
        attackWhileDamaged.active = false;
    }

    public override bool RunEffectBonusChangedEvent()
    {
        if (this.target.enabled && this.active)
        {
            ActionQueue.Add(new ActionSequence(this.ReAffect()));
        }

        return false;
    }

    public IEnumerator ReAffect()
    {
        yield return this.Deactivate();
        yield return this.Activate();
    }
}
