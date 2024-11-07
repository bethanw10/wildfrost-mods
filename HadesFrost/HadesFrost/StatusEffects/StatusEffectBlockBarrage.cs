// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnKill
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HadesFrost.StatusEffects
{
    public class StatusEffectBlockBarrage : StatusEffectData
    {
        public override void Init() => OnHit += new EffectHitEventHandler(Check);

        public override bool RunHitEvent(Hit hit)
        {
            var allyIsBehind = IsEntityBehind(hit?.target);

            return allyIsBehind && 
                   hit.target?.owner == target?.owner &&
                   (hit.attacker?.traits?.Any(t => t.data.name == "Barrage") ?? false) && 
                   hit.Offensive && 
                   hit.canBeNullified;
        }

        private bool IsEntityBehind(Entity hitEntity)
        {
            foreach (var cardContainer in target.actualContainers.ToArray())
            {
                if (!(cardContainer is CardSlot cardSlot) || !(cardContainer.Group is CardSlotLane group))
                {
                    continue;
                }

                for (var index = group.slots.IndexOf(cardSlot) + 1; index < group.slots.Count; ++index)
                {
                    var entity = group.slots[index].GetTop();
                    if (!(bool)entity)
                    {
                        continue;
                    }

                    if ((bool)entity && entity == hitEntity)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public IEnumerator Check(Hit hit)
        {
            hit.nullified = true;
            hit.statusEffects = new List<CardData.StatusEffectStacks>();
            hit.damageBlocked = hit.damage;
            hit.damage = 0;
            hit.countsAsHit = false;
            yield break;
        }
    }
}
