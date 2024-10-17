using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using FMOD;
using HarmonyLib;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ArrowCharm
{
    public class ArrowCharmMod : WildfrostMod
    {
        public ArrowCharmMod(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.arrowcharm";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Arrow Charm";

        public override string Description => "Adds a new charm that gives Longshot and +1 Attack";

        private List<CardUpgradeDataBuilder> cardUpgrades;
        private bool preLoaded;

        private void CreateModAssets()
        {
            var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();

            var constraintHitsAll = ScriptableObject.CreateInstance<TargetConstraintHasStatus>();
            constraintHitsAll.not = true;
            var hitAll = Get<StatusEffectData>("Hit All Enemies");

            Debug.Log(hitAll);

            constraintHitsAll.status = hitAll;

            var constraintSmackbackOnly = ScriptableObject.CreateInstance<TargetConstraintAnd>();
            constraintSmackbackOnly.not = true;

            var constraintSmackback = ScriptableObject.CreateInstance<TargetConstraintHasTrait>();
            constraintSmackback.trait = Get<TraitData>("Smackback");

            var constraintCounter = ScriptableObject.CreateInstance<TargetConstraintMaxCounterMoreThan>();
            constraintCounter.not = true;
            constraintCounter.moreThan = 0;

            constraintSmackbackOnly.constraints = new TargetConstraint[] { constraintSmackback, constraintCounter };

            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            var traits = new CardData.TraitStacks(Get<TraitData>("Longshot"), 1);

            cardUpgrades = new List<CardUpgradeDataBuilder>
            {
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeArrowCharm")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("ArrowCharm.png")
                    .WithTitle("Arrow Charm")
                    .WithText("Gain <keyword=longshot>\n<+1><keyword=attack>")
                    .WithTier(1)
                    .SetConstraints(constraintAttack, constraintUnit, constraintSmackbackOnly, constraintHitsAll)
                    .ChangeDamage(1)
                    .SetTraits(traits)
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