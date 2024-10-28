using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;

namespace HadesFrost
{
    public static class Charms
    {
        public static void Setup(HadesFrost mod)
        {
            var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesDamage>();

            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "On Hit Damage Damaged Target",
                        "On Hit Damage Undamaged Target")
                    .WithText("Deal <{a}> additional damage to undamaged foes")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXOnHit)data;
                        var constraint = new TargetConstraintDamaged
                        {
                            not = true
                        };
                        castData.applyConstraints = new TargetConstraint[] { constraint };
                    }));

            mod.CardUpgrades.Add(
                new CardUpgradeDataBuilder(mod)
                    .CreateCharm("CardUpgradeBlackShawl")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("MakokoCharm.png")
                    .WithTitle("Black Shawl Charm")
                    .WithText("Deal <3> additional damage to undamaged foes")
                    .SetConstraints(constraintAttack)
                    .WithTier(2)
                    .SetEffects(new CardData.StatusEffectStacks(mod.TryGet<StatusEffectData>("On Turn Apply Attack To Self"), 1))
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        data.effects = new[] { mod.SStack("On Hit Damage Undamaged Target", 3) };
                    })
            );

            //--

            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "Instant Gain Fury",
                        "Instant Gain Consume")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXInstant)data;

                        castData.effectToApply = mod.TryGet<StatusEffectData>("Temporary Consume");
                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                    }));

            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "Lose Scrap",
                        "Lose Gain Consume")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectInstantLoseX)data;
                        castData.statusToLose = mod.TryGet<StatusEffectData>("Gain Consume When Played");
                    }));

            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "On Card Played Reduce Attack Effect 1 To Self",
                        "Gain Consume When Played")
                    .WithText("Gain <keyword=consume> when played")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXOnCardPlayed)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Temporary Consume");
                    }));
            //
            // statusEffects.Add(
            //     this.StatusCopy(
            //             "On Card Played Reduce Attack Effect 1 To Self",
            //             "Lose Gain Consume When Played")
            //         .WithVisible(false)
            //         .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
            //         {
            //             var castData = (StatusEffectApplyXOnCardPlayed)data;
            //             castData.effectToApply = this.TryGet<StatusEffectData>("Lose Gain Consume");
            //             castData.targetMustBeAlive = false;
            //         }));

            // //old
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXOnCardPlayed>("Lose Gain Consume When Played")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXOnCardPlayed)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Lose Gain Consume");
                        castData.targetMustBeAlive = false;
                    })
            );

            var constraintConsume = ScriptableObject.CreateInstance<TargetConstraintHasTrait>();
            constraintConsume.trait = mod.TryGet<TraitData>("Consume");

            mod.CardUpgrades.Add(
                new CardUpgradeDataBuilder(mod)
                    .CreateCharm("CardUpgradeBoneHourglass")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("MakokoCharm.png")
                    .WithTitle("Bone Hourglass Charm")
                    .WithText("Gives items with <keyword=consume> an extra use")
                    .SetConstraints(constraintConsume)
                    .WithTier(2)
                    .SetEffects(new CardData.StatusEffectStacks(mod.TryGet<StatusEffectData>("On Turn Apply Attack To Self"), 1))
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        var lose = ScriptableObject.CreateInstance<StatusEffectInstantLoseTrait>();
                        lose.traitToLose = mod.TryGet<TraitData>("Consume");

                        data.effects = new[]
                        {
                            mod.SStack("Gain Consume When Played"),
                            mod.SStack("Lose Gain Consume When Played"),
                        };
                        var script = ScriptableObject.CreateInstance<CardScriptRemoveTrait>();
                        script.toRemove = new[] { mod.TryGet<TraitData>("Consume") };
                        data.scripts = new CardScript[] { script };
                    })
            );
        }
    }
}