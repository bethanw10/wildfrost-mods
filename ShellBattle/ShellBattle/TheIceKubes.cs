using BattleEditor;
using Deadpan.Enums.Engine.Components.Modding;
using JetBrains.Annotations;

namespace ShellBattle
{
    [UsedImplicitly]
    public partial class MoreFights
    {
        private void AddIceBattle()
        {
            new BattleDataEditor(this, "Ice")
                .SetSprite(ImagePath("Seedlings.png").ToSprite())
                .SetNameRef("Ice")
                .EnemyDictionary(
                    ('B', "BabySnowbo"),
                    ('K', "Smackgoon"),
                    ('N', "Snoolf"),
                    ('S', "Snale"),
                    ('F', "FrostCrab"),
                    ('R', "BrickWallBoss")
                )
                .StartWavePoolData(0, "Wave 1")
                .ConstructWaves(3, 0, "FBB")
                .StartWavePoolData(1, "Wave 2")
                .ConstructWaves(3, 1, "FKS")
                .StartWavePoolData(2, "Wave 3")
                .ConstructWaves(3, 9, "RSS")
                .AddBattleToLoader().RegisterBattle(0, mandatory: false)
                .GiveMiniBossesCharms(new[] { "BrickWallBoss" }, "CardUpgradeBlue", "CardUpgradeBattle", "CardUpgradeHeart");
        }

        private void AddIceCards()
        {
            AddSnail();
            AddBoss();
            AddFrostCrab();
        }

        private void AddSnail()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Snale", "Snale", idleAnim: "SwayAnimationProfile")
                .SetStats(1, 1, 2)
                .WithCardType("Enemy")
                .WithValue(500)
                .WithBloodProfile("Blood Profile Snow")
                .SetSprites("hickory.png", "Witch HazelBG.png")
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.startWithEffects = new[]
                    {
                        this.SStack("Block")
                    };
                })
            );
        }
        private void AddFrostCrab()
        {
            statusEffects.Add(
                this.StatusCopy("When Destroyed Apply Overload To Attacker", "When Destroyed Apply Frost To Attacker")
                    .WithText("When destroyed, apply <{a}><keyword=frost> to the attacker")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXWhenDestroyed)data;
                        castData.effectToApply = this.TryGet<StatusEffectData>("Frost");
                    }));

            cards.Add(new CardDataBuilder(this)
                .CreateUnit("FrostCrab", "Frost Krab", idleAnim: "SwayAnimationProfile")
                .SetStats(4, 1, 3)
                .WithCardType("Enemy")
                .WithValue(500)
                .WithBloodProfile("Blood Profile Snow")
                .SetSprites("Hunker.png", "Witch HazelBG.png")
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.startWithEffects = new[]
                    {
                        this.SStack("When Destroyed Apply Frost To Attacker")
                    };
                })
            );
        }

        private void AddBoss()
        {
            statusEffects.Add(
                this.StatusCopy("Trigger When Self Or Ally Loses Block", "On Ally Block Lost Gain Block")
                    .WithText("When an ally loses block, gain <{a}> block")
                    .SubscribeToAfterAllBuildEvent(delegate(StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXWhenUnitLosesY)data;
                        castData.effectToApply = this.TryGet<StatusEffectData>("Block");
                        castData.self = false;
                        castData.descColorHex = "";
                        castData.canBeBoosted = true;
                        castData.isReaction = false;
                    }));

            statusEffects.Add(
                this.StatusCopy("Bonus Damage Equal To Shell", "Bonus Damage Equal To Block")
                    .WithText("Deal additional damage equal to <keyword=block>")
                    .SubscribeToAfterAllBuildEvent(delegate(StatusEffectData data)
                    {
                        var castData = (StatusEffectBonusDamageEqualToX)data;
                        castData.effectType = "block";
                    }));

           cards.Add(new CardDataBuilder(this)
                .CreateUnit("BrickWallBoss", "Brick", idleAnim: "FloatAnimationProfile")
                .SetStats(5, 1, 4)
                .WithCardType("Miniboss")
                .WithValue(500)
                .WithBloodProfile("Blood Profile Snow")
                .SetSprites("WitchHazel.png", "Witch HazelBG.png")
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.startWithEffects = new[]
                    {
                        this.SStack("On Ally Block Lost Gain Block"),
                        this.SStack("Bonus Damage Equal To Block"),
                        this.SStack("ImmuneToSnow"),
                        this.SStack("Block")
                    };
                })
            );
        }
    }
}
