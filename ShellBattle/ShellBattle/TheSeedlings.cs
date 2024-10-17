using BattleEditor;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;

namespace ShellBattle
{
    public partial class MoreFights
    {
        private void AddShellBattle()
        {
            new BattleDataEditor(this, "Spare Shells")
                .SetSprite(ImagePath("Seedlings.png").ToSprite())
                .SetNameRef("The Seedlings")
                .EnemyDictionary(
                    ('S', "Squirrel"),
                    ('E', "Pecan"),
                    ('L', "ShellWitchling"),
                    ('C', "Cashew"),
                    ('W', "ShellWitch"),
                    ('P', "Sporkypine"),
                    ('B', "Beeberry"),
                    ('T', "Walnut"),
                    ('H', "WitchHazel")
                )
                .StartWavePoolData(0, "Wave 1")
                .ConstructWaves(3, 0, "CBL")
                .StartWavePoolData(1, "Wave 2")
                .ConstructWaves(3, 1, "CPL", "CTL", "CLL")
                .StartWavePoolData(2, "Wave 3")
                .ConstructWaves(3, 9, "HTW")
                .AddBattleToLoader().RegisterBattle(1, mandatory: AlwaysFight)
                .GiveMiniBossesCharms(new[] { "Squirrel" }, "CardUpgradeAcorn", "CardUpgradeBattle", "CardUpgradeHeart")
                .GiveMiniBossesCharms(new[] { "WitchHazel" }, "CardUpgradeShellOnKill", "CardUpgradeSun",
                    "CardUpgradeBattle",
                    "CardUpgradeHeart", "CardUpgradeAttackAndHealth");
        }

        private void AddShellCards()
        {
            AddWitchling();
            AddCashew();
            AddWalnut();
            AddSquirrel();
            AddWitchHazel();
        }

