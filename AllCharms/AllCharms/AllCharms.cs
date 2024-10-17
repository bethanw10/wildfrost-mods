using System;
using System.Collections.Generic;
using System.Linq;
using AllCharms.Charms;
using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using UnityEngine;
using static CardData;
using Debug = UnityEngine.Debug;

namespace AllCharms
{
    /*
     *[h1]Adds a bunch of new charms[/h1]
       
       [b]Arrow Charm[/b]
       Gain Longshot and +1 Attack
       
       [b]Cherry Charm[/b]
       Gives +1 to all buffs and debuffs applied to a card
       
       [b]Grog Charm[/b]
       Count down counter on hit
       +1 to counter
       
       [b]Makoko Charm[/b]
       +1 attack each trigger
       Set attack to 0
       
       [b]Inkcap Charm[/b]
       Replace shroom effect with ink and vice versa
       
       [b]Shade Heart Charm[/b]
       Gain summoned
       +1 attack, -1 health, -1 counter
       
       [b]Scissors Charm[/b]
       Unequips all charms from a card
       Does not take up a slot
       
       [b]Scales Charm[/b]
       Average out all stats (attack/scrap, health, counter)
     */
    public class AllCharms : WildfrostMod
    {
        private static bool preLoaded;

        private List<StatusEffectDataBuilder> statusEffects = new List<StatusEffectDataBuilder>();
        private List<CardUpgradeDataBuilder> cardUpgrades = new List<CardUpgradeDataBuilder>();
        private List<KeywordDataBuilder> keywords = new List<KeywordDataBuilder>();

