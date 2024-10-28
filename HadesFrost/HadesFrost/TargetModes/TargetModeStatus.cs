using System.Collections.Generic;
using System.Linq;

namespace HadesFrost
{
    internal class TargetModeStatus : TargetMode
    {
        public string targetType;
        public bool missing = false;
        public bool failSafe = false;

        public override Entity[] GetPotentialTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            var hashSet = new HashSet<Entity>();
            hashSet.AddRange(from e in entity.GetAllEnemies()
                where (bool)e && e.enabled && e.alive && e.canBeHit && HasStatus(e)
                select e);

            if (hashSet.Count <= 0 && failSafe)
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

        private bool HasStatus(Entity entity)
        {
            var hasStatus = entity.statusEffects.Any(t => t.type == targetType);

            return missing 
                ? !hasStatus 
                : hasStatus;
        }

    }
}