using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;
using WildfrostHopeMod;
using WildfrostHopeMod.Configs;


namespace Piri
{
    public class Piri : WildfrostMod
    {
        public Piri(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.piri";

        public override string[] Depends => new[] { "hope.wildfrost.configs" };

        public override string Title => "Piri";

        public override string Description => @"Adds a new spice based pet

With the ConfigManager Mod, you can change the card between:

Apply 2 spice to all allies 
Stats: 4 | 2 | 5

Apply 2 spice to allies in row 
Stats: 4 | 2 | 4

When an ally is debuffed, apply double spice 
Stats: 4 | 2 | 4

When an ally is healed, apply equal spice 
Stats: 3 | 2 | 4

On kill, gain spice equal to target's attack 
Stats: 3 | 4 | 4";

        private readonly List<CardDataBuilder> cards = new List<CardDataBuilder>();
        private readonly List<StatusEffectDataBuilder> statusEffects = new List<StatusEffectDataBuilder>();
        private bool preLoaded;

        [ConfigItem(Variant.ApplyToAll, "", "Piri")]
        [ConfigManagerTitle("Change Stats/Ability")]
        [ConfigManagerDesc("Requires game restart and new run\nStats are in the format\n <sprite=health> | <sprite=attack> | <sprite=counter>")]
        [ConfigOptions(
        new[]
        {
            "Apply 2 spice to all allies\nStats: 4 | 2 | 5",
            "Apply 2 spice to allies in row\nStats: 4 | 2 | 4",
            "When an ally is debuffed, apply double spice\nStats: 4 | 2 | 4",
            "When an ally is healed, apply equal spice\nStats: 3 | 2 | 4",
            "On kill, gain spice equal to target's attack\nStats: 3 | 4 | 4",
        }, 
        new[]
        {
            Variant.ApplyToAll, 
            Variant.ApplyToRow,
            Variant.Debuff,
            Variant.Healed,
            Variant.OnKillGainAttackAsSpice
        })]
        public Variant ConfiguredVariant;

        private void CreateModAssets()
        {
            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXOnTurn>("On Turn Apply Spice To Allies In Row")
                    .WithCanBeBoosted(true)
                    .WithText("Apply <{0}><keyword=spice> to allies in the row")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXOnTurn data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.AlliesInRow;
                        data.effectToApply = Get<StatusEffectData>("Spice").InstantiateKeepName();
                        data.canBeBoosted = true;

                        data.desc = "Apply <{0}><keyword=spice> to self and allies in the row";
                        data.textInsert = "{a}";
                    })
            );

            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXOnTurn>("On Turn Apply Shell To Allies In Row")
                    .WithCanBeBoosted(true)
                    .WithText("Apply <{0}><keyword=shell> to allies in the row")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXOnTurn data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.AlliesInRow;
                        data.effectToApply = Get<StatusEffectData>("Shell").InstantiateKeepName();
                        data.canBeBoosted = true;

