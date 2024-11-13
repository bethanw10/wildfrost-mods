using System;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.ButtonStatuses;
using HadesFrost.StatusEffects;
using HadesFrost.Utils;
using UnityEngine;

namespace HadesFrost.Setup
{
    public static class Hexes
    {
        public static void Setup(HadesFrost mod)
        {
            HexKeyword(mod);

            Magick(mod);

            LunarRay(mod);
            PhaseShift(mod);
            MoonWater(mod);
            WolfHowl(mod);
            DarkSide(mod);
            TwilightCurse(mod);
            NightBloom(mod);
            TotalEclipse(mod);
            SkyFall(mod);
        }

        private static void HexKeyword(HadesFrost mod)
        {
            mod.Keywords.Add(
                new KeywordDataBuilder(mod)
                    .Create("hex")
                    .WithShowName(true)
                    .WithShowIcon(false)
                    .WithTitle("Hex")
                    .WithCanStack(false)
                    .WithPanelColour(Color.grey)
                    .WithBodyColour(new Color(22, 28, 21))
                    .WithTitleColour(new Color(0.486f, 0.610f, 0.900f))
                    .WithDescription(
                        "Hexes are abilities that require <sprite name=magickicon>\n\n" +
                        "When a hex is equipped, gain <sprite name=magickicon> equal to all damage dealt by your team\n\n" +
                        "Hexes can be activated by clicking their icon"));
        }

