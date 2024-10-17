using System.Linq;
using UnityEngine;

namespace AllCharms.Charms
{
    public class CardScriptRemoveCharms : CardScript
    {
        public override void Run(CardData target)
        {
            var deck = GameObject.Find("DeckDisplay");
            var holder = deck.GetComponentInChildren<CardCharmHolder>();

            // Debug.Log("bethan " + test);
            //
            // var charms = GameObject.Find("Charms");
            // var holder = (CardCharmHolder)charms?.GetComponent(typeof(CardCharmHolder));

            if (holder == null)
            {
                return;
            }

            foreach (var cardUpgradeData in target.upgrades.Where(t =>
                         t.type != CardUpgradeData.Type.Crown &&
                         t.name != "bethanw10.wildfrost.scissorscharm.CardUpgradeScissors"))
            {
                // Lumin charm and cake charm do not clone before updating
                if (cardUpgradeData.name == "CardUpgradeBoost")
                {
                    foreach (var targetAttackEffect in target.attackEffects)
                    {
                        targetAttackEffect.count -= 1;
                    }
                }

                if (cardUpgradeData.name == "CardUpgradeCake")
                {
                    foreach (var targetAttackEffect in target.attackEffects)
                    {
                        targetAttackEffect.count -= 4;
                    }
                }

                if (cardUpgradeData.name == "CardUpgradeReduceEffects")
                {
                    foreach (var targetAttackEffect in target.attackEffects)
                    {
                        targetAttackEffect.count += 1;
                    }
                }

                var upgrade = cardUpgradeData.Clone();

                holder.Create(upgrade);

                References.PlayerData.inventory.upgrades.Add(upgrade);
                Events.InvokeUpgradeGained(upgrade);
            }

            holder.SetPositions();

            target.hp = target.original.hp;
            target.damage = target.original.damage;
            target.counter = target.original.counter;

            target.startWithEffects = target.original.startWithEffects;


            // Noomlin bell
            if (target.traits.Any(t => t.data.name == "Noomlin") && 
                !target.upgrades.Any(t => t.name == "CardUpgradeNoomlin") &&
                !target.original.traits.Any(t => t.data.name == "Noomlin"))
            {
                var noomlin = target.traits.FirstOrDefault(t => t.data.name == "Noomlin");
                target.traits = target.original.traits;
                target.traits.Add(noomlin);
            }
            else
            {
                target.traits = target.original.traits;
            }


            target.upgrades = target.upgrades.Where(t => t.type == CardUpgradeData.Type.Crown ||
                                                         t.name == "bethanw10.wildfrost.scissorscharm.CardUpgradeScissors").ToList();

            target.charmSlots = target.original.charmSlots;

            target.attackEffects = target.original.attackEffects;
            target.canBeHit = target.original.canBeHit;
            target.hasAttack = target.original.hasAttack;

            if (target.customData?.ContainsKey("extraCharmSlots") == true)
            {
                target.customData.Remove("extraCharmSlots");
            }
        }
    }
}