        private void AddWitchling()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("ShellWitchling", "Shell Witchling", idleAnim: "FloatAnimationProfile")
                .SetStats(4, 1, 2)
                .WithCardType("Enemy")
                .SetSprites("ShellWitchling.png", "ShellWitchlingBG.png")
                .WithValue(200)
                .WithBloodProfile("Blood Profile Husk")
                .SetTraits(this.TStack("Backline"))
                .SetStartWithEffect(this.SStack("On Turn Apply Shell To AllyInFrontOf", 2))
            );
        }

        private void AddCashew()
        {
            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenYAppliedTo>("When Shell Applied To Self Gain Attack")
                    .WithCanBeBoosted(true)
                    .WithText("When <keyword=shell>'d, gain <+{a}> attack")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXWhenYAppliedTo data)
                    {
                        data.whenAppliedToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        data.whenAppliedTypes = new[] { "shell" };
                        data.effectToApply = this.TryGet<StatusEffectData>("Increase Attack");
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                    })
            );

            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Cashew", "Hickory", idleAnim: "SwayAnimationProfile")
                .SetStats(8, 1, 3)
                .WithCardType("Enemy")
                .WithValue(250)
                .WithBloodProfile("Blood Profile Husk")
                .SetSprites("hickory.png", "hickoryBG.png")
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.startWithEffects = new[] { this.SStack("When Shell Applied To Self Gain Attack") };
                }));
        }

        private void AddWalnut()
        {
            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenDestroyed>("On Death Apply Shell To Random Ally")
                    .WithCanBeBoosted(true)
                    .WithText("When destroyed, apply <{a}><keyword=shell> to a random ally")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXWhenDestroyed data)
                    {
                        data.targetMustBeAlive = false;
                        data.effectToApply = this.TryGet<StatusEffectData>("Shell");
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomAlly;
                    })
            );


            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Walnut", "Hunker", idleAnim: "SwayAnimationProfile")
                .SetStats(1, 1, 3)
                .WithCardType("Enemy")
                .WithBloodProfile("Blood Profile Husk")
                .WithValue(250)
                .SetSprites("Hunker.png", "HunkerBG.png")
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.startWithEffects = new[]
                    {
                        // SStack("On Turn Apply Shell To Self"), 
                        this.SStack("Shell", 4),
                        this.SStack("On Death Apply Shell To Random Ally", 4)
                    };
                }));
        }

        private void AddSquirrel()
        {
            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectInstantTakeShellAsHealth>("Instant Take Shell")
                    .WithCanBeBoosted(true)
                    .WithText("Lose shell")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectInstantTakeShellAsHealth data)
                    {
                        var target = new TargetConstraintHasStatusType
                        {
                            statusType = "shell"
                        };
                        data.targetConstraints = new TargetConstraint[] { target };
                        //data.IncreaseHealthEffect = TryGet<StatusEffectData>("Increase Max Health");
                        data.IncreaseHealthEffect = this.TryGet<StatusEffectData>("Increase Attack");
                    })
            );

            statusEffects.Add(
                this.StatusCopy("On Turn Apply Scrap To RandomAlly", "Take Shell And Gain Equal Attack")
                    .WithText("Remove all <keyword=shell> from a random ally and gain equal <keyword=attack>")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXOnTurn)data;
                        castData.effectToApply = this.TryGet<StatusEffectData>("Instant Take Shell");
                        castData.desc =
                            "Before attacking, take <keyword=shell> from a random ally and gain equal <keyword=attack>";
                        castData.noTargetType = NoTargetType.NoTargetForStatus;
                        castData.noTargetTypeArgs = new[] { "<sprite name=shell>" };
                        var target = new TargetConstraintHasStatusType
                        {
                            statusType = "shell"
                        };
                        castData.applyConstraints = new TargetConstraint[] { target };
                    })
            );

            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Squirrel", "Squirrel", idleAnim: "FloatAnimationProfile")
                .SetStats(20, 1, 2)
                .WithCardType("Miniboss")
                .WithValue(500)
                .WithBloodProfile("Blood Profile Husk")
                .SetSprites("Piri.png", "PiriBG.png")
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.startWithEffects = new[]
                        { this.SStack("Take Shell And Gain Equal Attack"), this.SStack("ImmuneToSnow") };
                })
            );
        }

        private void AddWitchHazel()
        {
            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectInstantConvertShellToTeeth>("Instant Convert Shell To Teeth")
                    .WithCanBeBoosted(true)
                    .WithText("Lose shell")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectInstantConvertShellToTeeth data)
                    {
                        var target = new TargetConstraintHasStatusType
                        {
                            statusType = "shell"
                        };
                        data.targetConstraints = new TargetConstraint[] { target };
                        data.TeethEffect = this.TryGet<StatusEffectData>("Teeth");
                    })
            );

            statusEffects.Add(
                this.StatusCopy("On Turn Apply Scrap To RandomAlly", "On Turn Convert Ally Shell To Teeth")
                    .WithText("Convert a random ally's <keyword=shell> to <keyword=teeth>")
                    .WithText("test", lang: SystemLanguage.Korean)
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXOnTurn)data;
                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomAlly;
                        castData.effectToApply = this.TryGet<StatusEffectData>("Instant Convert Shell To Teeth");
                        castData.desc =
                            "Convert a random ally's <keyword=shell> to <keyword=teeth>";
                        castData.noTargetType = NoTargetType.NoTargetForStatus;
                        castData.noTargetTypeArgs = new[] { "<sprite name=shell>" };
                        var target = new TargetConstraintHasStatusType
                        {
                            statusType = "teeth"
                        };
                        castData.applyConstraints = new TargetConstraint[] { target };
                    })
            );

            statusEffects.Add(
                this.StatusCopy("On Turn Apply Scrap To RandomAlly", "On Turn Convert Shell To Teeth")
                    .WithText("Convert <keyword=shell> to <keyword=teeth>")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXOnTurn)data;
                        castData.effectToApply = this.TryGet<StatusEffectData>("Instant Convert Shell To Teeth");
                        castData.desc =
                            "Convert a random ally's <keyword=shell> to <keyword=teeth>";
                        castData.noTargetType = NoTargetType.NoTargetForStatus;
                        castData.noTargetTypeArgs = new[] { "<sprite name=shell>" };
                        var target = new TargetConstraintHasStatusType
                        {
                            statusType = "shell"
                        };
                        castData.applyConstraints = new TargetConstraint[] { target };
                    })
            );

            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXRandom>("On Turn Convert Shell To Teeth")
                    .WithCanBeBoosted(true)
                    .WithText("Convert self or a random ally's <keyword=shell> to <keyword=teeth>")
                    .WithType("")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXRandom)data;
                        castData.effectToApply = this.TryGet<StatusEffectData>("Instant Convert Shell To Teeth");
                        data.desc =
                            "Convert self or a random ally's <keyword=shell> to <keyword=teeth>";
                        castData.noTargetType = NoTargetType.NoTargetForStatus;
                        castData.noTargetTypeArgs = new[] { "<sprite name=shell>" };
                        var target = new TargetConstraintHasStatusType
                        {
                            statusType = "teeth"
                        };
                        castData.applyConstraints = new TargetConstraint[] { target };
                    })
            );


            cards.Add(new CardDataBuilder(this)
                .CreateUnit("WitchHazel", "Witch Hazel", idleAnim: "FloatAnimationProfile")
                .SetStats(20, 4, 4)
                .WithCardType("Miniboss")
                .WithValue(500)
                .WithBloodProfile("Blood Profile Husk")
                .SetSprites("WitchHazel.png", "Witch HazelBG.png")
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.startWithEffects = new[]
                    {
                        this.SStack("On Turn Convert Ally Shell To Teeth"),
                        this.SStack("ImmuneToSnow"),
                        this.SStack("Shell", 3)
                    };
                })
            );
        }
    }
}