        private static void Magick(HadesFrost mod)
        {
            mod.CreateIconKeyword("magick", "Magick", "A resource used for activating <Hexes>", "magickicon")
                .ChangeColor(note: new Color(0.98f, 0.89f, 0.61f));

            StatusIcons.CreateIcon(
                "magickicon",
                mod.ImagePath("magickicon.png").ToSprite(),
                "magick",
                "counter",
                Color.white,
                Color.clear,
                new[] { mod.TryGet<KeywordData>("magick") }, 
                1,
                0.008f);

            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXWhenEnemyTakesDamage>("Gain Magick Equal To Team Damage Dealt")
                    .WithCanBeBoosted(false)
                    .WithType("")
                    .WithStackable(false)
                    .WithText("Gain <keyword=magick> equal to damage dealt to enemy")
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        var castData = (StatusEffectApplyXWhenEnemyTakesDamage)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Magick");
                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        castData.applyEqualAmount = true;
                        castData.doPing = false;
                        castData.AllTypes = true;
                        castData.IgnoreType = "hex";
                    })
            );
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectNothing>("Magick")
                    .WithType("magick")
                    .WithKeyword("magick")
                    .WithIsKeyword(true)
                    .WithVisible(true)
                    .WithCanBeBoosted(false)
                    .WithOffensive(false)
                    .WithIconGroupName("counter")
                    .WithTextInsert("{a}")
                    .WithStackable(true)
                    .FreeModify(delegate (StatusEffectNothing data)
                    {
                        data.removeOnDiscard = false;
                        data.applyFormatKey = mod.TryGet<StatusEffectData>("Shroom").applyFormatKey;
                        data.targetConstraints = new TargetConstraint[] { ScriptableObject.CreateInstance<TargetConstraintIsUnit>() };
                    })
            );
        }

        private static void LunarRay(HadesFrost mod)
        {
            const int cost = 30;
            var description = "Deal <5> damage to enemies in the row\n" + $"<Cost: {cost}> <sprite name=magickicon>";

            const string name = "Lunar Ray";
            var keywordName = name.ToLower().Replace(" ", "");

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>(name.Replace(" ", "") + " Hex")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.EnemiesInRow;
                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.HitDamage = 5; // or double current attack?
                    castData.MagickCost = cost;
                })
            );
        }

        private static void PhaseShift(HadesFrost mod)
        {
            const int cost = 35;
            var description = "Increase all enemies' <keyword=counter> by <1>\n" + $"<Cost: {cost}> <sprite name=magickicon>";

            const string name = "Phase Shift";
            var keywordName = name.ToLower().Replace(" ", "");

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>(name.Replace(" ", "") + " Hex")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Enemies;
                    castData.effectToApply = mod.TryGet<StatusEffectData>("Increase Max Counter");
                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.MagickCost = cost;
                })
            );
        }

        private static void MoonWater(HadesFrost mod)
        {
            const int cost = 20;
            var description = "Restore <3><keyword=health>\n" + $"<Cost: {cost}> <sprite name=magickicon>";

            const string name = "Moon Water";
            var keywordName = name.ToLower().Replace(" ", "");

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>(name.Replace(" ", "") + " Hex")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                    castData.effectToApply = mod.TryGet<StatusEffectData>("Heal");
                    castData.FixedAmount = 3;

                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.MagickCost = cost;
                })
            );
        }

        private static void WolfHowl(HadesFrost mod)
        {
            const int cost = 30;

            var description = "Deal <3> damage to all enemies\n" + $"<Cost: {cost}> <sprite name=magickicon>";
            const string name = "Wolf Howl";
            var keywordName = name.ToLower().Replace(" ", "");

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>(name.Replace(" ", "") + " Hex")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Enemies;
                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.HitDamage = 3; // double current attack?
                    castData.MagickCost = cost;
                })
            );
        }

        private static void TwilightCurse(HadesFrost mod)
        {
            const int cost = 40;
            var description = "Turn a random non-boss enemy into a <card=Popper>\n" + $"<Cost: {cost}> <sprite name=magickicon>";
            const string name = "Twilight Curse";
            var keywordName = name.ToLower().Replace(" ", "");

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusEffectInstantMorphCard>("Morph Sheepopper")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusEffectInstantMorphCard)data;
                    castData.ResultingCardName = "Popper";
                    castData.SpawnOnBoard = true;
                    castData.CheckHand = false;
                    castData.CheckDeck = false;

                    var bossTarget = ScriptableObject.CreateInstance<TargetConstraintIsCardType>();
                    bossTarget.allowedTypes = new[] { mod.TryGet<CardType>("Enemy") };

                    var sheepopper = ScriptableObject.CreateInstance<TargetConstraintIsSpecificCard>();
                    sheepopper.not = true;
                    sheepopper.allowedCards = new[] { mod.TryGet<CardData>("Popper") };

                    castData.targetConstraints = new TargetConstraint[] { bossTarget, sheepopper };
                })
            );

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>(name.Replace(" ", "") + " Hex")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomEnemy;
                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.MagickCost = cost;
                    castData.effectToApply = mod.TryGet<StatusEffectData>("Morph Sheepopper");
                })
            );
        }

        private static void DarkSide(HadesFrost mod)
        {
            const int cost = 35;
            var description = $"Gain <1><keyword=block>\nGain <+1><keyword=attack>\n<Cost: {cost}> <sprite name=magickicon>";
            const string name = "Dark Side";
            var keywordName = name.ToLower().Replace(" ", "");

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusEffectInstantApplyEffect>("Apply Block")
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusEffectInstantApplyEffect)data;
                    castData.effectToApply = mod.TryGet<StatusEffectData>("Block");
                })
            );

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusEffectInstantMultiple>("Block And Increase Damage")
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusEffectInstantMultiple)data;
                    castData.effects = new[]
                    {
                        mod.TryGet<StatusEffectInstant>("Increase Attack"),
                        mod.TryGet<StatusEffectInstant>("Apply Block")
                    };
                })
            );

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>(name.Replace(" ", "") + " Hex")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                    castData.effectToApply = mod.TryGet<StatusEffectData>("Block And Increase Damage");

                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.MagickCost = cost;
                })
            );
        }

        private static void NightBloom(HadesFrost mod)
        {
            const int cost = 35;
            var description = "Apply <1> <keyword=haze> to front enemy\n" + $"<Cost: {cost}> <sprite name=magickicon>";
            const string name = "Night Bloom";
            var keywordName = name.ToLower().Replace(" ", "");

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>(name.Replace(" ", "") + " Hex")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.FrontEnemy;
                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.effectToApply = mod.TryGet<StatusEffectData>("Haze");
                    castData.MagickCost = cost;
                })
            );
        }

        private static void TotalEclipse(HadesFrost mod)
        {
            const int cost = 25;
            var description = $"Deal <8> damage to front enemy\n<Cost: {cost}> <sprite name=magickicon>";
            const string name = "Total Eclipse";
            var keywordName = name.ToLower().Replace(" ", "");

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>(name.Replace(" ", "") + " Hex")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.FrontEnemy;
                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.HitDamage = 8;
                    castData.MagickCost = cost;
                })
            );
        }

        private static void SkyFall(HadesFrost mod)
        {
            const int cost = 20;
            var description = $"Apply <1> <keyword=weakness> to all enemies\n<Cost: {cost}> <sprite name=magickicon>";
            const string name = "Sky Fall";
            var keywordName = name.ToLower().Replace(" ", "");

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects.Add(new StatusEffectDataBuilder(mod)
                .Create<StatusHexApplyX>(name.Replace(" ", "") + " Hex")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusHexApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Enemies;
                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.effectToApply = mod.TryGet<StatusEffectData>("Weakness");
                    castData.MagickCost = cost;
                })
            );
        }

        private static void CreateCard(HadesFrost mod, string name, string description)
        {
            var nameNoSpaces = name.Replace(" ", "");
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateUnit(nameNoSpaces, name, idleAnim: "PulseAnimationProfile")
                    .WithCardType("Summoned")
                    .SetSprites(nameNoSpaces.ToLower() + ".png", "HexBG.png")
                    .SetStats()
                    .WithText($"Leader gains <keyword={Extensions.PrefixGUID("hex", mod)}>:\n" + description));
        }

        private static void CreateKeyword(WildfrostMod mod, string keywordName, string name, string description)
        {
            var keyword = new KeywordDataBuilder(mod)
                .Create(keywordName)
                .WithShowName(true)
                .WithShowIcon(false)
                .WithTitle(name)
                .WithCanStack(false)
                .WithPanelColour(Color.grey)
                .WithBodyColour(new Color(22, 28, 21))
                .WithTitleColour(new Color(0.486f, 0.610f, 0.900f))
                .WithDescription(description + "|Click to activate");

            StatusIcons.CreateButtonIcon(
                name,
                mod.ImagePath(keywordName + "button.png").ToSprite(),
                keywordName,
                "",
                Color.white,
                new[] { keyword.Build() });
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

            CardData.StatusEffectStacks statusEffect;

            if (cardName == "LunarRay" || cardName == "PhaseShift" || cardName == "MoonWater" ||
                cardName == "WolfHowl" || cardName == "DarkSide" || cardName == "TwilightCurse" ||
                cardName == "NightBloom" || cardName == "TotalEclipse" || cardName == "SkyFall")
            {
                statusEffect = mod.SStack(cardName + " Hex");
            }
            else
            {
                return;
            }

            var upgrade = new CardUpgradeDataBuilder(mod)
                .Create("Hex")
                .SetEffects(
                    statusEffect,
                    mod.SStack("Gain Magick Equal To Team Damage Dealt"))
                .Build();

            References.Player.data.inventory.deck.Remove(entity.data);

            upgrade.GainEffects(leader);
        }
    }
}