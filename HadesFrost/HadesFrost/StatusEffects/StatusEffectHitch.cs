using System.Collections;
using System.Linq;
using UnityEngine;

namespace HadesFrost.StatusEffects
{
    public class StatusEffectHitch : StatusEffectData
    {
        public override void Init() => this.OnHit += new StatusEffectData.EffectHitEventHandler(this.Hit);

        private IEnumerator Hit(Hit hit)
        {
            var cardsOnBoard = Battle.GetCardsOnBoard(this.target.owner);
            cardsOnBoard.Remove(this.target);

            foreach (var entity in cardsOnBoard)
            {
                var hasHitch = entity.statusEffects.Any(s => s.type == "hitch");

                if (!hasHitch)
                {
                    continue;
                }

                var hit2 = new Hit(hit.target, entity, count)
                {
                    canRetaliate = false,
                    countsAsHit = false,
                    damageType = "hitch"
                };

                target.curveAnimator.Ping();
                yield return hit2.Process();
            }

            // Pokefrost.VFX.TryPlayEffect("jolt", target.transform.position, 0.5f * target.transform.lossyScale);
            // Pokefrost.SFX.TryPlaySound("jolt");
        }

        public override bool RunHitEvent(global::Hit hit) => hit.Offensive && this.count > 0 && (Object)hit.target == (Object)this.target;
    }
}