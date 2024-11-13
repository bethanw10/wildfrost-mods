using System.Collections;
using UnityEngine;

namespace HadesFrost.StatusEffects
{
    public class StatusEffectJolted : StatusEffectData
    {
        public override void Init()
        {
            OnCardPlayed += Check;
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            return entity == target;
        }

        public IEnumerator Check(Entity entity, Entity[] targets)
        {
            var hit2 = new Hit(entity, entity, count)
            {
                canRetaliate = false,
                damageType = "jolt"
            };

            VFXHelper.VFX.TryPlayEffect("jolt", target.transform.position, 0.5f * target.transform.lossyScale);
            // HadesFrost.SFX.TryPlaySound("jolt");
            target.curveAnimator.Ping();
            yield return new WaitForSeconds(0.25f);
            yield return hit2.Process();
        }
    }
}