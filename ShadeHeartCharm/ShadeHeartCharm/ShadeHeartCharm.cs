using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;
using static CardData;

namespace ShadeHeartCharm
{
    public class ShadeHeartCharm : WildfrostMod
    {
        public ShadeHeartCharm(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.shadeheartcharm";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Shade Heart Charm";

        public override string Description => "Adds a new charm that gives summoned, +1 attack, -1 health and -1 counter";

        private List<CardUpgradeDataBuilder> cardUpgrades;
        private bool preLoaded;

        private void CreateModAssets()
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();

            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            cardUpgrades = new List<CardUpgradeDataBuilder>
            {
                new CardUpgradeDataBuilder(this)
                    .Create("CardUpgradeShadeHeart")
                    .WithPools("MagicCharmPool")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("ShadeHeartCharm.png")
                    .WithTitle("Shade Heart Charm")
                    .WithText("Gain <keyword=summoned>\n<-1><keyword=health>\n<+1><keyword=attack>\n<-1><keyword=counter>")
                    .SetConstraints(constraint, constraintUnit)
                    .WithTier(2)
                    .ChangeDamage(1)
                    .ChangeCounter(-1)
                    .ChangeHP(-1)
                    .SetEffects(new StatusEffectStacks(Get<StatusEffectData>("Summoned"), 1))
                    .SetTraits(new TraitStacks(Get<TraitData>("Summoned"), 1))
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
    }
}