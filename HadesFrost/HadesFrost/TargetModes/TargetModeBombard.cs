using System.Collections.Generic;
using System.Linq;

namespace HadesFrost.TargetModes
{
    internal class TargetModeBombard : TargetMode
    {
        public override Entity[] GetPotentialTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            var hashSet = new HashSet<Entity>();

            var bombard = entity.statusEffects.FirstOrDefault(s => s is StatusEffectBombard);

            if (bombard != null && bombard is StatusEffectBombard castBombard)
            {
                var targets = castBombard.targetList.Select(t => t.entities).SelectMany(t => t).Where(t => t != null);
                hashSet.AddRange(targets);
            }
            else
            {
                var targetModeBasic = CreateInstance<TargetModeBasic>();
                return targetModeBasic.GetPotentialTargets(entity, target, targetContainer);
            }

            return hashSet.ToArray();
        }

        public override Entity[] GetSubsequentTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            return GetTargets(entity, target, targetContainer);
        }
    }
}