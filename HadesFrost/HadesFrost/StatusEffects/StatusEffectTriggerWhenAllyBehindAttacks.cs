// Decompiled with JetBrains decompiler
// Type: StatusEffectTriggerWhenAllyAttacks
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HadesFrost.StatusEffects
{
    public class StatusEffectTriggerWhenAllyBehindAttacks : StatusEffectReaction
    {
        [SerializeField]
        public bool againstTarget;
        public readonly HashSet<Entity> prime = new HashSet<Entity>();

        public override bool RunHitEvent(Hit hit)
        {
            if (this.target.enabled && Battle.IsOnBoard(this.target) && hit.countsAsHit && hit.Offensive && (bool)(Object)hit.target && this.CheckEntity(hit.attacker))
                this.prime.Add(hit.attacker);
            return false;
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (this.prime.Count > 0 && this.prime.Contains(entity) && targets != null && targets.Length > 0)
            {
                this.prime.Remove(entity);
                if (Battle.IsOnBoard(this.target) && this.CanTrigger())
                    this.Run(entity, targets);
            }
            return false;
        }

        public void Run(Entity attacker, Entity[] targets)
        {
            if (this.againstTarget)
            {
                foreach (Entity target in targets)
                    ActionQueue.Stack(new ActionTriggerAgainst(this.target, attacker, target, null), true);
            }
            else
                ActionQueue.Stack(new ActionTrigger(this.target, attacker), true);
        }

        public bool CheckEntity(Entity entity) => (bool)(Object)entity && entity.owner.team == this.target.owner.team && entity != this.target && this.CheckBehind(entity) && Battle.IsOnBoard(entity) && this.CheckDuplicate(entity) && this.CheckDuplicate(entity.triggeredBy);

        public bool CheckBehind(Entity entity)
        {
            foreach (var cardContainer in this.target.actualContainers.ToArray())
            {
                if (!(cardContainer is CardSlot cardSlot) || !(cardContainer.Group is CardSlotLane group))
                {
                    continue;
                }

                for (var index = group.slots.IndexOf(cardSlot) + 1; index < group.slots.Count; ++index)
                {
                    var rowEntity = group.slots[index].GetTop();

                    if ((bool)rowEntity)
                    {
                        return entity == rowEntity;
                    }
                }
            }

            return false;
        }

        public bool CheckDuplicate(Entity entity)
        {
            return !entity.IsAliveAndExists() || 
                   entity.statusEffects.All(statusEffect => statusEffect.name != this.name);
        }
    }
}
