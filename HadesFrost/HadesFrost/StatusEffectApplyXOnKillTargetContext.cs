using System.Collections;
using UnityEngine;

namespace Piri
{
    public class StatusEffectApplyXOnKillTargetContext : StatusEffectApplyX
    {
        public override void Init() => OnEntityDestroyed += Check;

        public override bool RunEntityDestroyedEvent(Entity entity, DeathType deathType) => 
            entity.lastHit != null && 
            entity.lastHit.attacker == target;

        public IEnumerator Check(Entity entity, DeathType deathType)
        {
            var amount = contextEqualAmount.Get(entity);
            yield return Run(GetTargets(targets: new[]
            {
                entity
            }), amount);
        }
    }
}