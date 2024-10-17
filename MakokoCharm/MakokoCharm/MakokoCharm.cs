using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;
using static CardData;

namespace MakokoCharm
{
    public class MakokoCharm : WildfrostMod
    {
        public MakokoCharm(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.makokocharm";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Makoko Charm";

        public override string Description => "Adds a new charm that gives +1 attack each turn, but sets attack to 0";

        private List<CardUpgradeDataBuilder> cardUpgrades;
        private bool preLoaded;

        private void CreateModAssets()
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();

            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            cardUpgrades = new List<CardUpgradeDataBuilder>
            {
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeMakoko")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("MakokoCharm.png")
                    .WithTitle("Makoko Charm")
                    .WithText("Increase <keyword=attack>by <1> each turn\nSet<keyword=attack>to <0>")
                    .SetConstraints(constraint, constraintUnit)
                    .WithTier(2)
                    .ChangeDamage(0)
                    .WithSetDamage(true)
                    .SetEffects(new StatusEffectStacks(Get<StatusEffectData>("On Turn Apply Attack To Self"), 1))
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