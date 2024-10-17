using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using UnityEngine;
using static CardData;

namespace GrogCharm
{
    public class GrogCharm : WildfrostMod
    {
        public GrogCharm(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.grogcharm";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Grog Charm";

        public override string Description => "Adds a new charm that adds count down counter by 1 when hit, but adds 1 to max counter";

        private List<CardUpgradeDataBuilder> cardUpgrades;
        private bool preLoaded;

        private void CreateModAssets()
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintMaxCounterMoreThan>();

            cardUpgrades = new List<CardUpgradeDataBuilder>
            {
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeGrog")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("GrogCharm.png")
                    .WithTitle("Grog Charm")
                    .WithText("Increase <keyword=counter> by <1>\nWhen hit, count down <keyword=counter> by 1")
                    .SetConstraints(constraint)
                    .WithTier(2)
                    .ChangeCounter(1)
                    .SetEffects(new StatusEffectStacks(Get<StatusEffectData>("When Hit Reduce Counter To Self"), 1))
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