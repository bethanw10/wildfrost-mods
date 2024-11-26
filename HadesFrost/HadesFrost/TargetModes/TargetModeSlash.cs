// Decompiled with JetBrains decompiler
// Type: TargetModeBasic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HadesFrost.TargetModes
{
    public class TargetModeSlash : TargetMode
    {
        public override bool TargetRow => false;

        public override Entity[] GetPotentialTargets(
            Entity entity,
            Entity target,
            CardContainer targetContainer)
        {
            var entitySet = new HashSet<Entity>();

            var basic = new TargetModeBasic();

            var entities = basic.GetPotentialTargets(entity, target, targetContainer);
            entitySet.AddRange(entities);

            if (entitySet.Count <= 0)
            {
                return null;
            }

            var adjacent = GetAdjacent(entities.First());

            if (adjacent != null)
            {
                entities = basic.GetPotentialTargets(entity, adjacent, targetContainer);
                entitySet.AddRange(entities);
            }

            return entitySet.Count <= 0 ? null : entitySet.ToArray();
        }

        public override Entity[] GetSubsequentTargets(
            Entity entity,
            Entity target,
            CardContainer targetContainer)
        {
            return GetTargets(entity, target, targetContainer);
        }

        private static Entity GetAdjacent(Entity target)
        {
            foreach (var cardContainer in target.actualContainers.ToArray())
            {
                if (!(cardContainer is CardSlot cardSlot) || !(cardContainer.Group is CardSlotLane group))
                {
                    continue;
                }

                var slot = group.slots.IndexOf(cardSlot);

                foreach (var row in Battle.instance.GetRows(target.owner))
                {
                    var entity = row[slot];

                    Debug.Log(entity?.name);

                    if (entity != null && entity != target)
                    {
                        return entity;
                    }
                }
            }

            return null;
        }
    }
}
