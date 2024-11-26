using System.Collections;
using System.Linq;

namespace HadesFrost.StatusEffects
{
    public class StatusEffectHitch : StatusEffectData
    {
        public override void Init() => OnHit += Hit;

        private IEnumerator Hit(Hit hit)
        {
            var cardsOnBoard = Battle.GetCardsOnBoard(target.owner);
            cardsOnBoard.Remove(target);

            if (hit.damageType == "hitch")
            {
                yield break;
            }

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
                    damageType = "hitch"
                };

                target.curveAnimator.Ping();
                yield return hit2.Process();
            }

            // Pokefrost.VFX.TryPlayEffect("jolt", target.transform.position, 0.5f * target.transform.lossyScale);
            // Pokefrost.SFX.TryPlaySound("jolt");
        }

        public override bool RunHitEvent(Hit hit) => hit.Offensive && count > 0 && hit.target == target;
    }
}