        public AllCharms(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.allcharms";

        public override string[] Depends => new string[] { };

        public override string Title => "Bethan's Charms";

        public override string Description => "Adds a bunch of new charms. " +
                                              "Subscribe to ConfigManager to enable/disable any of the charms (restart the game after making changes). " +
                                              "If the mod configs don't work, please delete config.cfg in this mod's folder and restart the game. ";

        [ConfigItem(true, "Changes will be made after a game restart", "Arrow Charm")]
        public bool EnableArrowCharm;

        [ConfigItem(true, "Changes will be made after a game restart", "Cherry Charm")]
        public bool EnableCherryCharm;

        [ConfigItem(true, "Changes will be made after a game restart", "Grog Charm")]
        public bool EnableGrogCharm;

        [ConfigItem(true, "Changes will be made after a game restart", "Inkcap Charm")]
        public bool EnableInkcapCharm;

        [ConfigItem(true, "Changes will be made after a game restart", "Makoko Charm")]
        public bool EnableMakokoCharm;

        [ConfigItem(true, "Changes will be made after a game restart", "Scales Charm")]
        public bool EnableScalesCharm;

        [ConfigItem(true, "Changes will be made after a game restart", "Shade Heart Charm")]
        public bool EnableShadeHeartCharm;

        [ConfigItem(true, "Changes will be made after a game restart", "Scissors Charm")]
        public bool EnableScissorsCharm;
        
        [ConfigItem(true, "Changes will be made after a game restart", "Bell Charm")]
        public bool EnableBellCharm;

        private void CreateModAssets()
        {
            if (EnableArrowCharm) { ArrowCharm(); }
            if (EnableCherryCharm) { CherryCharm(); }
            if (EnableGrogCharm) { GrogCharm(); }
            if (EnableInkcapCharm) { InkcapCharm(); }
            if (EnableMakokoCharm) { MakokoCharm(); }
            if (EnableScalesCharm) { ScalesCharm(); }
            if (EnableShadeHeartCharm) { ShadeHeartCharm(); }
            if (EnableScissorsCharm) { ScissorsCharm(); }
            if (EnableBellCharm) { BellCharm(); }
            preLoaded = true;
        }

        private void BellCharm()
        {
            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectBonusDamageEqualToX>("Add Bell Count To Attack")
                    .WithCanBeBoosted(false)
                    .WithTextForAllLanguages("Add redraw bell count to <keyword=attack>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectBonusDamageEqualToX data)
                    {
                        data.on = StatusEffectBonusDamageEqualToX.On.ScriptableAmount;
                        data.scriptableAmount = new ScriptableBellCount();
                    })
            );

            var constraint = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();
            cardUpgrades.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeBell")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("BellCharm.png")
                    .WithTitle("Bell Charm")
                    .WithTextForAllLanguages("Add redraw bell count to <keyword=attack>")
                    .SetConstraints(constraint)
                    .WithTier(2)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        data.effects = new[]
                        {
                            new StatusEffectStacks(Get<StatusEffectData>("Add Bell Count To Attack"), 1)
                        };
                    })
            );
        }

        private void ArrowCharm()
        {
            var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();

            var constraintHitsAll = ScriptableObject.CreateInstance<TargetConstraintHasStatus>();
            constraintHitsAll.not = true;
            var hitAll = Get<StatusEffectData>("Hit All Enemies");

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

            var traits = new TraitStacks(Get<TraitData>("Longshot"), 1);

            var upgrades = new List<CardUpgradeDataBuilder>
            {
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeArrowCharm")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("ArrowCharm.png")
                    .WithTitle("Arrow Charm")
                    .WithTextForAllLanguages("Gain <keyword=longshot>\n<+1><keyword=attack>")
                    .WithTier(1)
                    .SetConstraints(constraintAttack, constraintUnit, constraintSmackbackOnly, constraintHitsAll)
                    .ChangeDamage(1)
                    .SetTraits(traits)
            };

            StatusEffectInstantEatSomething test = new StatusEffectInstantEatSomething();
            test.eatEffect = Get<StatusEffectData>("Eat");

            cardUpgrades.AddRange(upgrades);
        }

        private void CherryCharm()
        {
            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            keywords.Add(
                new KeywordDataBuilder(this)
                    .Create("amplify")
                    .WithTitle("Amplify")
                    .WithTitleColour(new Color(1f, 0.79f, 0.34f))
                    .WithShowName(true)
                    .WithDescription("Adds <+1> to status effects applied to this card in battle|Includes buffs AND debuffs")
                    .WithNoteColour(new Color(0.65f, 0.65f, 0.65f))
                    .WithBodyColour(new Color(1f, 1f, 1f))
                    .WithCanStack(false)
            );

            cardUpgrades.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeCherry")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("CherryCharm.png")
                    .WithTitle("Cherry Charm")
                    .WithTextForAllLanguages($"Gain <keyword={Extensions.PrefixGUID("amplify", this)}>")
                    .SetConstraints(constraintUnit)
                    .WithTier(2)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        data.effects = new[]
                        {
                            new StatusEffectStacks(Get<StatusEffectData>("Add <+1> to all status effects"), 1)
                        };
                    })
            );

            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenYAppliedTo>("Add <+1> to all status effects")
                    .WithCanBeBoosted(false)
                    .WithTextForAllLanguages($"<keyword={Extensions.PrefixGUID("amplify", this)}>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXWhenYAppliedTo data)
                    {
                        data.adjustAmount = true;
                        data.addAmount = 1;
                        data.whenAppliedTypes = new[]
                        {
                            "snow", "frost", "weakness", "null", "demonize", "haze", "overload", "weakness", "vim",
                            "shroom", "shell", "spice", "block", "teeth", "multihit", "frenzy"
                        };
                        data.whenAppliedToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                    })
            );
        }

        private void GrogCharm()
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintMaxCounterMoreThan>();

            cardUpgrades.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeGrog")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("GrogCharm.png")
                    .WithTitle("Grog Charm")
                    .WithTextForAllLanguages("Increase <keyword=counter> by <1>\nWhen hit, count down <keyword=counter> by 1")
                    .SetConstraints(constraint)
                    .WithTier(2)
                    .ChangeCounter(1)
                    .SetEffects(new StatusEffectStacks(Get<StatusEffectData>("When Hit Reduce Counter To Self"), 1))
            );
        }

        private void InkcapCharm()
        {
            var inkOrShroomConstraint = ScriptableObject.CreateInstance<TargetConstraintOr>();

            var inkConstraint = ScriptableObject.CreateInstance<TargetConstraintHasEffectBasedOn>();
            inkConstraint.basedOnStatusType = "ink";

            var shroomConstraint = ScriptableObject.CreateInstance<TargetConstraintHasEffectBasedOn>();
            shroomConstraint.basedOnStatusType = "shroom";

            inkOrShroomConstraint.constraints = new TargetConstraint[] { inkConstraint, shroomConstraint };

            cardUpgrades.Add(
                new CardUpgradeDataBuilder(this)
                    .Create("CardUpgradeInkcap")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("InkcapCharm.png")
                    .WithTitle("Inkcap Charm")
                    .WithTextForAllLanguages("Replace <keyword=null> effects with <keyword=shroom> and vice versa")
                    .WithTier(2)
                    .WithPools("ClunkCharmPool", "BasicCharmPool")
                    .SetConstraints(inkOrShroomConstraint)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        var script = ScriptableObject.CreateInstance<Charms.CardScriptSwapEffectsBasedOn>();
                        script.statusA =  Get<StatusEffectData>("Null").InstantiateKeepName();
                        script.statusB =  Get<StatusEffectData>("Shroom").InstantiateKeepName();
                        script.Mod = this;
                        data.scripts = new CardScript[] { script };
                    })
            );

            statusEffects.AddRange(new List<StatusEffectDataBuilder>
            {
                // Shroom Launcher
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenUnitIsKilled>("When Enemy (Nulled) Is Killed Apply Their Null To RandomEnemy")
                    .WithCanBeBoosted(false)
                    .WithTextForAllLanguages("When a <keyword=null>'d enemy dies, apply their <keyword=null> to a random enemy")
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
                    .WithTextForAllLanguages("When hit, apply <{0}> <keyword=null> to the attacker")
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
                    .WithTextForAllLanguages("Whenever anything is <keyword=null>'d double the amount and lose <{a}> <keyword=scrap>")
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
                    .WithTextForAllLanguages("Whenever <keyword=spice>'d or <keyword=shell>'d apply equal <keyword=null> to a random enemy")
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
                    .WithTextForAllLanguages("Apply <{0}> <keyword=null> to all enemies")
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
            });
        }

        private void MakokoCharm()
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();

            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            cardUpgrades.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeMakoko")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("MakokoCharm.png")
                    .WithTitle("Makoko Charm")
                    .WithTextForAllLanguages("Gain +<1><keyword=attack> on attack\nSet<keyword=attack>to <0>")
                    .SetConstraints(constraint, constraintUnit)
                    .WithTier(2)
                    .ChangeDamage(0)
                    .WithSetDamage(true)
                    .SetEffects(new StatusEffectStacks(Get<StatusEffectData>("On Turn Apply Attack To Self"), 1))
            );
        }

        private void ScalesCharm()
        {
            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();
            var constraintStats = ScriptableObject.CreateInstance<TargetConstraintStatAverageNotZero>();

            cardUpgrades.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeScales")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("ScalesCharm.png")
                    .WithTitle("Scales Charm")
                    .WithTextForAllLanguages("Set <keyword=attack>, <keyword=counter> and <keyword=health>/<keyword=scrap> to their average")
                    .SetConstraints(constraintUnit, constraintStats)
                    .WithTier(1)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        var script = ScriptableObject.CreateInstance<CardScriptSetDamageAndCounterToHealth>();
                        data.scripts = new CardScript[] { script };
                    })
            );
        }

        private void ShadeHeartCharm()
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();

            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            cardUpgrades.Add(
                new CardUpgradeDataBuilder(this)
                    .Create("CardUpgradeShadeHeart")
                    .WithPools("MagicCharmPool")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("ShadeHeartCharm.png")
                    .WithTitle("Shade Heart Charm")
                    .WithTextForAllLanguages("Gain <keyword=summoned>\n<-1><keyword=health>\n<+1><keyword=attack>\n<-1><keyword=counter>")
                    .SetConstraints(constraint, constraintUnit)
                    .WithTier(2)
                    .ChangeDamage(1)
                    .ChangeCounter(-1)
                    .ChangeHP(-1)
                    .SetEffects(new StatusEffectStacks(Get<StatusEffectData>("Summoned"), 1))
                    .SetTraits(new TraitStacks(Get<TraitData>("Summoned"), 1))
            );
        }

        private void ScissorsCharm()
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintIsInDeck>();

            cardUpgrades.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeScissors")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("ScissorsCharm.png")
                    .WithTitle("Scissors Charm")
                    .WithTextForAllLanguages("Unequips all charms, except other Scissors Charms\nDoes not take up a charm slot")
                    .WithTier(2)
                    .SetConstraints(constraint)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        var script = ScriptableObject.CreateInstance<CardScriptRemoveCharms>();
                        data.takeSlot = false;
                        data.scripts = new CardScript[] { script };
                    })
            );
        }

        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();
            RemoveFromPools();
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

        private void RemoveFromPools()
        {
            try
            {
                Debug.Log("[bw] Running remove from pools");
                string[] poolsToCheck = { "MagicCharmPool", "ClunkCharmPool", "GeneralCharmPool", "SnowCharmPool", "BasicCharmPool" };
                foreach (var pool in poolsToCheck.Select(Extensions.GetRewardPool).Where(pool => pool != null))
                {
                    Debug.Log("[bw] Extensions.GetRewardPool\t" + pool.list.Count(l => l == null || l.ModAdded == this) + " matches");
                    pool.list.RemoveAllWhere(l => l == null || l.ModAdded == this);
                }

                Debug.Log("[bw] Done");
            }
            catch (Exception e)
            {
                Debug.LogError("[bethan] error removing charms " + e.Message + "\n" + e.StackTrace);
            }
        }
    }
}