using Deadpan.Enums.Engine.Components.Modding;
using System.Linq;
using static Console;

namespace HadesFrost
{
    public static class Boons
    {
        public static void Setup(HadesFrost mod)
        {
            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusEffectApplyXWhenDeployed>("Reduce Counter When Deployed")
                .WithCanBeBoosted(true)
                .WithText("When deployed, count down <keyword=counter> by <{a}>")
                .FreeModify(delegate (StatusEffectApplyXWhenDeployed data)
                {
                    data.effectToApply = mod.TryGet<StatusEffectData>("Reduce Counter");
                    data.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                })
            );
        }

        public static void GiveBoons(HadesFrost mod, Entity entity)
        {
            if (entity == null)
            {
                return;
            }

            var hadesCards = References.PlayerData?.inventory?.deck?.Where(d =>
                d.traits?.Any(t => t.data.name == $"{mod.GUID}.Hades") ?? false);

            if (hadesCards == null)
            {
                return;
            }

            var cardName = entity.name.Replace($"{mod.GUID}.", "");
            
            CardUpgradeData upgrade = null;

            switch (cardName)
            {
                case "Aphrodite":
                    {
                        upgrade = mod.TryGet<CardUpgradeData>("CardUpgradeFrosthand");
                        break;
                    }
                case "Apollo":
                    {
                        // Or burst damage
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("AresBoon")
                            .SubscribeToAfterAllBuildEvent(data =>
                            {
                                data.effects = new[] { mod.SStack("Reduce Counter When Deployed", 2) };
                            });
                        break;
                    }
                case "Ares":
                    {
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("AresBoon")
                            .SetEffects(mod.SStack("On Kill Apply Attack To Self"));
                        break;
                    }
                case "Artemis":
                    {
                        upgrade = mod.TryGet<CardUpgradeData>("CardUpgradeDemonize"); // arrow charm?
                        break;
                    }
                case "Athena":
                    {
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("AthenaBoon")
                            .SetEffects(mod.SStack("When Hit Apply Shell To Self", 2));
                        break;
                    }
                case "Dionysus":
                {
                    upgrade = new CardUpgradeDataBuilder(mod)
                        .Create("AthenaBoon")
                        .SetEffects(mod.SStack("When Hit Apply Shell To Self", 2));
                    break;
                }
                case "Demeter":
                    {
                        upgrade = mod.TryGet<CardUpgradeData>("CardUpgradeSnowball");
                        break;
                    }
                case "Hephaestus":
                    {
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("HephaestusBoon")
                            .SetEffects(mod.SStack("Shell", 3));
                        break;
                    }
                case "Hera":
                    {
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("HeraBoon")
                            .SetAttackEffects(mod.SStack("Hitch"));
                        break;
                    }
                case "Hermes":
                    {
                        new CommandGainGold().Run("50");
                        break;
                    }
                case "Hestia":
                    {
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("HestiaBoon")
                            .SetAttackEffects(mod.SStack("Overload"));
                        break;
                    }
                case "Poseidon":
                    {
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("PoseidonBoon")
                            .ChangeHP(3)
                            .WithSetHP(false);
                        // smackback or more gold from bling cave?
                        break;
                    }
                case "Zeus":
                    {
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("ZeusBoon")
                            .SetAttackEffects(mod.SStack("Jolted"));
                        break;
                    }
                default:
                    return;
            }

            if (upgrade == null)
            {
                return;
            }

            // a tad hacky
            foreach (var hadesCard in hadesCards)
            {
                upgrade.GainEffects(hadesCard);
            }
        }

    }
}