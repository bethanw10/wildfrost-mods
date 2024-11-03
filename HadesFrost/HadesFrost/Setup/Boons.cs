using System;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Statuses;
using HadesFrost.Utils;
using UnityEngine;
using static Console;

namespace HadesFrost.Setup
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

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusEffectIncreaseAttackWhileNotDamaged>("Increase Attack While Undamaged")
                .WithCanBeBoosted(true)
                .WithText("While undamaged, <keyword=attack> is increased by <{a}>")
                .FreeModify(delegate (StatusEffectIncreaseAttackWhileNotDamaged data)
                {
                    data.effectToGain = mod.TryGet<StatusEffectData>("Ongoing Increase Attack");
                })
            );
        }

        public static void GiveBoons(HadesFrost mod, Entity entity)
        {
            CardData leader;
            try
            {
                // No leader data when picking pet
                leader = References.LeaderData;
            }
            catch (Exception)
            {
                return;
            }
            
            if (entity == null || leader == null)
            {
                return;
            }

            // var hadesCards = References.PlayerData?.inventory?.deck?.Where(d =>
            //     d.traits?.Any(t => t.data.name == $"{mod.GUID}.Hades") ?? false);

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
                        // more damage when undamaged
                        // upgrade = new CardUpgradeDataBuilder(mod)
                        //     .Create("AresBoon")
                        //     .WithText("When deployed, reduce <keyword=counter> by <{a}>")
                        //     .SubscribeToAfterAllBuildEvent(data =>
                        //     {
                        //         data.effects = new[] { mod.SStack("Reduce Counter When Deployed", 2) };
                        //     });

                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("ApolloBoon")
                            .SetEffects(mod.SStack("Increase Attack While Undamaged", 2));

                        Debug.Log(upgrade);
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
                        upgrade = mod.TryGet<CardUpgradeData>("CardUpgradeDemonize"); // gain arrow charm?
                        break;
                    }
                case "Athena":
                    {
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("AthenaBoon")
                            .SetEffects(mod.SStack("On Turn Apply Shell To Self", 2));
                        break;
                    }
                case "Dionysus":
                    {
                        var nectar = mod.TryGet<CardData>("Nectar");
                        nectar.traits.Add(mod.TStack("Noomlin"));
                        References.Player.data.inventory.deck.Add(nectar);
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
                        new CommandGainGold().Run("75");
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
                        // deal more damage to enemies with debuff
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
            Debug.Log("applying to leader");
            upgrade.GainEffects(leader);
        }
    }
}