using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using TMPro;
using UnityEngine;
using WildfrostHopeMod.Utils;
using Object = UnityEngine.Object;


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

        public override string Description => ":)";

        private readonly List<CardDataBuilder> cards = new List<CardDataBuilder>();
        private readonly List<StatusEffectDataBuilder> statusEffects = new List<StatusEffectDataBuilder>();
        private readonly List<StatusEffectData> tempStatusEffects = new List<StatusEffectData>();

        public TMP_SpriteAsset hadesSprites;
        public override TMP_SpriteAsset SpriteAsset => hadesSprites;

        private void CreateModAssets()
        {
            Aphrodite();
            Apollo();
            Artemis();
            Ares();
            Athena();
            Demeter();
            Dionysus();
            Hephaestus();
            Hera();
            Hermes();
            Hestia();
            Poseidon();
            Zeus();

            Melinoe();

            this.CreateIconKeyword("jolted", "Jolted", "Take damage after triggering | Does not count down!", "joltedicon")
                .ChangeColor(note: new Color(0.98f, 0.89f, 0.61f));

            this.CreateIcon(
                "joltedicon", 
                ImagePath("joltedicon.png").ToSprite(), 
                "jolted", 
                "counter", 
                Color.black, 
                new[] { Get<KeywordData>("jolted") }, -1);

            SpriteAssetsFix();

            var jolted = StatusExt.CreateStatus<StatusEffectJolted>("Jolted", type: "jolted").Register(this, tempStatusEffects);
            jolted.iconGroupName = "health";
            jolted.visible = true;
            jolted.removeOnDiscard = true;
            jolted.isStatus = true;
            jolted.offensive = true;
            jolted.stackable = true;
            jolted.targetConstraints = new TargetConstraint[1] { ScriptableObject.CreateInstance<TargetConstraintIsUnit>() };
            jolted.textInsert = "{a}";
            jolted.keyword = "jolted";
            jolted.applyFormatKey = Get<StatusEffectData>("Shroom").applyFormatKey;

            Items();

            preLoaded = true;
        }

        private void SpriteAssetsFix()
        {
            hadesSprites = HopeUtils.CreateSpriteAsset(
                "hadesSprites",
                directoryWithPNGs: this.ImagePath("Sprites"),
                textures: new Texture2D[] { },
                sprites: new[] { ImagePath("joltedicon.png").ToSprite() });

            var ftext = Object.FindObjectOfType<FloatingText>(true);
            ftext.textAsset.spriteAsset.fallbackSpriteAssets.Add(hadesSprites);
        }

        private bool preLoaded;


        private void Items()
        {
            cards.Add(
                new CardDataBuilder(this)
                    .CreateItem("Pom of Power", "Pom of Power")
                    .SetSprites("Punch.png", "PunchBG.png")
                    .WithText("Boost the target's effects by 1")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .AddPool("GeneralItemPool")
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                    {
                        data.traits = new List<CardData.TraitStacks>
                        {
                            this.TStack("Barrage"), this.TStack("Consume"),
                        };
                        data.attackEffects = new[]
                        {
                            this.SStack("Instant Ongoing Increase Effects"),
                        };
                    }));

            /*
cards.Add(
    new CardDataBuilder(this)
        .CreateItem("Pom of Power", "Pom of Power")
        .SetSprites("Punch.png", "PunchBG.png")
        .WithText("Increase")
        .WithIdleAnimationProfile("PingAnimationProfile")
        .NeedsTarget()
        .AddPool("GeneralItemPool")
        .WithValue(40)
        .SubscribeToAfterAllBuildEvent(delegate(CardData data)
        {
            data.traits = new List<CardData.TraitStacks>
            {
                this.TStack("Barrage"),
                this.TStack("Consume"),
            };
            data.attackEffects = new[]
            {
                this.SStack("Increase Attack"),
                this.SStack("Increase Max Health")
            };
        }));
*/
        }

        private void Melinoe()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Melinoe", "Melinoë", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .SetStats(10, 5, 4)
                .SubscribeToAfterAllBuildEvent(delegate(CardData data) { }));
        }

        private void Ares()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Ares", "Ares", idleAnim: "FloatAnimationProfile")
                .SetSprites("Ares.png", "AresBG.png")
                .SetStats(4, 4, 5)
                .AddPool("GeneralUnitPool")
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
                        this.TStack("Longshot") // teeth?
                    };
                }));
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
                .SetSprites("Artemis.png", "ArtemisBG.png")
                .SetStats(4, 4, 5)
                .AddPool("GeneralUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate(CardData data)
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
        
        private void Athena()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Athena", "Athena")
                .SetSprites("Athena.png", "AthenaBG.png")
                .SetStats(5, 1, 6)
                .AddPool("GeneralUnitPool")
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
                    };
                    data.startWithEffects = new[]
                    {
                        this.SStack("Block")
                    };
                }));
        }

        private void Aphrodite()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Aphrodite", "Aphrodite", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .SetStats(4, 0, 5)
                .AddPool("GeneralUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "How lovely running into you again, gorgeous!",
                        "So good running into you again, sweetness!",
                        "Whoever's on your bad side, love, they're on mine as well.",
                        "Is not the purest act of love to aid somebody in their time of need?",
                        "I'd love to help you however best I'm able, my little godling.",
                        "You seem like you could use a helping hand, love."
                    };
                    data.attackEffects = new[]
                    {
                        this.SStack("Frost", 3),
                        // this.SStack() Apply haze if attack greater than?
                    };
                }));
        }

        private void Apollo()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Apollo", "Apollo", idleAnim: "FloatAnimationProfile")
                .SetSprites("Apollo.png", "ApolloBG.png")
                .SetStats(4, 0, 5)
                .AddPool("GeneralUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "Let's show them a dazzling display they won't soon forget.",
                        "Brighter days are ahead, just have to get through this rough patch.",
                        "What can we do but try and make these dark days a bit brighter, Cousin?",
                        "Peace will come again eventually, but for now, we have a reputation to uphold.",
                        "Must be something I can do to brighten up your evening there a bit?",
                        "Where there's light, there's hope, sunshine, so get set for some of each!"
                    };
                    data.attackEffects = new[]
                    {
                        this.SStack("Frost", 3), // counter?? //multiple targets?
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
                    .FreeModify(delegate (StatusEffectChangeTargetMode data)
                    {
                        var targetMode = ScriptableObject.CreateInstance<TargetModeStatus>();
                        targetMode.targetType = "snow";
                        data.targetMode = targetMode;
                        data.targetMode = targetMode;
                    })
            );

            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Demeter", "Demeter", idleAnim: "PingAnimationProfile")
                .SetSprites("Demeter.png", "DemeterBG.png")
                .SetStats(8, 2, 3)
                .AddPool("GeneralUnitPool")
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
                        this.SStack("ImmuneToSnow"),
                        this.SStack("When Hit Apply Snow To Attacker", 2),
                        this.SStack("Hits All Snowed Enemies")
                    };
                }));
        }

        private void Dionysus()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Dionysus", "Dionysus", idleAnim: "PingAnimationProfile")
                .SetSprites("Dionysus.png", "Dionysus.png")
                .SetStats(8, 2, 3)
                .AddPool("GeneralUnitPool")
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
                        this.SStack("ImmuneToSnow"), // shroom? aimless?
                    };
                }));
        }
        
        private void Hera()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Hera", "Hera")
                .SetSprites("Hera.png", "HeraBG.png")
                .SetStats(6, 1, 4)
                .AddPool("GeneralUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "Nothing like jointly annihilating our enemies to bring the family closer together, hm?",
                        "Aunt Hera's back, my dear, so let's have ourselves an interesting time!",
                        "Our family may be cursed by the Fates themselves, but we do know how to fight.",
                        "We on Olympus take our vengeance very personally.",
                        "I've enough power vested in me that I'm pleased to spare you some of it for now.",
                    };
                    data.attackEffects = new[]
                    {
                        this.SStack("Weakness") // TODO: Hitch
                    };
                }));
        }

        private void Hermes()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Hermes", "Hermes")
                .SetSprites("Hermes.png", "HermesBG.png")
                .SetStats(6, 1, 2)
                .AddPool("GeneralUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "In a bit of a rush, here, so let's get moving, hey?",
                        "Lots going on! Been running around all night!",
                        "Live fast or not at all, right? Now take your pick!",
                        "No calamitous attack on our family is going to slow us down!"
                    };
                    data.attackEffects = new[]
                    {
                        this.SStack("Weakness") // TODO: Some counter/speed thing
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
                .AddPool("GeneralUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate(CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "This old flame can never be put out, and don't you forget it, dearie!",
                        "Is something burning over there, dearie? Well it's about to be!",
                        "Been far too long since we incinerated stuff together, hasn't it?",
                        "Ah, let's burn it all down, what say you, dearie?",
                        "Sacrifices sometimes must be made, dearie. So let's round them up and put them to the flame!",
                    };
                    data.startWithEffects = new[]
                    {
                        this.SStack("When Enemy Is Hit By Item Apply Overburn To Them")
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
                .AddPool("GeneralUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate(CardData data)
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
                        this.SStack("On Turn Add Damage to Items In Hand")
                    };
                }));
        }

        private void Poseidon()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Poseidon", "Poseidon")
                .SetSprites("Poseidon.png", "PoseidonBG.png")
                .SetStats(6, 3, 4)
                .AddPool("GeneralUnitPool")
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
                    data.attackEffects = new[]
                    {
                        this.SStack("Weakness") // TODO: Knock back?
                    };
                }));
        }

        private void Zeus()
        {
            cards.Add(new CardDataBuilder(this)
                .CreateUnit("Zeus", "Zeus")
                .SetSprites("Zeus.png", "ZeusBG.png")
                .SetStats(6, 1, 4)
                .AddPool("GeneralUnitPool")
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.greetMessages = new[]
                    {
                        "Our enemies will soon be in for quite the shock, I think!",
                        "I trust we'll have all this unpleasantness behind us in no time, hm?",
                        "All those who stand against Olympus shall be summarily struck down.",
                    };
                    data.attackEffects = new[]
                    {
                        this.SStack("Jolted") // TODO: Jolted
                    };
                }));
        }

        public override void Load()
        {
            if (!preLoaded)
            {
                // this.LoadPanels();
                CreateModAssets();
            }

            base.Load();
        }

        public override void Unload()
        {
            preLoaded = false;
            // TODO
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