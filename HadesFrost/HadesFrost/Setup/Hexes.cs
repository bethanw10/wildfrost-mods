using System;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.ButtonStatuses;
using HadesFrost.Utils;
using UnityEngine;

namespace HadesFrost.Setup
{
    public static class Hexes
    {
        public static void Setup(HadesFrost mod)
        {
            LunarRay(mod);
        }

        private static void LunarRay(HadesFrost mod)
        {
            var focusEnergy =
                new KeywordDataBuilder(mod)
                    .Create($"focusenergy")
                    .WithShowName(true)
                    .WithShowIcon(false)
                    .WithTitle("Focus Energy")
                    .WithCanStack(false)
                    .WithPanelColour(Color.grey)
                    .WithBodyColour(new Color(22, 28, 21))
                    .WithTitleColour(Color.green)
                    .WithDescription("<Free Action>: Discard the rightmost card in hand|Click to activate\nOnce per turn");

            //mod.CreateIconKeyword("focusenergy", "Focus Energy", "<Free Action>: Discard the rightmost card in hand|Click to activate\nOnce per turn");

            mod.CreateButtonIcon(
                "kingdraFocusEnergy", 
                mod.ImagePath("kingdrabutton.png").ToSprite(), 
                "focusEnergy", 
                "", 
                Color.white, 
                new[] { focusEnergy.Build() });

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>("Frenzy Rightmost Button")
                .WithType("focusEnergy")
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.RightCardInHand;
                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.oncePerTurn = true;
                    castData.effectToApply = mod.TryGet<StatusEffectData>("Instant Apply Frenzy (To Item In Hand)");
                })
            );

            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateItem("LunarRay", "Lunar Ray")
                    .SetSprites("LunarRay.png", "LunarRayBG.png")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .SetStats()
                    .WithText("Your Hex fires a beam that deals up to 800 damage over 2 Sec.")
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                    }));
        }

        public static void GiveHex(HadesFrost mod, Entity entity)
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

            var cardName = entity.name.Replace($"{mod.GUID}.", "");

            CardUpgradeData upgrade;

            switch (cardName)
            {
                case "LunarRay":
                    {
                        upgrade = new CardUpgradeDataBuilder(mod)
                            .Create("LunarRay")
                            .SetEffects(mod.SStack("Increase Attack While Undamaged", 2));

                        Debug.Log(upgrade);
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
            upgrade.GainEffects(leader);
        }
    }
}