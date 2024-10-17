using System;
using System.Linq;

namespace AllCharms.Charms
{
    public class CardScriptSetDamageAndCounterToHealth : CardScript
    {
        public override void Run(CardData target)
        {
            target.hasAttack = true;

            var health = target.hp;
            var damage = target.damage;
            var counter = target.counter;

            var scrap = target.startWithEffects.FirstOrDefault(s => s.data is StatusEffectScrap);


            if (scrap != null)
            {
                health = scrap.count;
            }

            var average = (int)Math.Round((health + damage + counter) / 3d);

            target.damage = average;
            target.counter = average;
            target.counter = average;

            if (scrap != null)
            {
                scrap.count = average;
            }
            else
            {
                target.hp = average;
            }
        }
    }
}