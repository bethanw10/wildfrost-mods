using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;

namespace ScalesCharm
{
    public class ScalesCharm : WildfrostMod
    {
        public ScalesCharm(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.scalescharm";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Scales Charm";

        public override string Description => "Adds a new charm that averages attack, counter and health (or scrap)";

        private List<CardUpgradeDataBuilder> cardUpgrades;
        private bool preLoaded;

        private void CreateModAssets()
        {
            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();
            var constraintStats = ScriptableObject.CreateInstance<TargetConstraintStatAverageNotZero>();

            cardUpgrades = new List<CardUpgradeDataBuilder>
            {
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeScales")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("ScalesCharm.png")
                    .WithTitle("Scales Charm")
                    .WithText("Set <keyword=attack>, <keyword=counter> and <keyword=health>/<keyword=scrap> to their average")
                    .SetConstraints(constraintUnit, constraintStats)
                    .WithTier(2)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        var script = ScriptableObject.CreateInstance<CardScriptSetDamageAndCounterToHealth>();
                        data.scripts = new CardScript[] { script };
                    })
            };

            preLoaded = true;
        }

        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }
            base.Load();
        }

        public override List<T> AddAssets<T, TY>()
        {
            var typeName = typeof(TY).Name;
            switch (typeName)
            {
                case nameof(CardUpgradeData):
                    return cardUpgrades.Cast<T>().ToList();
                default:
                    return null;
            }
        }

        private class CardScriptSetDamageAndCounterToHealth : CardScript
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

        private class TargetConstraintStatAverageNotZero : TargetConstraint
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
}