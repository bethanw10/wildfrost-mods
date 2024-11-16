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

        public static void LeaderImagesFix(Entity entity)
        {
            if (entity.display is Card card && !card.hasScriptableImage)
            {
                card.mainImage.gameObject.SetActive(true);
            }
        }

        private static void Melinoe(HadesFrost mod)
        {
            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Melinoe", "Melinoë", idleAnim: "FloatAnimationProfile")
                .SetSprites("Melinoe.png", "MelinoeBG.png")
                .WithCardType("Leader")
                .SetStats(6, 4, 3)
                .FreeModify(data =>
                {
                    data.createScripts = new[]  
                    {
                        mod.GiveUpgrade(),
                        AddRandomHealth(0, 0),
                        AddRandomDamage(0, 0),
                        AddRandomCounter(0, 0)
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
                        AddRandomHealth(0,0),
                        AddRandomDamage(0,0),
                        AddRandomCounter(0,0)
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
            var keyword = new KeywordDataBuilder(mod)
                .Create("Hades")
                .WithCanStack(false)
                .WithDescription("Receive a boon when an Olympian God is added to your team" +
                                 "|<sprite name=Controller ButtonSheet_24> an Olympian to see their boon")
                .WithShowName(true)
                .WithShowIcon(false)
                .WithTitleColour(new Color(0.495f, 0.780f, 0.304f))
                .WithTitle("Child of Hades");

            mod.Keywords.Add(keyword);

            mod.Traits.Add(new TraitDataBuilder(mod)
                .Create("Hades")
                .WithKeyword(keyword.Build())
            );
        }

        private static CardScript GiveUpgrade(this WildfrostMod mod, string name = "Crown")
        {
            CardScriptGiveUpgrade script = ScriptableObject.CreateInstance<CardScriptGiveUpgrade>();
            script.name = $"Give {name}";
            script.upgradeData = mod.TryGet<CardUpgradeData>(name);
            return script;
        }

        private static CardScript AddRandomHealth(int min, int max)
        {
            CardScriptAddRandomHealth health = ScriptableObject.CreateInstance<CardScriptAddRandomHealth>();
            health.name = "Random Health";
            health.healthRange = new Vector2Int(min, max);
            return health;
        }

        private static CardScript AddRandomDamage(int min, int max) 
        {
            CardScriptAddRandomDamage damage = ScriptableObject.CreateInstance<CardScriptAddRandomDamage>();
            damage.name = "Give Damage";
            damage.damageRange = new Vector2Int(min, max);
            return damage;
        }

        private static CardScript AddRandomCounter(int min, int max) 
        {
            CardScriptAddRandomCounter counter = ScriptableObject.CreateInstance<CardScriptAddRandomCounter>();
            counter.name = "Give Counter";
            counter.counterRange = new Vector2Int(min, max);
            return counter;
        }
    }
}