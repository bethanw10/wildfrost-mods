using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using UnityEngine;

namespace TutorialMod
{
    public class InkcapMod : WildfrostMod
    {
        public InkcapMod(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.inkcap";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Inkcap Charm";

        public override string Description => "Adds a new charm that swaps ink effects with shroom and vice versa";

        private List<StatusEffectDataBuilder> statusEffects;
        private List<CardUpgradeDataBuilder> cardUpgrades;
        private bool preLoaded;

        private void CreateModAssets()
        {
            var inkOrShroomConstraint = ScriptableObject.CreateInstance<TargetConstraintOr>();

            var inkConstraint = ScriptableObject.CreateInstance<TargetConstraintHasEffectBasedOn>();
            inkConstraint.basedOnStatusType = "ink";

            var shroomConstraint = ScriptableObject.CreateInstance<TargetConstraintHasEffectBasedOn>();
            shroomConstraint.basedOnStatusType = "shroom";

            inkOrShroomConstraint.constraints = new TargetConstraint[] { inkConstraint, shroomConstraint };

            cardUpgrades = new List<CardUpgradeDataBuilder>
            {
                new CardUpgradeDataBuilder(this)
                    .Create("CardUpgradeInkcap")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("InkcapCharm.png")
                    .WithTitle("Inkcap Charm")
                    .WithText("Replace <keyword=null> effects with <keyword=shroom> and vice versa")
                    .WithTier(2)
                    .WithPools("ClunkCharmPool", "BasicCharmPool")
                    .SetConstraints(inkOrShroomConstraint)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        var script = ScriptableObject.CreateInstance<TutorialMod.CardScriptSwapEffectsBasedOn>();
                        script.statusA =  Get<StatusEffectData>("Null").InstantiateKeepName();
                        script.statusB =  Get<StatusEffectData>("Shroom").InstantiateKeepName();
                        script.Mod = this;
                        data.scripts = new CardScript[] { script };
                    })
            };

            statusEffects = new List<StatusEffectDataBuilder>
            {
                // Shroom Launcher
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenUnitIsKilled>("When Enemy (Nulled) Is Killed Apply Their Null To RandomEnemy")
                    .WithCanBeBoosted(false)
                    .WithText("When a <keyword=null>'d enemy dies, apply their <keyword=null> to a random enemy")
                    .WithType("")
                    .FreeModify(delegate(StatusEffectApplyXWhenUnitIsKilled data)
                    {
                        data.enemy = true;
                        data.ally = false;
                        var target = ScriptableObject.CreateInstance<TargetConstraintHasStatus>();
                        target.status = Get<StatusEffectData>("Null").InstantiateKeepName();
                        target.name = "Has Ink";
                        data.unitConstraints = new TargetConstraint[] { target };
                        data.applyEqualAmount = true;

                        var equalAmount = ScriptableObject.CreateInstance<ScriptableCurrentStatus>();
                        equalAmount.statusType = "ink";
                        data.contextEqualAmount = equalAmount;
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomEnemy;

                        data.effectToApply = Get<StatusEffectData>("Null").InstantiateKeepName();
                        data.noTargetType = NoTargetType.NoTargetForStatus;
                        data.noTargetTypeArgs = new[] { "<sprite name=ink>" };
                        data.queue = true;
                        data.applyConstraints.AddItem(ScriptableObject.CreateInstance<TargetConstraintOnBoard>());
                        data.applyConstraints.AddItem(ScriptableObject.CreateInstance<TargetConstraintIsAlive>());

                    }),

                // Shroomine
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenHit>("When Hit Apply Null To Attacker")
                    .WithCanBeBoosted(false)
                    .WithText("When hit, apply <{0}> <keyword=null> to the attacker")
                    .WithType("")
                    .FreeModify(delegate(StatusEffectApplyXWhenHit data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Attacker;
                
                        data.effectToApply = Get<StatusEffectData>("Null").InstantiateKeepName();

                        data.targetMustBeAlive = false;
                        data.canBeBoosted = true;

                        data.desc = "When hit, apply <{0}> <keyword=null> to the attacker";
                        data.textInsert = "{a}";

                        data.hiddenKeywords.AddItem(Get<KeywordData>("hit").InstantiateKeepName());
                    }),

                // Shroominator
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenYAppliedTo>("When Null Applied To Anything Double Amount And Lose Scrap")
                    .WithCanBeBoosted(false)
                    .WithText("Whenever anything is <keyword=null>'d double the amount and lose <{a}> <keyword=scrap>")
                    .WithType("")
                    .FreeModify(delegate(StatusEffectApplyXWhenYAppliedTo data)
                    {
                        data.multiplyAmount = 2f;
                        data.adjustAmount = true;

                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;

                        data.whenAppliedToFlags = (StatusEffectApplyX.ApplyToFlags)(-1);
                        data.whenAppliedTypes = new [] { "ink" };

                        data.effectToApply = Get<StatusEffectData>("Lose Scrap").InstantiateKeepName();

                        data.targetMustBeAlive = true;
                        data.queue = true;
                        data.canBeBoosted = true;
                        data.eventPriority = -1;
                    }),

                // Fulbert
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenYAppliedTo>("When Spice Or Shell Applied To Self Null To RandomEnemy")
                    .WithCanBeBoosted(false)
                    .WithText("Whenever <keyword=spice>'d or <keyword=shell>'d apply equal <keyword=null> to a random enemy")
                    .WithType("")
                    .FreeModify(delegate(StatusEffectApplyXWhenYAppliedTo data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomEnemy;
                        data.effectToApply = Get<StatusEffectData>("Null").InstantiateKeepName();
                        data.noTargetType = NoTargetType.NoTargetForStatus;
                        data.noTargetTypeArgs = new[] { "<sprite name=ink>" };
                        data.queue = true;
                        data.targetMustBeAlive = true;
                        data.eventPriority = -1;
                        data.whenAppliedToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        data.whenAppliedTypes = new [] { "spice", "shell" };
                        data.applyEqualAmount = true;
                    }),

                // Player
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXOnCardPlayed>("On Card Played Apply Null To Enemies")
                    .WithCanBeBoosted(false)
                    .WithText("Apply <{0}> <keyword=null> to all enemies")
                    .WithType("")
                    .FreeModify(delegate(StatusEffectApplyXOnCardPlayed data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Enemies;
                        data.effectToApply = Get<StatusEffectData>("Null").InstantiateKeepName();
                        data.noTargetType = NoTargetType.NoTargetForStatus;
                        data.noTargetTypeArgs = new[] { "<sprite name=ink>" };
                        data.targetMustBeAlive = true;
                        data.canBeBoosted = true;

                        data.desc = "Apply <{0}> <keyword=null> to all enemies";
                        data.textInsert = "{a}";
                    }),
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
                default:
                    return null;
            }
        }
    }
}