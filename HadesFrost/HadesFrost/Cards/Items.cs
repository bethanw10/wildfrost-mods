using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.StatusEffects;
using HadesFrost.TargetModes;
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
            WitchsStaff(mod);
            MoonstoneAxe(mod);
            SisterBlades(mod);
            FrostbittenHorn(mod);

            UmbralFlames(mod);
            ArgentSkull(mod);
            BlackCoat(mod);

            Ambrosia(mod);

            ThunderSignet(mod);
            IridescentFan(mod);
            AdamantShard(mod);

            // Zagreus
            StygianBlade(mod); 
            ShieldOfChaos(mod);
            EternalSpear(mod);
            LambentPlume(mod);
            Schelemeus(mod);
        }

        private static void StygianBlade(HadesFrost mod)
        {
            var effect =
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectChangeTargetMode>("Slash")
                    .WithCanBeBoosted(true)
                    .WithText("")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectChangeTargetMode data)
                    {
                        data.targetMode = ScriptableObject.CreateInstance<TargetModeSlash>();
                    });

            var chargeKeyword = new KeywordDataBuilder(mod)
                .Create("Slash")
                .WithCanStack(false)
                .WithDescription("Also hits the adjacent enemy in the other row")
                .WithShowName(true)
                .WithShowIcon(false)
                .WithTitle("Slash");

            mod.Keywords.Add(chargeKeyword);
            mod.StatusEffects.Add(effect);

            mod.Traits.Add(new TraitDataBuilder(mod)
                .Create("Slash")
                .WithOverrides(
                    mod.TryGet<TraitData>("Barrage"),
                    mod.TryGet<TraitData>("Longshot"),
                    mod.TryGet<TraitData>("Aimless")
                )
                .WithKeyword(chargeKeyword.Build())
                .WithEffects(effect.Build())
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("StygianBlade", "Stygian Blade")
                    .SetSprites("StygianBlade.png", "StygianBladeBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .SetDamage(2)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Slash")
                        };
                    }));
        }

        private static void ShieldOfChaos(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                mod.StatusCopy("On Hit Equal Heal To FrontAlly", "On Hit Equal Shell To FrontAlly")
                    .WithText("Apply <keyword=shell> to front ally equal to damage dealt")
                    .FreeModify(data =>
                    {
                        var castData = (StatusEffectApplyXOnHit)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Shell").InstantiateKeepName();
                        castData.noTargetTypeArgs = new[] { "shell" };
                    }));

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("ShieldOfChaos", "Shield of Chaos")
                    .SetSprites("ShieldOfChaos.png", "ShieldOfChaosBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .SetDamage(2)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.startWithEffects = new[]
                        {
                            mod.SStack("On Hit Equal Shell To FrontAlly")
                        };
                    }));
        }

        private static void EternalSpear(HadesFrost mod)
        {
            var effect =
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectChangeTargetMode>("Pierce")
                    .WithCanBeBoosted(true)
                    .WithText("")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectChangeTargetMode data)
                    {
                        data.targetMode = ScriptableObject.CreateInstance<TargetModePierce>();
                    });

            var chargeKeyword = new KeywordDataBuilder(mod)
                .Create("Pierce")
                .WithCanStack(false)
                .WithDescription("Also hits the enemy behind")
                .WithShowName(true)
                .WithShowIcon(false)
                .WithTitle("Pierce");

            mod.Keywords.Add(chargeKeyword);
            mod.StatusEffects.Add(effect);

            mod.Traits.Add(new TraitDataBuilder(mod)
                .Create("Pierce")
                .WithOverrides(
                    mod.TryGet<TraitData>("Barrage"), 
                    mod.TryGet<TraitData>("Longshot"),
                    mod.TryGet<TraitData>("Aimless")
                )
                .WithKeyword(chargeKeyword.Build())
                .WithEffects(effect.Build())
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("EternalSpear", "Eternal Spear")
                    .SetSprites("EternalSpear.png", "EternalSpearBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .SetDamage(2)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Pierce")
                        };
                    }));
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
                    .CanPlayOnHand()
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Heal", 2) // heal 3?
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
                    .CanPlayOnHand()
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
                    .WithIdleAnimationProfile("Heartbeat2AnimationProfile")
                    .SetDamage(1)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Jolted", 7),
                        };
                    }));
        }

        private static void IridescentFan(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("IridescentFan", "Iridescent Fan")
                    .SetSprites("IridescentFan.png", "IridescentFanBG.png")
                    .WithIdleAnimationProfile("FlyAnimationProfile")
                    .SetDamage(0)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Hitch", 2),
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
                    .WithText("Add <+{a}><keyword=attack> to a card in your hand")
                    .FreeModify(data =>
                    {
                        var castData = (StatusEffectApplyXInstant)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Increase Attack").InstantiateKeepName();
                        
                        var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesDamage>();

                        castData.targetConstraints = new TargetConstraint[] { constraintAttack };
                    }));

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("AdamantShard", "Adamant Shard")
                    .SetSprites("AdamantShard.png", "AdamantShardBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .WithValue(40)
                    .SetTraits(mod.TStack("Consume"))
                    .CanPlayOnHand()
                    .CanPlayOnBoard(false)
                    .NeedsTarget()
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Instant Damage (To Card In Hand)", 5),
                        };

                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Consume")
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

        private static void LambentPlume(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("LambentPlume", "Lambent Plume")
                    .SetSprites("LambentPlume.png", "LambentPlumeBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .WithValue(40)
                    .SetDamage(0)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[]
                        {
                            mod.SStack("Reduce Counter")
                        };
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Draw"), mod.TStack("Zoomlin")
                        };
                    }));
        }

        private static void WitchsStaff(HadesFrost mod)
        {
            var bonus =
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectBonusDamageEqualToX>("Bonus Damage Companions")
                    .WithCanBeBoosted(false)
                    .WithText("Deal +1 bonus damage for each active companion")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectBonusDamageEqualToX data)
                    {
                        data.add = true;
                        data.on = StatusEffectBonusDamageEqualToX.On.ScriptableAmount;
                        data.scriptableAmount = new ScriptableNumAllies();
                    });

            mod.StatusEffects.Add(bonus);

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("WitchsStaff", "Witch's Staff")
                    .SetSprites("WitchsStaff.png", "WitchsStaffBG.png")
                    .WithIdleAnimationProfile("ShakeAnimationProfile")
                    .SetDamage(1)
                    .SetTraits(mod.TStack("Barrage"))
                    //.SetStartWithEffect(mod.SStack("On Card Played Apply Attack To Self"))
                    //.SetStartWithEffect(mod.SStack("On Kill Apply Attack To Self", 2))
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.startWithEffects = new[]
                        {
                            // mod.SStack("Bonus Damage Companions"),
                            mod.SStack("On Hit Damage Undamaged Target"),
                        };
                    })
                );
        }

        private static void SisterBlades(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("SisterBlades", "Sister Blades")
                    .SetSprites("SisterBlades.png", "SisterBladesBG.png")
                    .WithIdleAnimationProfile("ShakeAnimationProfile")
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
                    .WithIdleAnimationProfile("Heartbeat2AnimationProfile")
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
                    .WithValue(45)
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateUnit("ArgentSkullShell", "Argent Skull Shell")
                    .SetSprites("ArgentSkull.png", "ArgentSkullBG.png")
                    .WithIdleAnimationProfile("Heartbeat2AnimationProfile")
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
                .WithCanStack(true)
                .WithDescription("Gain <keyword=attack> each turn spent in hand.\n\nReset <keyword=attack> when played or discarded")
                .WithShowName(true)
                .WithShowIcon(false)
                .WithTitle("Charge");

            mod.Keywords.Add(chargeKeyword);
            mod.StatusEffects.Add(chargeEffect);

            mod.Traits.Add(new TraitDataBuilder(mod)
                .Create("Charge")
                .WithKeyword(chargeKeyword.Build())
                .WithEffects(chargeEffect.Build())
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("MoonstoneAxe", "Moonstone Axe")
                    .SetSprites("MoonstoneAxe.png", "MoonstoneAxeBG.png")
                    .WithIdleAnimationProfile("ShakeAnimationProfile")
                    .SetDamage(1)
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
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

        private static void BlackCoat(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("BlackCoat", "Black Coat")
                    .SetSprites("BlackCoat.png", "BlackCoatBG.png")
                    .WithIdleAnimationProfile("Heartbeat2AnimationProfile")
                    .SetDamage(6)
                    .WithValue(45)
                    .NeedsTarget(false)
                    .WithPlayType(Card.PlayType.Play)
                    .WithTargetMode(new TargetModeBombard())
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Bombard 1")
                        };
                    }));
        }

        private static void Skelly(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateUnit("Skelly", "Skelly")
                    .SetSprites("Skelly.png", "SkellyBG.png")
                    .WithIdleAnimationProfile()
                    .WithCardType("Clunker")
                    .SetStats()
                    .WithFlavour("Give me everything you got!!")
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.startWithEffects = new[]
                        {
                            mod.SStack("Scrap", 3),
                        };
                    }));
        }

        private static void Schelemeus(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateUnit("Schelemeus", "Schelemeus")
                    .SetSprites("Schelemeus.png", "SchelemeusBG.png")
                    .WithIdleAnimationProfile()
                    .WithCardType("Clunker")
                    .SetStats()
                    .WithFlavour("These old bones are more resilient than they appear")
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