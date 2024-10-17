using System;
using System.Linq;

namespace AllCharms.Charms
{
    public class TargetConstraintStatAverageNotZero : TargetConstraint
    {
        public override bool Check(Entity target)
        {
            return Check(target.data);
        }

        public override bool Check(CardData target)
        {
            var health = target.hp;
            var damage = target.damage;
            var counter = target.counter;

            var scrap = target.startWithEffects.FirstOrDefault(s => s.data is StatusEffectScrap);


            if (scrap != null)
            {
                health = scrap.count;
            }

            var average = (int)Math.Round((health + damage + counter) / 3d);

            return average > 0;
        }
    }
}