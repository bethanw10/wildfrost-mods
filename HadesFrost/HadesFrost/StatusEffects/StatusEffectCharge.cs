// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnTurn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections;
using HadesFrost.Utils;
using UnityEngine;

namespace HadesFrost.StatusEffects
{
    public class StatusEffectCharge : StatusEffectApplyX
    {
        [SerializeField]
        private int EffectCount;

        [SerializeField]
        private bool DrawnTurn;
        
        /* If the bell is rung when not charged, we don't want that turn to count as a turn in hand.
         * If the bell is rung when charged, we do want the next turn to count.
         *  Set DrawnTurn to true when bell is rung, then don't increase attack until the next turn.
         *  Set DrawnTurn to false if another card is played (in case the bell was charged).
         **/

        public override void Init()
        {
            OnTurnStart += Activate;
            Events.OnRedrawBellHit += PreventEffectOnDrawTurn;
            OnCardPlayed += CardPlayed;
            Events.OnActionQueued += ActionQueued;
        }

        private void PreventEffectOnDrawTurn(RedrawBellSystem bellSystem)
        {
            DrawnTurn = true;
        }

        private void ActionQueued(PlayAction playAction)
        {
            // Discarded after playing
            if (playAction is ActionReduceUses actionReduce &&
                actionReduce.entity == target)
            {
                ActionQueue.Add(new ActionSequence(Deactivate()));
            }

            // Discarded by bell
            if (playAction is ActionMove actionMove &&
                actionMove.entity == target && 
                (bool)target.owner && actionMove.toContainers.Contains(target.owner.discardContainer))
            {
                
                ActionQueue.Add(new ActionSequence(Deactivate()));
            }
        }

        public override bool RunTurnStartEvent(Entity entity) => entity == target;

        private IEnumerator Activate(Entity entity)
        {
            if (DrawnTurn)
            {
                DrawnTurn = false;
            }
            else
            {
                EffectCount++;
                yield return Run(GetTargets());
            }
        }

        private IEnumerator CardPlayed(Entity entity, Entity[] targets)
        {
            if (entity != target)
            {
                DrawnTurn = false;
                yield break;
            }
        }

        private IEnumerator Deactivate()
        {
            var attackWhileDamaged = this;
            for (var index = attackWhileDamaged.target.statusEffects.Count - 1; index >= 0; --index)
            {
                var statusEffect = attackWhileDamaged.target.statusEffects[index];

                if ((bool)statusEffect && statusEffect.name == attackWhileDamaged.effectToApply.name)
                {
                    yield return statusEffect.RemoveStacks(statusEffect.count, true);
                    break;
                }
            }
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets) => true;

        public override bool RunEnableEvent(Entity entity) => entity == target && target.InHand();
    }
}
