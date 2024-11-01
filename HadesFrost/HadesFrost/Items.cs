﻿using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Statuses;
using HadesFrost.Utils;

namespace HadesFrost
{
    public static class Items
    {
        public static void Setup(HadesFrost mod)
        {
            // PomOfPower(mod);

            PomSlice(mod);

            NectarAndAmbrosia(mod);

            ThunderSignet(mod);

            IridescentFan(mod);

            Coronacht(mod);

            Skelly(mod);
        }

        private static void NectarAndAmbrosia(HadesFrost mod)
        {
            // restore equal to num allies, barrage?
            // heal to max, haze? eh
            // +1 health +1 attack barrage
            //  nectar restore 3 and cleanse
            // critical
            // increase by 1 on use ??

            mod.StatusEffects.Add(
                mod.StatusCopy("Cleanse", "Cleanse With Text")
                    .WithText("<keyword=cleanse>"));

            mod.StatusEffects.Add(
                mod.StatusCopy("On Card Played Reduce Attack Effect 1 To Self", "On Card Played Increase Attack Effect 1 To Self")
                    .WithText("Increase by <{a}> when played")
                    .FreeModify(data =>
                    {
                        var castData = (StatusEffectApplyXOnCardPlayed)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Increase Attack Effects");
                    })
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("Nectar", "Nectar")
                    .SetSprites("Nectar.png", "AresBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .AddPool("GeneralItemPool")
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Heal", 3)
                        };
                        data.startWithEffects = new[]
                        {
                            mod.SStack("On Card Played Increase Attack Effect 1 To Self"),
                        };
                    }));

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("Ambrosia", "Ambrosia")
                    .SetSprites("Ambrosia.png", "AresBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .AddPool("GeneralItemPool")
                    .WithValue(60)
                    .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Heal", 10), // or just all?
                            mod.SStack("Cleanse With Text")
                        };
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Consume")
                        };
                    }));
        }

        private static void PomSlice(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("Pom Slice", "Pom Slice")
                    .SetSprites("PomOfPower.png", "AresBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .AddPool("GeneralItemPool")
                    .NeedsTarget(false)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                    {
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Consume"),
                        };
                        data.startWithEffects = new[]
                        {
                            mod.SStack("On Card Played Boost To RandomAlly"),
                        };
                    }));
        }

        private static void ThunderSignet(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("ThunderSignet", "Thunder Signet")
                    .SetSprites("ThunderSignet.png", "AresBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .SetDamage(0)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Jolted", 4),
                        };
                    }));
        }

        private static void IridescentFan(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("IridescentFan", "Iridescent Fan")
                    .SetSprites("IridescentFan.png", "AresBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .SetDamage(0)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Hitch"),
                        };

                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Barrage")
                        };
                    }));
        }

        private static void Coronacht(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectCharge>("Gain Attack While In Hand")
                    .WithCanBeBoosted(true)
                    .WithText("<+1><keyword=attack> each turn spent in hand, resets <keyword=attack> discard")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectCharge data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        data.effectToApply = mod.TryGet<StatusEffectData>("Ongoing Increase Attack").InstantiateKeepName();
                    })
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("Coronacht", "Coronacht")
                    .SetSprites("Coronacht.png", "AresBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .SetDamage(1)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.startWithEffects = new[]
                        {
                            mod.SStack("Gain Attack While In Hand")
                        };
                    }));
        }

        private static void Skelly(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateUnit("Skelly", "Skelly")
                    .SetSprites("Skelly.png", "SkellyBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .WithCardType("Clunker")
                    .SetStats(null, null, 0)
                    .WithFlavour("Give me everything you got!!")
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.startWithEffects = new[]
                        {
                            mod.SStack("Teeth"),
                            mod.SStack("Scrap", 3),
                        };
                    }));
        }
    }
}