using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;
using static CardData;

namespace ScissorsCharm
{
    public class ScissorsCharm : WildfrostMod
    {
        public ScissorsCharm(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.scissorscharm";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Scissors Charm";

        public override string Description => "Adds a new charm can remove other charms from a card";

        private List<CardUpgradeDataBuilder> cardUpgrades;
        private bool preLoaded;

        private void CreateModAssets()
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintMaxCounterMoreThan>();

            cardUpgrades = new List<CardUpgradeDataBuilder>
            {
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeScissors")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithImage("ScissorsCharm.png")
                    .WithTitle("Scissors Charm")
                    .WithText("Unequips all charms, except other Scissors Charms\nDoes not take up a charm slot")
                    .WithTier(2)
                    .SubscribeToAfterAllBuildEvent(delegate (CardUpgradeData data)
                    {
                        var script = ScriptableObject.CreateInstance<CardScriptRemoveCharms>();
                        data.takeSlot = false;
                        data.scripts = new CardScript[] { script };
                    })
            };

            preLoaded = true;
        }

        public class CardScriptRemoveCharms : CardScript
        {
            public override void Run(CardData target)
            {
                var charms = GameObject.Find("Charms");
                var holder = (CardCharmHolder)charms.GetComponent(typeof(CardCharmHolder));

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
                target.upgrades = target.upgrades.Where(t => t.type == CardUpgradeData.Type.Crown || 
                                                             t.name == "bethanw10.wildfrost.scissorscharm.CardUpgradeScissors").ToList();

                target.charmSlots = target.original.charmSlots;
                target.traits = target.original.traits;
                target.attackEffects = target.original.attackEffects;
                target.canBeHit = target.original.canBeHit;
                target.hasAttack = target.original.hasAttack;

                if (target.customData?.ContainsKey("extraCharmSlots") == true)
                {
                    target.customData.Remove("extraCharmSlots");
                }
            }
        }

        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }
            base.Load();
        }


        public override List<T> AddAssets<T, TY>()
        {
            var typeName = typeof(TY).Name;
            switch (typeName)
            {
                case nameof(CardUpgradeData):
                    return cardUpgrades.Cast<T>().ToList();
                default:
                    return null;
            }
        }
    }
}