using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using HarmonyLib;
using UnityEngine;
using WildfrostHopeMod;
using WildfrostHopeMod.Configs;


namespace HadesFrost
{
    public class HadesFrost : WildfrostMod
    {
        public HadesFrost(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.hadesfrost";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Hades Frost";

        public override string Description => @":)";

        private readonly List<CardDataBuilder> cards = new List<CardDataBuilder>();
        private readonly List<StatusEffectDataBuilder> statusEffects = new List<StatusEffectDataBuilder>();
        private bool preLoaded;

        private void CreateModAssets()
        {
            Hephaestus();

            Demeter();

            Aphrodite();

            Hestia();

            Artemis();

            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Melinoe", "Melinoë", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .SetStats(10, 5, 4)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                }));

            preLoaded = true;
        }

        private void Artemis()
        {
            statusEffects.Add(
                this.StatusCopy(
                        "When Enemy (Shroomed) Is Killed Apply Their Shroom To RandomEnemy",
                        "When Enemy (Demonized) Is Killed Apply Their Demonize To RandomEnemy")
                    .WithText("When a <keyword=demonize>'d enemy dies, apply their <keyword=demonize> to a random enemy")
                    .SubscribeToAfterAllBuildEvent(delegate(StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXWhenUnitIsKilled)data;

                        var constraint = ScriptableObject.CreateInstance<TargetConstraintHasStatus>();
                        constraint.status = this.TryGet<StatusEffectData>("Demonize");
                        castData.unitConstraints = new TargetConstraint[] { constraint };
                        var amount = new ScriptableCurrentStatus
                        {
                            statusType = "demonize"
                        };
                        castData.contextEqualAmount = amount;
                        castData.effectToApply = this.TryGet<StatusEffectData>("Demonize");
                        castData.noTargetTypeArgs = new[] { "<sprite name=demonize>" };
                    }));

            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Artemis", "Artemis", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .SetStats(4, 4, 5)
                .AddPool("BasicUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "The power of the hunt helps keep me company, so... maybe it'll help you, too!",
                        "Finally tracked you down...",
                        "Need a hunting partner?"
                    };
                    data.attackEffects = new[]
                    {
                        this.SStack("Demonize"),
                    };
                    data.startWithEffects = new[]
                    {
                        this.SStack("When Enemy (Demonized) Is Killed Apply Their Demonize To RandomEnemy")
                    };
                    data.traits = new List<CardData.TraitStacks>
                    {
                        this.TStack("Longshot")
                    };
                }));
        }

        private void Hestia()
        {
            statusEffects.Add(
                this.StatusCopy("When Enemy Is Hit By Item Apply Demonize To Them",
                        "When Enemy Is Hit By Item Apply Overburn To Them")
                    .WithText("When an enemy is hit with an <Item>, apply <{a}><keyword=overload> to them")
                    .SubscribeToAfterAllBuildEvent(delegate(StatusEffectData data)
                    {
                        var castData = (StatusEffectApplyXWhenUnitIsHit)data;
                        castData.effectToApply = this.TryGet<StatusEffectData>("Overload");
                        castData.isReaction = true;
                    }));

            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Hestia", "Hestia", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .SetStats(5)
                .AddPool("BasicUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "This old flame can never be put out, and don't you forget it, dearie!",
                        "Is something burning over there, dearie? Well it's about to be!",
                        "Been far too long since we incinerated stuff together, hasn't it?"
                    };
                    data.startWithEffects = new[]
                    {
                        this.SStack("When Enemy Is Hit By Item Apply Overburn To Them")
                    };
                }));
        }

        private void Aphrodite()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Aphrodite", "Aphrodite", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .SetStats(4, 0, 5)
                .AddPool("BasicUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "How lovely running into you again, gorgeous!",
                        "So good running into you again, sweetness!",
                        "Whoever's on your bad side, love, they're on mine as well."
                    };
                    data.attackEffects = new[]
                    {
                        this.SStack("Frost", 3),
                        // this.SStack() Apply haze if attack greater than?
                    };
                }));
        }

        private void Demeter()
        {
            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectChangeTargetMode>("Hits All Snowed Enemies")
                    .WithCanBeBoosted(true)
                    .WithText("Hits all <keyword=snow>'d enemies")
                    .WithType("")
                    .FreeModify(delegate(StatusEffectChangeTargetMode data)
                    {
                        var targetMode = ScriptableObject.CreateInstance<TargetModeStatus>();
                        targetMode.targetType = "snow";
                        data.targetMode = targetMode;
                        data.targetMode = targetMode;
                    })
            );

            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Demeter", "Demeter", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .SetStats(8, 2, 3)
                .AddPool("BasicUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "You have your mother's strength. Take mine as well.",
                        "So many traitors stand against us now, but soon they all shall lie in ruin and decay.",
                        "Cold vengeance grows deep inside our hearts; let it flourish for a while."
                    };
                    data.startWithEffects = new[]
                    {
                        this.SStack("ImmuneToSnow"),
                        this.SStack("When Hit Apply Snow To Attacker", 2),
                        this.SStack("Hits All Snowed Enemies")
                    };
                }));
        }

        private void Hephaestus()
        {
            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXOnTurn>("On Turn Add Damage to Items In Hand")
                    .WithCanBeBoosted(true)
                    .WithText("Apply +<{a}><keyword=attack> to items in hand")
                    .WithType("")
                    .FreeModify(delegate(StatusEffectApplyXOnTurn data)
                    {
                        var constraintAttack = ScriptableObject.CreateInstance<TargetConstraintDoesAttack>();
                        var constraintItem = ScriptableObject.CreateInstance<TargetConstraintIsItem>();

                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Hand;
                        data.effectToApply = this.TryGet<StatusEffectData>("Increase Attack").InstantiateKeepName();
                        data.canBeBoosted = true;
                        data.applyConstraints = new TargetConstraint[] { constraintAttack, constraintItem };
                        data.desc = "Apply +<{a}><keyword=attack> to items in hand";
                    })
            );

            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Hephaestus", "Hephaestus", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .SetStats(7, 3, 4)
                .AddPool("BasicUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "I don't normally just give away my services, but it ain't normally of late.",
                        "Ehh, fancy meeting you again and all, now let me get to work.",
                        "Hold up those flashy blades of yours for me and I'll have 'em sharper than ever."
                    };
                    data.startWithEffects = new[]
                    {
                        this.SStack("On Turn Add Damage to Items In Hand")
                    };
                }));
        }

        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }

            base.Load();
        }

        public override void Unload()
        {
            preLoaded = false;

            base.Unload();
        }

        public override List<T> AddAssets<T, Y>()          
        {
            var typeName = typeof(Y).Name;
            switch (typeName)                               
            {
                case nameof(CardData):
                    return cards.Cast<T>().ToList();        
                case nameof(StatusEffectData):
                    return statusEffects.Cast<T>().ToList(); 
                default:
                    return null;
            }
        }
    }
}