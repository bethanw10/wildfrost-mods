// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnTurn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections;
using UnityEngine;

namespace HadesFrost.StatusEffects
{
    [CreateAssetMenu(menuName = "Status Effects/Specific/Apply X On Turn", fileName = "Apply X On Turn")]
    public class StatusEffectCharge : StatusEffectApplyX
    {
        public bool DrawnTurn;
        
        /* If the bell is rung when not charged, we don't want that turn to count as a turn in hand. If the bell is rung when charged,
         * We do want the next turn to count.
         *  Set DrawnTurn to true when bell is rung, then don't increase attack until the next turn.
         *  Set DrawnTurn to false if another card is played (in case the bell was charged).
         **/

        public override void Init()
        {
            //this.OnEnable += new StatusEffectData.EffectEntityEventHandler(this.PreventEffectOnDrawTurn);
            //Events.OnCardDraw += new UnityAction<int>(PreventEffectOnDrawTurn);
            Events.OnRedrawBellHit += PreventEffectOnDrawTurn;
            OnTurnStart += Activate;
            OnCardPlayed += CardPlayedDeactivate;
            Events.OnActionQueued += ActionQueued;
        }

        private void PreventEffectOnDrawTurn(RedrawBellSystem bellSystem)
        {
            DrawnTurn = true;
        }

        public void ActionQueued(PlayAction playAction)
        {
            // Drawn
            if ((playAction is ActionReveal actionReveal) && (bool)(Object)target.owner && (Object)actionReveal.entity == (Object)target)
            {
                // ActionQueue.Add(new ActionSequence(this.PreventEffectOnDrawTurn(actionReveal.entity)));
            }

            // Discarded
            if (playAction is ActionMove actionMove && (Object)actionMove.entity == (Object)target && (bool)(Object)target.owner && actionMove.toContainers.Contains<CardContainer>(target.owner.discardContainer))
            {
                ActionQueue.Add(new ActionSequence(Deactivate(null, null)));
            }
        }

        public override bool RunTurnStartEvent(Entity entity) => (Object)entity == (Object)target;

        public IEnumerator Activate(Entity entity)
        {
            if (DrawnTurn)
            {
                // Debug.Log("drawn true, setting to false");
        
                DrawnTurn = false;
            }
            else
            {
                // Debug.Log("ACTIVATE");

                yield return Run(GetTargets());
            }
        }

        public IEnumerator CardPlayedDeactivate(Entity entity, Entity[] targets)
        {
            if (entity != target)
            {
                DrawnTurn = false;
                yield break;
            }

            yield return Deactivate(entity, targets);
        }

        public IEnumerator Deactivate(Entity entity, Entity[] targets)
        {
            // Debug.Log("deactivating?");
            var attackWhileDamaged = this;
            for (var index = attackWhileDamaged.target.statusEffects.Count - 1; index >= 0; --index)
            {
                var statusEffect = attackWhileDamaged.target.statusEffects[index];
               // Debug.Log(statusEffect.name);

                if ((bool)(Object)statusEffect && statusEffect.name == attackWhileDamaged.effectToApply.name)
                {
                    // Debug.Log("remove?");

                    yield return statusEffect.RemoveStacks(statusEffect.count, true);
                    break;
                }
            }
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets) => true;

        public override bool RunEnableEvent(Entity entity) => (Object)entity == (Object)target && target.InHand();

        public IEnumerator PreventEffectOnDrawTurn(Entity entity)
        {
            // Debug.Log("[hades] enabled");
            // Debug.Log("turns " + References.Battle?.turnCount);

            if (References.Battle?.turnCount != 0) // allow for initial draw
            {
                yield return DrawnTurn = true;
            }
        }
    }
}
