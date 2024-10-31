// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnTurn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Status Effects/Specific/Apply X On Turn", fileName = "Apply X On Turn")]
public class StatusEffectCharge : StatusEffectApplyX
{
    public bool DrawnTurn;

    public override void Init()
    {
        //this.OnEnable += new StatusEffectData.EffectEntityEventHandler(this.PreventEffectOnDrawTurn);
        //Events.OnCardDraw += new UnityAction<int>(PreventEffectOnDrawTurn);
        this.OnTurnStart += this.Activate;
        this.OnCardPlayed += this.Deactivate;
        Events.OnActionQueued += new UnityAction<PlayAction>(this.ActionQueued);
    }

    public void ActionQueued(PlayAction playAction)
    {
        if ((playAction is ActionReveal actionReveal) && (bool)(Object)this.target.owner && (Object)actionReveal.entity == (Object)this.target)
        {
            ActionQueue.Add(new ActionSequence(this.PreventEffectOnDrawTurn(actionReveal.entity)));
        }

        if (playAction is ActionMove actionMove && (Object)actionMove.entity == (Object)this.target && (bool)(Object)this.target.owner && actionMove.toContainers.Contains<CardContainer>(this.target.owner.discardContainer))
        {
            ActionQueue.Add(new ActionSequence(this.Deactivate(null, null)));
        }
    }

    public override bool RunTurnStartEvent(Entity entity) => 
       (Object)entity == (Object)this.target;


    public IEnumerator Activate(Entity entity)
    {
        if (this.DrawnTurn)
        {
            // Debug.Log("drawn true, setting to false");
        
            this.DrawnTurn = false;
        }
        else
        {
            // Debug.Log("ACTIVATE");

            yield return this.Run(this.GetTargets());
        }
    }

    public IEnumerator Deactivate(Entity entity, Entity[] targets)
    {
        // Debug.Log("deactivating?");
        var attackWhileDamaged = this;
        for (var index = attackWhileDamaged.target.statusEffects.Count - 1; index >= 0; --index)
        {
            var statusEffect = attackWhileDamaged.target.statusEffects[index];
            Debug.Log(statusEffect.name);

            if ((bool)(Object)statusEffect && statusEffect.name == attackWhileDamaged.effectToApply.name)
            {
                // Debug.Log("remove?");

                yield return statusEffect.RemoveStacks(statusEffect.count, true);
                break;
            }
        }
    }

    public override bool RunCardPlayedEvent(Entity entity, Entity[] targets) => (Object)entity == (Object)this.target;

    public override bool RunEnableEvent(Entity entity) => (Object)entity == (Object)this.target && this.target.InHand();

    public IEnumerator PreventEffectOnDrawTurn(Entity entity)
    {
        // Debug.Log("[hades] enabled");
        // Debug.Log("turns " + References.Battle?.turnCount);

        if (References.Battle?.turnCount != 0) // allow for initial draw
        {
            yield return this.DrawnTurn = true;
        }
    }
}
