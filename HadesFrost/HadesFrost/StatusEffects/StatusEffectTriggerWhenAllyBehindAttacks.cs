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
            if (target.enabled && Battle.IsOnBoard(target) && hit.countsAsHit && hit.Offensive && (bool)(Object)hit.target && CheckEntity(hit.attacker))
                prime.Add(hit.attacker);
            return false;
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (prime.Count > 0 && prime.Contains(entity) && targets != null && targets.Length > 0)
            {
                prime.Remove(entity);
                if (Battle.IsOnBoard(target) && CanTrigger())
                    Run(entity, targets);
            }
            return false;
        }

        public void Run(Entity attacker, Entity[] targets)
        {
            if (againstTarget)
            {
                foreach (Entity target in targets)
                    ActionQueue.Stack(new ActionTriggerAgainst(this.target, attacker, target, null), true);
            }
            else
                ActionQueue.Stack(new ActionTrigger(this.target, attacker), true);
        }

        public bool CheckEntity(Entity entity) => (bool)(Object)entity && entity.owner.team == target.owner.team && entity != target && CheckBehind(entity) && Battle.IsOnBoard(entity) && CheckDuplicate(entity) && CheckDuplicate(entity.triggeredBy);

        public bool CheckBehind(Entity entity)
        {
            foreach (var cardContainer in target.actualContainers.ToArray())
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
                   entity.statusEffects.All(statusEffect => statusEffect.name != name);
        }
    }
}