                        data.desc = "Apply <{0}><keyword=shell> to self and allies in the row";
                        data.textInsert = "{a}";
                    })
            );

            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenAllyOrSelfDebuffed>("On Debuff Apply Double Spice")
                    .WithText("When self or an ally is debuffed, apply double <keyword=spice>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXWhenAllyOrSelfDebuffed data)
                    {
                        data.count = 0;
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Target;
                        data.effectToApply = Get<StatusEffectData>("Spice").InstantiateKeepName();
                        data.queue = true;
                        data.eventPriority = 0;
                        data.applyEqualAmount = true;
                        data.equalAmountBonusMult = 1f;
                    })
            );

            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenAllyOrSelfDebuffed>("On Debuff Apply Double Shell")
                    .WithText("When self or an ally is debuffed, apply double <keyword=shell>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXWhenAllyOrSelfDebuffed data)
                    {
                        data.count = 0;
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Target;
                        data.effectToApply = Get<StatusEffectData>("Shell").InstantiateKeepName();
                        data.queue = true;
                        data.eventPriority = 0;
                        data.applyEqualAmount = true;
                        data.equalAmountBonusMult = 1f;
                    })
            );

            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXOnKillTargetContext>("On Kill Apply Spice Equal To Attack")
                    .WithText("On kill, gain <keyword=spice> equal to target's <keyword=attack>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXOnKillTargetContext data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        data.effectToApply = Get<StatusEffectData>("Spice").InstantiateKeepName();
                        data.queue = true;
                        data.eventPriority = 0;
                        data.applyEqualAmount = true;
                        data.contextEqualAmount = new ScriptableCurrentAttack();
                    })
            );

            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXOnKillTargetContext>("On Kill Apply Shell Equal To Attack")
                    .WithText("On kill, gain <keyword=shell> equal to target's <keyword=attack>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXOnKillTargetContext data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        data.effectToApply = Get<StatusEffectData>("Shell").InstantiateKeepName();
                        data.queue = true;
                        data.eventPriority = 0;
                        data.applyEqualAmount = true;
                        data.contextEqualAmount = new ScriptableCurrentAttack();
                    })
            );

            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenAllyHealed>("When Ally Is Healed Apply Equal Spice")
                    .WithText("When an ally is healed, apply equal <keyword=spice>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXWhenAllyHealed data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Target;
                        data.effectToApply = Get<StatusEffectData>("Spice").InstantiateKeepName();
                        data.queue = true;
                        data.applyEqualAmount = true;
                    })
            );

            statusEffects.Add(
                new StatusEffectDataBuilder(this)
                    .Create<StatusEffectApplyXWhenAllyHealed>("When Ally Is Healed Apply Equal Shell")
                    .WithText("When an ally is healed, apply equal <keyword=shell>")
                    .WithType("")
                    .FreeModify(delegate (StatusEffectApplyXWhenAllyHealed data)
                    {
                        data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Target;
                        data.effectToApply = Get<StatusEffectData>("Shell").InstantiateKeepName();
                        data.queue = true;
                        data.applyEqualAmount = true;
                    })
            );

            var card = new CardDataBuilder(this)
                .CreateUnit("piri", "Piri", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .IsPet((ChallengeData)null);

            switch (ConfiguredVariant)
            {
                case Variant.ApplyToRow:
                    card.SetStats(4, 2, 4)
                        .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                        {
                            data.startWithEffects = new[]
                            {
                                SStack("On Turn Apply Spice To Allies In Row", 2)
                            };
                        });

                    break;

                case Variant.Debuff:
                    card.SetStats(4, 2, 4)
                        .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                        {
                            data.startWithEffects = new[]
                            {
                                SStack("On Debuff Apply Double Spice")
                            };
                        });

                    break;
                
                case Variant.Healed:
                    card.SetStats(3, 2, 4)
                        .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                        {
                            data.startWithEffects = new[]
                            {
                                SStack("When Ally Is Healed Apply Equal Spice")
                            };
                        });

                    break;


                case Variant.OnKillGainAttackAsSpice:
                    card.SetStats(3, 4, 4)
                        .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                        {
                            data.startWithEffects = new[]
                            {
                                SStack("On Kill Apply Spice Equal To Attack")
                            };
                        });
                    break;

                case Variant.ApplyToAll:
                default:
                    card.SetStats(4, 2, 5)
                        .SetStartWithEffect(SStack("On Turn Apply Spice To Allies", 2));

                    break;
            }

            cards.Add(card);

            preLoaded = true;
        }

        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }

            ConfigManager.OnModLoaded += HandleModLoaded;

            base.Load();
        }

        public override void Unload()
        {
            preLoaded = false;

            base.Unload();
        }

        private void HandleModLoaded(WildfrostMod mod)
        {
            if (mod.GUID != "hope.wildfrost.configs")
            {
                return;
            }

            var section = ConfigManager.GetConfigSection(this);
            if (section != null)
            {
                section.OnConfigChanged += HandleConfigChange;
            }
        }

        private void HandleConfigChange(ConfigItem item, object value)
        {
            Debug.Log("[PIRI] config changed!!");
        }

        public override List<T> AddAssets<T, Y>()           //This method is called 6-7 times in base.Load() for each Builder. Can you name them all?
        {
            var typeName = typeof(Y).Name;
            switch (typeName)                                //Checks what the current builder is
            {
                case nameof(CardData):
                    return cards.Cast<T>().ToList();         //Loads our cards
                case nameof(StatusEffectData):
                    return statusEffects.Cast<T>().ToList(); //Loads our status effects
                default:
                    return null;
            }
        }

        private CardData.StatusEffectStacks SStack(string name, int amount = 1) => new CardData.StatusEffectStacks(Get<StatusEffectData>(name), amount);
    }
}