using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.StatusEffects;
using HadesFrost.TargetModes;
using HadesFrost.Utils;
using UnityEngine;

namespace HadesFrost.Setup
{
    public static class Companions
    {
        public static void Setup(HadesFrost mod)
        {
            Aphrodite(mod);
            Apollo(mod);
            Artemis(mod);
            Ares(mod);
            Athena(mod);
            Chaos(mod);
            Demeter(mod);
            Dionysus(mod);
            Hephaestus(mod);
            Hera(mod);
            Hermes(mod);
            Hestia(mod);
            Poseidon(mod);
            Zeus(mod);
        }

        private static void Ares(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXOnKillWithContext>("When Enemy Is Killed Gain Fury Equal To Attack")
                    .WithCanBeBoosted(false)
                    .WithText("On kill, gain <keyword=fury> equal to target's <keyword=attack>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXOnKillWithContext data)
                    {
                        data.effectToApply = mod.TryGet<StatusEffectData>("Instant Gain Fury");
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        data.contextEqualAmount = ScriptableObject.CreateInstance<ScriptableCurrentAttack>();
                        data.applyEqualAmount = true;
                    })
            );

            var boonStatus = SetupBoonStatus(mod, "Ares", "Battle Rage", "Leader gains 'Gain <+1><keyword=attack> on kill'");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Ares", "Ares")
                .SetSprites("Ares.png", "AresBG.png")
                .SetStats(3, 2, 3)
                
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "Let us together deal some death, shall we?",
                        "The devastation you have caused has not escaped my notice!",
                        "We stand before a golden opportunity, to inflict pain. Let us proceed!",
                        "Whenever you bring death, why, you've my full support.",
                        "I am quite eager to commence jointly killing your enemies, my kin.",
                        "It's not been long since last I saw bloodshed... but far too long for me, nevertheless.",
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("When Enemy Is Killed Gain Fury Equal To Attack")
                    };
                }));
        }

        private static void Artemis(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "When Enemy (Shroomed) Is Killed Apply Their Shroom To RandomEnemy",
                        "When Enemy (Demonized) Is Killed Apply Their Demonize To RandomEnemy")
                    .WithText("When a <keyword=demonize>'d enemy dies, apply their <keyword=demonize> to a random enemy")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXWhenUnitIsKilled)data;

                        var constraint = ScriptableObject.CreateInstance<TargetConstraintHasStatus>();
                        constraint.status = mod.TryGet<StatusEffectData>("Demonize");
                        castData.unitConstraints = new TargetConstraint[] { constraint };
                        var amount = ScriptableObject.CreateInstance<ScriptableCurrentStatus>();
                        amount.statusType = "demonize";
                        castData.contextEqualAmount = amount;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Demonize");
                        castData.noTargetTypeArgs = new[] { "<sprite name=demonize>" };
                    }));

            var boonStatus = SetupBoonStatus(mod, "Artemis", "Deadly Strike", "Leader gains 'Apply <1><keyword=demonize>'");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Artemis", "Artemis")
                .SetSprites("Artemis.png", "ArtemisBG.png")
                .SetStats(4, 4, 4)
                
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "How about I join you for this hunt, OK?",
                        "Finally tracked you down...",
                        "Need a hunting partner?",
                        "Got here as quickly as I could.",
                        "What are you waiting for, let's get to hunting, then!",
                        "Well, perfect timing, now let's stock up and head out.",
                        "Hello again, you ready for this hunt? Me, I've been looking forward since last time, so, let's get on with it.",
                    };
                    // data.attackEffects = new[]
                    // {
                    //     mod.SStack("Demonize"),
                    // };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("When Enemy (Demonized) Is Killed Apply Their Demonize To RandomEnemy")
                    };
                    data.traits = new List<CardData.TraitStacks>
                    {
                        mod.TStack("Longshot")
                    };
                }));
        }

        private static void Athena(HadesFrost mod)
        {
            var boonStatus = SetupBoonStatus(mod, "Athena", "Divine Protection", "Leader gains <1><keyword=block>");

            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "On Kill Apply Block To Self",
                        "On Kill Apply Block To RandomAlly")
                    .WithText("On kill, apply <{a}><keyword=block> to a random ally")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXOnKill)data;

                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomAlly;
                    }));

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Athena", "Athena")
                .SetSprites("Athena.png", "AthenaBG.png")
                .SetStats(5, 2, 3)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "I'm here again to lend to you my power, noble Cousin.",
                        "I know that you are more than capable even without me, Cousin. But just in case...",
                        "My support alone is not enough to see you through this, but it certainly shall help.",
                        "Your foes shall soon find it impossible to overcome your strength, combined with mine.",
                        "I shall do everything within my power to defend you from the perils of your journey.",
                        "I take it having an impenetrable defense against your enemies may be of some use, Cousin?",
                        "Vanquish all who stand against you, noble Cousin. I and your relatives upon Olympus shall assist you in so doing.",
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("On Kill Apply Block To RandomAlly") 
                    };
                }));
        }

        private static void Aphrodite(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXIfStatsAreLower>("Apply Haze If")
                    .WithCanBeBoosted(true)
                    .WithText("Apply <{a}><keyword=haze> if <keyword=attack> is at least <{a}> higher than the target's <keyword=attack>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXIfStatsAreLower data)
                    {
                        var constraint = ScriptableObject.CreateInstance<TargetConstraintDoesDamage>();
                        data.applyConstraints = new TargetConstraint[] { constraint };
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Target;
                        data.effectToApply = mod.TryGet<StatusEffectData>("Haze");
                        data.effectToApply.count = data.count; // ?
                        data.eventPriority = 2;
                    })
            );

            var boonStatus = SetupBoonStatus(mod, "Aphrodite", "Heartbreak Strike", "Leader gains 'Apply <1><keyword=frost>'");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Aphrodite", "Aphrodite")
                .SetSprites("Aphrodite.png", "AphroditeBG.png")
                .SetStats(3, 0, 5)
                
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "How lovely running into you again, gorgeous!",
                        "Let's see what mischief we can make, between the two of us!",
                        "Wherever you may go, there's always drama waiting for you, isn't there?",
                        "So good running into you again, sweetness!",
                        "Whoever's on your bad side, love, they're on mine as well.",
                        "Is not the purest act of love to aid somebody in their time of need?",
                        "I'd love to help you however best I'm able, my little godling.",
                        "You seem like you could use a helping hand, love.",
                        "Oh, dearest, I suspect we'll have ourselves a most exciting time together, you and I! Just do your best and it'll all be fine!",
                        "Would you by any chance have room, there, in your heart for a most-gentle goddess such as me?",
                        "Hello again, there, little godling, let's get to it, hm? I want what you want; there's no further need for words.",
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("Apply Haze If"),
                    };
                }));
        }

        private static void Apollo(HadesFrost mod)
        {
            var keyword = new KeywordDataBuilder(mod)
                .Create("Unyielding")
                .WithCanStack(false)
                .WithDescription("This card's <keyword=counter> can't be reduced by items/units during battle.")
                .WithShowName(true)
                .WithShowIcon(false)
                .WithTitle("Unyielding");

            var unyielding = new StatusEffectDataBuilder(mod)
                .Create<StatusEffectCannotReduceCounter>("Cannot Reduce Counter")
                .WithCanBeBoosted(false);

            mod.Keywords.Add(keyword);
            mod.StatusEffects.Add(unyielding);

            mod.Traits.Add(new TraitDataBuilder(mod)
                .Create("Unyielding")
                .WithKeyword(keyword.Build())
                .WithEffects(unyielding.Build())
            );

            // mod.Cards.Add(
            //     mod.CardCopy("SunRod", "RadiantSunRod")
            //     .SetTraits(mod.TStack("Consume"), mod.TStack("Zoomlin"))
            //     .WithTitle("Sun Rod")
            //     .SubscribeToAfterAllBuildEvent(delegate (CardData data)
            //     {
            //         var constraint = ScriptableObject.CreateInstance<TargetConstraintIsSpecificCard>();
            //         constraint.allowedCards = new[] { mod.TryGet<CardData>("Apollo") };
            //         constraint.not = true;
            //         data.targetConstraints = new TargetConstraint[] { constraint };
            //     }));
            //
            // mod.StatusEffects.Add(
            //     mod.StatusCopy("Summon SkullMuffin", "Summon SunRod")
            //         .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
            //         {
            //             ((StatusEffectSummon)data).summonCard = mod.TryGet<CardData>("RadiantSunRod");
            //         })
            // );
            //
            // mod.StatusEffects.Add(
            //     mod.StatusCopy(
            //             "On Card Played Add SkullMuffin To Hand",
            //             "On Card Played Add Sun Rod To Hand")
            //         .WithText("Add <{a}> <card=bethanw10.hadesfrost.RadiantSunRod> to your hand")
            //         .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
            //         {
            //             var castData = (StatusEffectApplyXOnCardPlayed)data;
            //             var effect = (StatusEffectInstantSummon)castData.effectToApply;
            //             effect.targetSummon = mod.TryGet<StatusEffectSummon>("Summon SunRod");
            //         }));


            // mod.Cards.Add(new CardDataBuilder(mod)
            //     .CreateUnit("Apollo", "Apollo", idleAnim: "FloatAnimationProfile")
            //     .SetSprites("Apollo.png", "ApolloBG.png")
            //     .SetStats(4, 4, 5)
            //     
            //     .SubscribeToAfterAllBuildEvent(delegate (CardData data)
            //     {
            //         data.startWithEffects = new[]
            //         {
            //             mod.SStack(boonStatus),
            //             mod.SStack("On Card Played Add Sun Rod To Hand")
            //         };
            //         data.traits = new List<CardData.TraitStacks>
            //         {
            //             mod.TStack("Unyielding")
            //         };
            //     }));
            
            var boonStatus = SetupBoonStatus(mod, "Apollo", "Perfect Image", "Leader gains 'While undamaged, <keyword=attack> is increased by <2>'");

            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXWhenEnemiesAttack>("Count Down When Enemies Attack")
                    .WithText("When an enemy attacks, count down <keyword=counter> by <{a}>")
                    .WithType("")
                    .WithCanBeBoosted(true)
                    .FreeModify(delegate (StatusEffectApplyXWhenEnemiesAttack data)
                    {
                        data.doPing = false;
                        data.effectToApply = mod.TryGet<StatusEffectData>("Reduce Counter");
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                    })
            );
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXOnCardPlayed>("Trigger Allies")
                    .WithText("Trigger all allies")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXOnCardPlayed data)
                    {
                        data.effectToApply = mod.TryGet<StatusEffectData>("Trigger (High Prio)");
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Allies;
                    })
            );
            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Apollo", "Apollo")
                .SetSprites("Apollo.png", "ApolloBG.png")
                .SetStats(8, null, 10)

                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "Let's show them a dazzling display they won't soon forget.",
                        "Brighter days are ahead, we just have to get through this rough patch together.",
                        "Shall we try and make these dark days a bit brighter, Cousin?",
                        "Must be something I can do to brighten up your evening there a bit?",
                        "Where there's light, there's hope, sunshine, so get set for some of each!",
                        "As you might have guessed, I'm taking this whole thing 'engulfing the sun in ice' thing very personally"
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("Trigger Allies"),
                        mod.SStack("Count Down When Enemies Attack"),
                    };
                }));
        }

        private static void Chaos(HadesFrost mod)
        {
            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Chaos", "Chaos")
                .SetSprites("Chaos.png", "Chaos.png")
                .SetStats(1, 1, 1)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "Please tell me how my power over all existence may be of small assistance to you this time, Spawn of Hades?",
                        "From my perspective I have summoned you, whilst you believe you came of your volition; both statements can be true, is that not so?",
                        "Come forth, O Son of Hades, and together, let us sow my namesake.",
                        "How shall we alter the intentions of the Fates?",
                        "How might I mend the fabric of existence for you, Spawn of Hades?",
                        "How you evaluate decisions is of limitless interest to me.",
                        "I require no exchange of words with you. Merely a choice.",
                        "Whatever shall transpire, I shall be curious to see.",
                        "There are no decisions that are not life-changing, Spawn of Hades."
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack("Block") // when deployed, random stats. // Apply random debuff
                    };
                }));
        }

        private static void Demeter(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectChangeTargetMode>("Hits All Snowed Enemies")
                    .WithText("Hits all <keyword=snow>'d enemies")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectChangeTargetMode data)
                    {
                        var targetMode = ScriptableObject.CreateInstance<TargetModeStatus>();
                        targetMode.targetType = "snow";
                        data.targetMode = targetMode;
                    })
            );

            var boonStatus = SetupBoonStatus(mod, "Demeter", "Ice Strike", "Leader gains 'Apply <1><keyword=snow>'");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Demeter", "Demeter", idleAnim: "PingAnimationProfile")
                .SetSprites("Demeter.png", "DemeterBG.png")
                .SetStats(8, 2, 3)
                .NeedsTarget(false)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "You have your mother's strength. Take mine as well.",
                        "So many traitors stand against us now, but soon they all shall lie in ruin and decay.",
                        "Cold vengeance grows deep inside our hearts; let it flourish for a while.",
                        "Death and decay to the enemies of Olympus."
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("ImmuneToSnow"),
                        mod.SStack("When Hit Apply Snow To Attacker", 2),
                        mod.SStack("Hits All Snowed Enemies")
                    };
                }));
        }

        private static void Dionysus(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXWhenEnemyTakesDamage>("When Shroom Damage Taken Heal")
                    .WithText("When enemy takes <keyword=shroom> damage, increase <keyword=health> of allies in the row by <{a}>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXWhenEnemyTakesDamage data)
                    {
                        data.TargetDamageType = "shroom";
                        data.effectToApply = mod.TryGet<StatusEffectData>("Increase Max Health");
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.AlliesInRow;
                        data.applyEqualAmount = false;
                    })
            );

            // mod.StatusEffects.Add(
            //     mod.StatusCopy("When Anyone Takes Shroom Damage Apply Attack To Self", "When Shroom Damage Taken Heal")
            //         .WithText("When anyone takes <keyword=shroom> damage, increase <keyword=health> of allies in the row by <{a}>")
            //         .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
            //         {
            //             var castData = (StatusEffectApplyXWhenAnyoneTakesDamage)data;
            //             // castData.effectToApply = mod.TryGet<StatusEffectData>("Heal (No Ping)");
            //             castData.effectToApply = mod.TryGet<StatusEffectData>("Increase Max Health");
            //             castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.AlliesInRow;
            //             castData.applyEqualAmount = false; 
            //         })
            // );

            //var boonStatus = SetupBoonStatus(mod, "Dionysus", "Premium Vintage", "Adds a <card=bethanw10.hadesfrost.Nectar> with <keyword=noomlin> to your deck");
            var boonStatus = SetupBoonStatus(
                mod, 
                "Dionysus", 
                "Premium Vintage",
                "Adds <keyword=noomlin> to all <card=bethanw10.hadesfrost.Nectar> or <card=bethanw10.hadesfrost.Ambrosia> in your deck");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Dionysus", "Dionysus", idleAnim: "PingAnimationProfile")
                .SetSprites("Dionysus.png", "DionysusBG.png")
                .SetStats(8, 0, 3)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "All right, man, I have got your back, and we have got this!",
                        "You and me. We're getting through this, now or never, ready, yeah?",
                        "What do you say we go all out this time, how about it, you with me, man, or what?",
                        "This time for sure, I mean, I know you're going to make it, together with me, man!",
                        "I got to make it to a feast in just a little bit, here, man, but quickly, let's go, yeah?"
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("When Shroom Damage Taken Heal")
                    };
                    data.attackEffects = new[]
                    {
                        mod.SStack("Shroom")
                    };
                }));
        }

        private static void Hera(HadesFrost mod)
        {
            var boonStatus = SetupBoonStatus(mod, "Hera", "Sworn Strike", "Leader gains 'Apply <1><keyword=hitch>'");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Hera", "Hera")
                .SetSprites("Hera.png", "HeraBG.png")
                .SetStats(5, 1, 3)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "Nothing like jointly annihilating our enemies to bring the family closer together, hm?",
                        "Aunt Hera's back, my dear, so let's have ourselves an interesting time!",
                        "Our family may be cursed by the Fates themselves, but we do know how to fight.",
                        "We on Olympus take our vengeance very personally.",
                        "I've enough power vested in me that I'm pleased to spare you some of it for now.",
                        "I promised the family that I'd take you under my wing. And I always keep my vows",
                    };
                    data.attackEffects = new[]
                    {
                        mod.SStack("Hitch", 2)
                    };
                    data.traits = new List<CardData.TraitStacks>
                    {
                        mod.TStack("Aimless")
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("MultiHit")
                    };
                }));
        }

        private static void Hermes(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXOnTurn>("On Turn Count Down Ally Ahead")
                    .WithCanBeBoosted(true)
                    .WithText("Count down <keyword=counter> of ally ahead by <{a}>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXOnTurn data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.AllyInFrontOf;
                        data.effectToApply = mod.TryGet<StatusEffectData>("Reduce Counter").InstantiateKeepName();
                    })
            );

            var boonStatus = SetupBoonStatus(mod, "Hermes", "Quick Buck", "Gain <75><keyword=blings> now");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Hermes", "Hermes")
                .SetSprites("Hermes.png", "HermesBG.png")
                .SetStats(2, null, 3)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "In a bit of a rush, here, so let's get moving, hey?",
                        "Lots going on! Been running around all night!",
                        "Live fast or not at all, right? Now hurry and take your pick!",
                        "No calamitous attack on our family is going to slow us down!",
                        "Message for you boss. It says, 'You're rather slow. If only somebody could help you out with that.'",
                        "Afraid there's no one faster than myself, here, Coz. But good news is, you are about to close the gap a little bit!",
                        "Last thing I ever want to do is slow you down, boss. So, enough chit-chat. Now pick and go. Go!",
                        "It's you, boss, that's good! But you're standing still! That's bad. Let's get you up and moving about again, all right?",
                        "Hey boss, found you, good. Hermes, at your service."
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack("On Turn Count Down Ally Ahead"),
                        mod.SStack(boonStatus),
                    };
                    data.traits = new List<CardData.TraitStacks>
                    {
                        mod.TStack("Draw")
                    };
                }));
        }

        private static void Hestia(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "When Enemy Is Hit By Item Apply Demonize To Them",
                        "When Enemy Is Hit By Item Apply Overburn To Them")
                    .WithText("When an enemy is hit with an <Item>, apply <{a}><keyword=overload> to them")
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXWhenUnitIsHit)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Overload");
                    }));

            var boonStatus = SetupBoonStatus(mod, "Hestia", "Flame Strike", "Leader gains 'Apply <1><keyword=overload>'");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Hestia", "Hestia", idleAnim: "SquishAnimationProfile")
                .SetSprites("Hestia.png", "HestiaBG.png")
                .SetStats(5)
                
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "This old flame can never be put out, and don't you forget it, dearie!",
                        "I'll melt through all this blasted frost, just you wait!",
                        "Well now I reckon it's time to fan the flames a bit, isn't it then?",
                        "It's far too cold around here... won't stay that way for long though!",
                        "Is something burning over there, dearie? Well it's about to be!",
                        "Been far too long since we incinerated stuff together, hasn't it?",
                        "Ah, let's burn it all down, what say you, dearie?",
                        "Sacrifices sometimes must be made, dearie. So let's round them up and put them to the flame!",
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("When Enemy Is Hit By Item Apply Overburn To Them")
                    };
                }));
        }

        private static void Hephaestus(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXOnTurn>("On Turn Add Damage to Items In Hand")
                    .WithCanBeBoosted(true)
                    .WithText("Apply <+{a}><keyword=attack> to items in hand")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXOnTurn data)
                    {
                        var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();
                        var constraintItem = ScriptableObject.CreateInstance<TargetConstraintIsItem>();

                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Hand;
                        data.effectToApply = mod.TryGet<StatusEffectData>("Increase Attack").InstantiateKeepName();
                        data.canBeBoosted = true;
                        data.applyConstraints = new TargetConstraint[] { constraintAttack, constraintItem };
                        data.desc = "Apply <+{a}><keyword=attack> to items in hand";
                    })
            );

            var boonStatus = SetupBoonStatus(mod, "Hephaestus", "Heavy Metal", "Leader gains <3><keyword=shell>");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Hephaestus", "Hephaestus", idleAnim: "GiantAnimationProfile")
                .SetSprites("Hephaestus.png", "HephaestusBG.png")
                .SetStats(8, 3, 4)
                
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "I don't normally just give away my services, but it ain't normally of late.",
                        "Ehh, fancy meeting you again and all, now let me get to work.",
                        "Hold up those flashy blades of yours for me and I'll have 'em sharper than ever.",
                        "Just clanking away as always, though let's do some clanking now on your behalf.",
                        "Fine weapons you got there. Though let's make 'em proper vicious here.",
                        "Fine chance for my handiwork to shine!"
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("On Turn Add Damage to Items In Hand")
                    };
                }));
        }

        private static void Poseidon(HadesFrost mod)
        {
            var knockback = new KeywordDataBuilder(mod)
                .Create("Knockback")
                .WithCanStack(false)
                .WithDescription("Deal damage to unit behind target, equal to target's <keyword=attack>\n\nPush target back one")
                .WithShowName(true)
                .WithShowIcon(false)
                .WithTitle("Knockback");

            var damageBehind = new StatusEffectDataBuilder(mod)
                .Create<StatusEffectKnockback>("Damage Behind And Push")
                .WithCanBeBoosted(false)
                .FreeModify(delegate (StatusEffectKnockback data)
                {
                    data.effectToApply = mod.TryGet<StatusEffectData>("Push");
                    data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Target;
                    data.doPing = false;
                    data.makesOffensive = true;
                    data.postHit = true;
                });

            mod.Keywords.Add(knockback);
            mod.StatusEffects.Add(damageBehind);

            mod.Traits.Add(new TraitDataBuilder(mod)
                .Create("Knockback")
                .WithOverrides(mod.TryGet<TraitData>("Pull"), mod.TryGet<TraitData>("Barrage"))
                .WithKeyword(knockback.Build())
                .WithEffects(damageBehind.Build())
            );

            mod.StatusEffects.Add(
                mod.StatusCopy(
                        "When Enemy Is Killed Apply Gold To Self",
                        "When Enemy Is Killed Gain Gold")
                    .WithText("While active, gain <{a}> extra <keyword=blings> when an enemy is killed")
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        ((StatusEffectApplyXWhenUnitIsKilled)data).sacrificed = false;
                    })
            );

            var boonStatus = SetupBoonStatus(mod, "Poseidon", "Water Fitness", "Leader gains <+3><keyword=health>");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Poseidon", "Poseidon")
                .SetSprites("Poseidon.png", "PoseidonBG.png")
                .SetStats(5, 4, 4)
                
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "The seas aren't going anywhere, and neither are we, right?",
                        "Hoh, your greatest uncle's here! Here to make sure we really make a splash!",
                        "The strength of earth and sea is yours again!",
                        "Come on, let's go batter down our enemies like ships within a storm",
                        "The tides are turning, can't you feel it? I can, of course, but surely you can, too!",
                        "You have the power of the seas right at your beck and call!",
                        "I'll never stop fighting for you! Not until the fighting stops! Perhaps not even then!",
                        "Come, let us crash upon our enemies as great waves upon a shore!",
                        "Many great battles have been fought upon my seas, but they'll all pale in comparison to this!",
                    };
                    data.traits = new List<CardData.TraitStacks>
                    {
                        mod.TStack("Knockback")
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus),
                        mod.SStack("When Enemy Is Killed Gain Gold")
                    };
                }));
        }

        private static void Zeus(HadesFrost mod)
        {
            var boonStatus = SetupBoonStatus(mod, "Zeus", "Lightning Strike", "Leader gains 'Apply <1><keyword=jolted>'");

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Zeus", "Zeus")
                .SetSprites("Zeus.png", "ZeusBG.png")
                .SetStats(6, 0, 3)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "Our enemies will soon be in for quite the shock, I think!",
                        "I trust we'll have all this unpleasantness behind us in no time, hm?",
                        "All those who stand against Olympus shall be summarily struck down.",
                        "The time for amicable solutions to our predicament has long since past.",
                        "Always wonderful to see you! Even in times such as these...",
                    };
                    data.attackEffects = new[]
                    {
                        mod.SStack("Jolted", 2)
                    };
                    data.traits = new List<CardData.TraitStacks>
                    {
                        mod.TStack("Barrage"), mod.TStack("Spark")
                    };
                    data.startWithEffects = new[]
                    {
                        mod.SStack(boonStatus)
                    };
                }));
        }

        private static string SetupBoonStatus(HadesFrost mod, string cardName, string title, string description)
        {
            var boon =
                new KeywordDataBuilder(mod)
                    .Create($"{cardName}Boon")
                    .WithShowName(true)
                    .WithShowIcon(false)
                    .WithTitle("Boon <sprite name=boonicon>\n" + title)
                    .WithCanStack(false)
                    .WithPanelColour(Color.grey)
                    .WithBodyColour(new Color(22, 28, 21))
                    .WithTitleColour(new Color(0.495f, 0.780f, 0.304f))
                    .WithDescription(description);

            mod.Keywords.Add(boon);
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectNothing>($"{cardName} Boon")
                    .WithOrder(-1)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.hiddenKeywords = new[] { mod.TryGet<KeywordData>($"{cardName.ToLower()}boon") };
                        data.visible = true;
                    }));

            return $"{cardName} Boon";
        }
    }
}