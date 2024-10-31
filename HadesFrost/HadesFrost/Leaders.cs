using Deadpan.Enums.Engine.Components.Modding;
using System.Collections.Generic;
using HadesFrost.Extensions;
using UnityEngine;

namespace HadesFrost
{
    public static class Leaders
    {
        public static void Setup(HadesFrost mod)
        {
            Melinoe(mod);
        }

        private static void Melinoe(HadesFrost mod)
        {
            var boons = new KeywordDataBuilder(mod)
                .Create("Hades")
                .WithCanStack(false)
                //.WithDescription("Gain a buff when an Olympian God is added to your team|Inspect a god tp see their boon")
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

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Melinoe", "Melinoë", idleAnim: "FloatAnimationProfile")
                .SetSprites("Piri.png", "PiriBG.png")
                .SetStats(10, 5, 4)
                .SetTraits()
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    data.traits = new List<CardData.TraitStacks>
                    {
                        mod.TStack("Hades")
                    };
                }));
        }
    }
}