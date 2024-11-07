using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Utils;
using UnityEngine;

namespace HadesFrost.Setup
{
    public static class Leaders
    {
        public static void Setup(HadesFrost mod)
        {
            ChildOfHades(mod);

            Melinoe(mod);
            // Zagreus(mod);
        }

        private static void Melinoe(HadesFrost mod)
        {
            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Melinoe", "Melinoë", idleAnim: "FloatAnimationProfile")
                .SetSprites("Melinoe.png", "MelinoeBG.png")
                .WithCardType("Leader")
                .SetStats(6, 3, 4)
                .FreeModify(data =>
                {
                    data.createScripts = new[]  
                    {
                        mod.GiveUpgrade(),
                        mod.AddRandomHealth(0, 0),
                        mod.AddRandomDamage(0, 0),
                        mod.AddRandomCounter(0, 0)
                    };
                })
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.traits = new List<CardData.TraitStacks>
                    {
                        mod.TStack("Hades")
                    };
                }));
        }

        private static void Zagreus(HadesFrost mod)
        {
            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Zagreus", "Zagreus", idleAnim: "FloatAnimationProfile")
                .SetSprites("Zagreus.png", "PiriBG.png")
                .WithCardType("Leader")
                .SetStats(10, 5, 4)
                .FreeModify(data =>
                {
                    data.createScripts = new[]
                    {
                        mod.GiveUpgrade(),
                        mod.AddRandomHealth(0,0),
                        mod.AddRandomDamage(0,0),
                        mod.AddRandomCounter(0,0)
                    };
                })
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.traits = new List<CardData.TraitStacks>
                    {
                        mod.TStack("Hades")
                    };
                }));
        }

        private static void ChildOfHades(HadesFrost mod)
        {
            var boons = new KeywordDataBuilder(mod)
                .Create("Hades")
                .WithCanStack(false)
                .WithDescription("Receive a boon when an Olympian God is added to your team" +
                                 "|<sprite name=Controller ButtonSheet_24> an Olympian to see their boon")
                .WithShowName(true)
                .WithShowIcon(false)
                .WithTitleColour(new Color(137, 168, 111))
                .WithTitle("Child of Hades");

            mod.Keywords.Add(boons);

            mod.Traits.Add(new TraitDataBuilder(mod)
                .Create("Hades")
                .WithKeyword(boons.Build())
            );
        }

        private static CardScript GiveUpgrade(this WildfrostMod mod, string name = "Crown")
        {
            CardScriptGiveUpgrade script = ScriptableObject.CreateInstance<CardScriptGiveUpgrade>(); //This is the standard way of creating a ScriptableObject
            script.name = $"Give {name}";                               //Name only appears in the Unity Inspector. It has no other relevance beyond that.
            script.upgradeData = mod.TryGet<CardUpgradeData>(name);
            return script;
        }

        private static CardScript AddRandomHealth(this WildfrostMod mod, int min, int max)
        {
            CardScriptAddRandomHealth health = ScriptableObject.CreateInstance<CardScriptAddRandomHealth>();
            health.name = "Random Health";
            health.healthRange = new Vector2Int(min, max);
            return health;
        }

        private static CardScript AddRandomDamage(this WildfrostMod mod, int min, int max) 
        {
            CardScriptAddRandomDamage damage = ScriptableObject.CreateInstance<CardScriptAddRandomDamage>();
            damage.name = "Give Damage";
            damage.damageRange = new Vector2Int(min, max);
            return damage;
        }

        private static CardScript AddRandomCounter(this WildfrostMod mod, int min, int max) 
        {
            CardScriptAddRandomCounter counter = ScriptableObject.CreateInstance<CardScriptAddRandomCounter>();
            counter.name = "Give Counter";
            counter.counterRange = new Vector2Int(min, max);
            return counter;
        }
    }
}