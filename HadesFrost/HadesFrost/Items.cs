using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;

namespace HadesFrost
{
    public static class Items
    {
        public static void Setup(HadesFrost mod)
        {
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("Pom of Power", "Pom of Power")
                    .SetSprites("PomOfPower.png", "AresBG.png")
                    .WithText("Boost the target's effects by 1")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .AddPool("GeneralItemPool")
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Barrage"), mod.TStack("Consume"),
                        };
                        data.startWithEffects = new[]
                        {
                            mod.SStack("Instant Ongoing Increase Effects"),
                        };

                        var crown = mod.TryGet<CardUpgradeData>("Crown");
                        crown.canBeRemoved = false;
                        data.upgrades = new List<CardUpgradeData>
                        {
                            crown
                        };
                    }));

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("Pom Slice", "Pom Slice")
                    .SetSprites("PomOfPower.png", "AresBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .AddPool("GeneralItemPool")
                    .WithValue(40)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.traits = new List<CardData.TraitStacks>
                        {
                            mod.TStack("Consume"),
                        };
                        data.startWithEffects = new[]
                        {
                            mod.SStack("On Card Played Boost To RandomAlly"),
                        };
                    }));

            /*
cards.Add(
    new CardDataBuilder(mod)
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
                mod.TStack("Barrage"),
                mod.TStack("Consume"),
            };
            data.attackEffects = new[]
            {
                mod.SStack("Increase Attack"),
                mod.SStack("Increase Max Health")
            };
        }));
*/
        }
    }
}