using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;

namespace ArrowCharm
{
    public class CherryCharmMod : WildfrostMod
    {
        public CherryCharmMod(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.cherrycharm";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Cherry Charm";

        public override string Description => "Adds a new charm that gives +1 to any status effects applied to a card";

        private List<CardUpgradeDataBuilder> cardUpgrades;
        private List<StatusEffectDataBuilder> statusEffects;
        private List<KeywordDataBuilder> keywords;
        private bool preLoaded;

        private void CreateModAssets()
        {
            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            keywords = new List<KeywordDataBuilder>
            {
                new KeywordDataBuilder(this)
                    .Create("amplify")
                    .WithTitle("Amplify")
                    .WithTitleColour(new Color(1f, 0.79f, 0.34f))
                    .WithShowName(true)
                    .WithDescription("Adds <+1> to status effects applied to this card in battle|Includes buffs AND debuffs") 
                    .WithNoteColour(new Color(0.65f, 0.65f, 0.65f)) 
                    .WithBodyColour(new Color(1f, 1f, 1f))
                    .WithCanStack(false)
            };

            cardUpgrades = new List<CardUpgradeDataBuilder>
            {
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeCherry")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("CherryCharm.png")
                    .WithTitle("Cherry Charm")
                    .WithText($"Gain <keyword={Extensions.PrefixGUID("amplify", this)}>")
                    .SetConstraints(constraintUnit)
                    .WithTier(2)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        data.effects = new[]
                        {
                            new CardData.StatusEffectStacks(Get<StatusEffectData>("Add <+1> to all status effects"), 1)
                        };
                    })
            };

            statusEffects = new List<StatusEffectDataBuilder>
            {
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenYAppliedTo>("Add <+1> to all status effects")
                    .WithCanBeBoosted(false)
                    .WithText($"<keyword={Extensions.PrefixGUID("amplify", this)}>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXWhenYAppliedTo data)
                    {
                        data.adjustAmount = true;
                        data.addAmount = 1;
                        data.whenAppliedTypes = new[] 
                        { 
                            "snow", "frost", "weakness", "null", "demonize", "haze", "overload",
                            "shroom", "shell", "spice", "block", "teeth", "multihit", "frenzy"
                        };
                        data.whenAppliedToFlags = StatusEffectApplyX.ApplyToFlags.Self;
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
                case nameof(StatusEffectData):
                    return statusEffects.Cast<T>().ToList();
                case nameof(KeywordData):
                    return keywords.Cast<T>().ToList();
                default:
                    return null;
            }
        }
    }
}