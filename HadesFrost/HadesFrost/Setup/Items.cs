using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.StatusEffects;
using HadesFrost.Utils;
using UnityEngine;

namespace HadesFrost.Setup
{
    public static class Items
    {
        public static void Setup(HadesFrost mod)
        {
            PomSlice(mod);
            Skelly(mod);
            Nectar(mod);
            Ambrosia(mod);

            FrostbittenHorn(mod);
            ThunderSignet(mod);//
            IridescentFan(mod);//
            AdamantShard(mod);

            WitchsStaff(mod);
            MoonstoneAxe(mod);
            SisterBlades(mod); 

            UmbralFlames(mod);
            ArgentSkull(mod);
        }

        private static void Nectar(HadesFrost mod)
        {
            // restore equal to num allies, barrage?
            // heal to max, haze? eh
            // +1 health +1 attack barrage
            //  nectar restore 3 and cleanse
            // critical
            // increase by 1 on use ??

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
                    .SetSprites("Nectar.png", "NectarBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .AddPool("GeneralItemPool")
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Increase Max Health", 2) // increase? 2 
                        };
                        data.startWithEffects = new[]
                        {
                            mod.SStack("On Card Played Increase Attack Effect 1 To Self"),
                        };
                    }));
        }

        private static void Ambrosia(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                mod.StatusCopy("Cleanse", "Cleanse With Text")
                    .WithText("<keyword=cleanse>"));

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("Ambrosia", "Ambrosia")
                    .SetSprites("Ambrosia.png", "AmbrosiaBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .AddPool("GeneralItemPool")
                    .WithValue(60)
                    .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Increase Max Health", 8), // change to increase 8?
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
                    .SetSprites("PomOfPower.png", "PomOfPowerBG.png")
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
                    .SetSprites("ThunderSignet.png", "ThunderSignetBG.png")
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
                    .SetSprites("IridescentFan.png", "IridescentFanBG.png")
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

        private static void AdamantShard(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                mod.StatusCopy("Instant Apply Frenzy (To Card In Hand)", "Instant Damage (To Card In Hand)")
                    .WithText("Add <+{a}><keyword=attack> to an <item> in your hand")
                    .FreeModify(data =>
                    {
                        var castData = (StatusEffectApplyXInstant)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Increase Attack").InstantiateKeepName();
                        
                        var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesDamage>();
                        var constraintItem = ScriptableObject.CreateInstance<TargetConstraintIsItem>();

                        castData.targetConstraints = new TargetConstraint[] { constraintItem, constraintAttack };
                    }));

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("AdamantShard", "Adamant Shard")
                    .SetSprites("AdamantShard.png", "AdamantShardBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .WithValue(40)
                    .SetTraits(mod.TStack("Noomlin"), mod.TStack("Consume"))
                    .CanPlayOnHand()
                    .CanPlayOnBoard(false)
                    .NeedsTarget()
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Instant Damage (To Card In Hand)", 3),
                        };
                    }));
        }

        private static void FrostbittenHorn(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("FrostbittenHorn", "Frostbitten Horn")
                    .SetSprites("FrostbittenHorn.png", "FrostbittenHornBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .WithValue(40)
                    .SetDamage(0)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Snow", 2),
                            mod.SStack("Frost", 2)
                        };
                    }));
        }

        private static void WitchsStaff(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("WitchsStaff", "Witch's Staff")
                    .SetSprites("WitchsStaff.png", "WitchsStaffBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .SetDamage(1)
                    .WithValue(40)
                    .SetTraits(mod.TStack("Barrage"))
                    //.SetStartWithEffect(mod.SStack("On Card Played Apply Attack To Self"))
                    .SetStartWithEffect(mod.SStack("On Kill Apply Attack To Self"))
                );
        }

        private static void SisterBlades(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("SisterBlades", "Sister Blades")
                    .SetSprites("SisterBlades.png", "SisterBladesBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .SetDamage(1)
                    .WithValue(40)
                    .SetStartWithEffect(mod.SStack("MultiHit"))
            );
        }

        private static void ArgentSkull(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                mod.StatusCopy("Summon Fallow", "Summon ArgentSkullShell")
                    .WithText($"Summon <card={Extensions.PrefixGUID("ArgentSkullShell", mod)}>")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        ((StatusEffectSummon)data).summonCard = mod.TryGet<CardData>("ArgentSkullShell");
                    })
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("ArgentSkull", "Argent Skull")
                    .SetSprites("ArgentSkull.png", "ArgentSkullBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .CanPlayOnBoard()
                    .CanPlayOnFriendly()
                    .CanShoveToOtherRow()
                    .NeedsTarget()
                    .WithPlayType(Card.PlayType.Play)
                    .FreeModify(data =>
                    {
                        data.playOnSlot = true;
                    })
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.startWithEffects = new[]
                        {
                            mod.SStack("Summon ArgentSkullShell"),
                            mod.SStack("Uses", 3)
                        };
                    })
                    .WithValue(40)
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateUnit("ArgentSkullShell", "Argent Skull Shell")
                    .SetSprites("ArgentSkull.png", "ArgentSkullBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .SetStats(1)
                    .SetTraits(mod.TStack("Explode", 3))
            );
        }

        private static void MoonstoneAxe(HadesFrost mod)
        {
            var chargeEffect =
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectCharge>("Gain Attack While In Hand")
                    .WithCanBeBoosted(true)
                    .WithText("")
                    .WithType("")
                    .FreeModify(delegate(StatusEffectCharge data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        data.effectToApply = mod.TryGet<StatusEffectData>("Ongoing Increase Attack")
                            .InstantiateKeepName();
                    });

            var chargeKeyword = new KeywordDataBuilder(mod)
                .Create("Charge")
                .WithCanStack(false)
                .WithDescription("<+1><keyword=attack> each turn spent in hand\nReset <keyword=attack> when played or discarded")
                .WithShowName(true)
                .WithShowIcon(false)
                .WithTitle("Charge");

            mod.Keywords.Add(chargeKeyword);
            mod.StatusEffects.Add(chargeEffect);

            mod.Traits.Add(new TraitDataBuilder(mod)
                .Create("Charge")
                .WithOverrides(mod.TryGet<TraitData>("Pull"), mod.TryGet<TraitData>("Barrage"))
                .WithKeyword(chargeKeyword.Build())
                .WithEffects(chargeEffect.Build())
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("MoonstoneAxe", "Moonstone Axe")
                    .SetSprites("MoonstoneAxe.png", "MoonstoneAxeBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .SetDamage(1)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        // data.startWithEffects = new[]
                        // {
                        //     mod.SStack("Gain Attack While In Hand")
                        // };
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Charge")
                        };
                    }));
        }
        private static void UmbralFlames(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("UmbralFlames", "Umbral Flames")
                    .SetSprites("UmbralFlames.png", "UmbralFlamesBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .SetDamage(0)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Overload", 3)
                        };
                        // data.startWithEffects = new[]
                        // {
                        //     mod.SStack("MultiHit")
                        // };
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Combo")
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
                    .SetStats()
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