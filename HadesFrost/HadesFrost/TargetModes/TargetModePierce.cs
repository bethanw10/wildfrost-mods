// Decompiled with JetBrains decompiler
// Type: TargetModeBasic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections.Generic;
using System.Linq;
using HadesFrost.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetModeBasic", menuName = "Target Modes/Basic")]
public class TargetModePierce : TargetMode
{
    public override Entity[] GetPotentialTargets(
      Entity entity,
      Entity target,
      CardContainer targetContainer)
    {
        var entitySet = new HashSet<Entity>();

        var entities = new TargetModeBasic().GetPotentialTargets(entity, target, targetContainer);
        entitySet.AddRange(entities);

        if (entitySet.Count <= 0)
        {
            return null;
        }

        var behind = GetBehind(entities.First());

        if (behind != null)
        {
            entities = new TargetModeBasic().GetPotentialTargets(entity, behind, targetContainer);
            entitySet.AddRange(entities);
        }

        foreach (var entity1 in entitySet)
        {
            Common.Log(entity1.name);
        }

        return entitySet.Count <= 0 ? null : entitySet.ToArray();
    }

    public override Entity[] GetSubsequentTargets(
        Entity entity,
        Entity target,
        CardContainer targetContainer)
    {
        return this.GetTargets(entity, target, targetContainer);
    }

    private static Entity GetBehind(Entity target)
    {
        foreach (var cardContainer in target.actualContainers.ToArray())
        {
            if (!(cardContainer is CardSlot cardSlot) || !(cardContainer.Group is CardSlotLane group))
            {
                continue;
            }

            var rowEntity = group.slots[group.slots.IndexOf(cardSlot) + 1].GetTop();

            if ((bool)rowEntity)
            {
                return rowEntity;
            }
        }

        return null;
    }
}
