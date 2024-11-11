using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Utils;
using UnityEngine;

namespace HadesFrost.Setup
{
    public static class Charms
    {
        public static void Setup(HadesFrost mod)
        {
            BlackShawl(mod);
            BoneHourglass(mod);
            DiscordantBell(mod);
            LionFang(mod);

            VividSea(mod);
            CloudBangle(mod);
        }

        private static void BlackShawl(HadesFrost mod)
        {
            var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesDamage>();

            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "On Hit Damage Damaged Target",
                        "On Hit Damage Undamaged Target")
                    .WithText("Deal <{a}> additional damage to undamaged foes")
                    .SubscribeToAfterAllBuildEvent(delegate(StatusEffectData data)
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
                    .WithImage("BlackShawlCharm.png")
                    .WithTitle("Black Shawl Charm")
                    .WithText("Deal <3> additional damage to undamaged foes")
                    .SetConstraints(constraintAttack)
                    .WithTier(2)
                    .SetEffects(
                        new CardData.StatusEffectStacks(mod.TryGet<StatusEffectData>("On Turn Apply Attack To Self"), 1))
                    .SubscribeToAfterAllBuildEvent(delegate(CardUpgradeData data)
                    {
                        data.effects = new[] { mod.SStack("On Hit Damage Undamaged Target", 3) };
                    })
            );
        }

        private static void BoneHourglass(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectUses>("Uses")
                    .WithCanBeBoosted(true)
                    .WithText("<{a}> use(s) left")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectUses data)
                    {
                        
                    })
            );

            var constraintConsume = ScriptableObject.CreateInstance<TargetConstraintHasTrait>();
            constraintConsume.trait = mod.TryGet<TraitData>("Consume");

            mod.CardUpgrades.Add(
                new CardUpgradeDataBuilder(mod)
                    .CreateCharm("CardUpgradeBoneHourglass")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("BoneHourglassCharm.png")
                    .WithTitle("Bone Hourglass Charm")
                    .WithText("Gives items with <keyword=consume> an extra use")
                    .SetConstraints(constraintConsume)
                    .WithTier(2)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        var lose = ScriptableObject.CreateInstance<StatusEffectInstantLoseTrait>();
                        lose.traitToLose = mod.TryGet<TraitData>("Consume");

                        data.effects = new[]
                        {
                            mod.SStack("Uses", 2)
                        };
                        var script = ScriptableObject.CreateInstance<CardScriptRemoveTrait>();
                        script.toRemove = new[] { mod.TryGet<TraitData>("Consume") };
                        data.scripts = new CardScript[] { script };
                    })
            );
        }

        private static void DiscordantBell(HadesFrost mod)
        {
            var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesDamage>();
            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            mod.CardUpgrades.Add(
                new CardUpgradeDataBuilder(mod)
                    .CreateCharm("CardUpgradeDiscordantBell")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("DiscordantBellCharm.png")
                    .WithTitle("Discordant Bell Charm")
                    .WithText("<+3><keyword=attack>\nStart with <1><keyword=weakness>")
                    .SetConstraints(constraintAttack, constraintUnit)
                    .WithTier(2)
                    .ChangeDamage(3)
                    .SetEffects(mod.SStack("Weakness", 1))
            );
        }

        private static void LionFang(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "On Card Played Reduce Attack Effect 1 To Self",
                        "On Card Played Reduce Attack To Self")
                    .WithText("Reduce attack by <{a}>")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXOnCardPlayed)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Reduce Attack");
                    }));

            var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesDamage>();
            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            mod.CardUpgrades.Add(
                new CardUpgradeDataBuilder(mod)
                    .CreateCharm("CardUpgradeLionFang")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("LionFangCharm.png")
                    .WithTitle("Lion Fang")
                    .WithText("<+5><keyword=attack>\nLose <1><keyword=attack> after attacking")
                    .SetConstraints(constraintAttack, constraintUnit)
                    .WithTier(2)
                    .ChangeDamage(5)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.effects = new[]
                        {
                            mod.SStack("On Card Played Reduce Attack To Self")
                        };
                    })
            );
        }

        private static void VividSea(HadesFrost mod)
        {
            var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesDamage>();
            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintIsUnit>();

            mod.CardUpgrades.Add(
                new CardUpgradeDataBuilder(mod)
                    .CreateCharm("CardUpgradeVividSea")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("VividSeaCharm.png")
                    .WithTitle("Vivid Sea")
                    .WithText($"Gain <keyword={Extensions.PrefixGUID("knockback", mod)}>")
                    .SetConstraints(constraintAttack, constraintUnit)
                    .WithTier(2)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.giveTraits = new[] { mod.TStack("Knockback") };
                    })
            );
        }

        private static void CloudBangle(HadesFrost mod)
        {
            var constraintUnit = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();

            mod.CardUpgrades.Add(
                new CardUpgradeDataBuilder(mod)
                    .CreateCharm("CardUpgradeCloudBangle")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("CloudBangleCharm.png")
                    .WithTitle("Cloud Bangle")
                    .WithText($"Apply <2> <keyword=jolted>")
                    .SetConstraints(constraintUnit)
                    .WithTier(2)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.attackEffects = new[] { mod.SStack("Jolted") };
                    })
            );
        }
    }
}