using System;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.ButtonStatuses;
using HadesFrost.StatusEffects;
using HadesFrost.Utils;
using UnityEngine;
using static HadesFrost.Utils.Common;

namespace HadesFrost.Setup
{
    public static class Calls
    {
        public static void Setup(HadesFrost mod)
        {
            CallKeyword(mod);

            GodGauge(mod);

            AphroditeCall(mod);
            AresCall(mod);
            AthenaCall(mod);
            DemeterCall(mod);
            DionysusCall(mod);
        }

        private static void CallKeyword(HadesFrost mod)
        {
            mod.Keywords.Add(
                new KeywordDataBuilder(mod)
                    .Create("call")
                    .WithShowName(true)
                    .WithShowIcon(false)
                    .WithTitle("Call")
                    .WithCanStack(false)
                    .WithPanelColour(Color.grey)
                    .WithBodyColour(new Color(22, 28, 21))
                    .WithTitleColour(new Color(0.970f, 0.829f, 0.0291f))
                    .WithDescription(
                        "Calls are abilities that require <sprite name=magickicon>\n\n" +
                        "When a <Call> is equipped, gain <sprite name=magickicon> equal to all damage dealt by your team\n\n" +
                        "Calls can be activated by clicking their icon"));
        }
        
        private static void GodGauge(HadesFrost mod)
        {
            mod.CreateIconKeyword("godgauge", "God Gauge", "A resource used for activating <Calls>", "magickicon")
                .ChangeColor(note: new Color(0.98f, 0.89f, 0.61f));
        
            StatusIcons.CreateIcon(
                "magickicon",
                mod.ImagePath("magickicon.png").ToSprite(),
                "godgauge",
                "counter",
                Color.white,
                Color.clear,
                new[] { mod.TryGet<KeywordData>("godgauge") });
        
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXWhenEnemyTakesDamage>("Gain Magick Equal To Team Damage Dealt")
                    .WithCanBeBoosted(false)
                    .WithType("")
                    .WithStackable(false)
                    .WithText("Gain <keyword=godgauge> equal to damage dealt to enemy")
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        var castData = (StatusEffectApplyXWhenEnemyTakesDamage)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Magick");
                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        castData.applyEqualAmount = true;
                        castData.doPing = false;
                        castData.AllTypes = true;
                        castData.IgnoreType = "hex";
                        data.targetConstraints = TargetConstraintAlliesOnly(mod);
                    })
            );
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectNothing>("God Gauge")
                    .WithType("godgauge")
                    .WithKeyword("godgauge")
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

        private static void AphroditeCall(HadesFrost mod)
        {
            const int cost = 100;
            var description = "Apply <1> <keyword=haze> to a random enemy\n" + $"<Cost: {cost}> <sprite name=magickicon>";

            const string name = "Aphrodite's";
            var nameOnlyLetters = name.Replace(" ", "").Replace("'", "");
            var keywordName = nameOnlyLetters.ToLower();

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects
                .Add(new StatusEffectDataBuilder(mod)
                .Create<StatusActionApplyX>(nameOnlyLetters + "Aid")
                .WithType(keywordName)
                .WithStackable(false)
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var castData = (StatusActionApplyX)data;
                    castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.RandomEnemy;
                    castData.visible = true;
                    castData.isStatus = true;
                    castData.iconGroupName = "counter";
                    castData.effectToApply = mod.TryGet<StatusEffectData>("Haze");
                    castData.MagickCost = cost;

                    data.targetConstraints = TargetConstraintAlliesOnly(mod);
                })
            );
        }

        private static void AresCall(HadesFrost mod)
        {
            const int cost = 60;
            var description = "Apply <4> <keyword=teeth> to self\n" + $"<Cost: {cost}> <sprite name=magickicon>";

            const string name = "Ares'";
            var nameOnlyLetters = name.Replace(" ", "").Replace("'", "");
            var keywordName = nameOnlyLetters.ToLower();

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects
                .Add(new StatusEffectDataBuilder(mod)
                    .Create<StatusActionApplyX>(nameOnlyLetters + "Aid")
                    .WithType(keywordName)
                    .WithStackable(false)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        var castData = (StatusActionApplyX)data;
                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        castData.visible = true;
                        castData.isStatus = true;
                        castData.iconGroupName = "counter";
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Teeth");
                        castData.MagickCost = cost;

                        data.targetConstraints = TargetConstraintAlliesOnly(mod);
                    })
                );
        }

        private static void AthenaCall(HadesFrost mod)
        {
            const int cost = 100;
            var description = "Gain `<1> <keyword=block>' and <1> <deflect>\n" + $"<Cost: {cost}> <sprite name=magickicon>";

            const string name = "Athena's";
            var nameOnlyLetters = name.Replace(" ", "").Replace("'", "");
            var keywordName = nameOnlyLetters.ToLower();

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects
                .Add(new StatusEffectDataBuilder(mod)
                    .Create<StatusActionApplyX>(nameOnlyLetters + "Aid")
                    .WithType(keywordName)
                    .WithStackable(false)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        var castData = (StatusActionApplyX)data;
                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        castData.visible = true;
                        castData.isStatus = true;
                        castData.iconGroupName = "counter";
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Block");
                        castData.MagickCost = cost;

                        data.targetConstraints = TargetConstraintAlliesOnly(mod);
                    })
                );
        }

        private static void DemeterCall(HadesFrost mod)
        {
            const int cost = 100;
            var description = "Apply <1> <keyword=snow> to all enemies\n" + $"<Cost: {cost}> <sprite name=magickicon>";

            const string name = "Demeter's";
            var nameOnlyLetters = name.Replace(" ", "").Replace("'", "");
            var keywordName = nameOnlyLetters.ToLower();

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects
                .Add(new StatusEffectDataBuilder(mod)
                    .Create<StatusActionApplyX>(nameOnlyLetters + "Aid")
                    .WithType(keywordName)
                    .WithStackable(false)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        var castData = (StatusActionApplyX)data;
                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Enemies;
                        castData.visible = true;
                        castData.isStatus = true;
                        castData.iconGroupName = "counter";
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Snow");
                        castData.MagickCost = cost;

                        data.targetConstraints = TargetConstraintAlliesOnly(mod);
                    })
                );
        }
       
        private static void DionysusCall(HadesFrost mod)
        {
            const int cost = 50;
            var description = "Gain `When hit, apply <1> <keyword=shroom> to attacker'\n" + $"<Cost: {cost}> <sprite name=magickicon>";

            const string name = "Dionysus'";
            var nameOnlyLetters = name.Replace(" ", "").Replace("'", "");
            var keywordName = nameOnlyLetters.ToLower();

            CreateKeyword(mod, keywordName, name, description);
            CreateCard(mod, name, description);

            mod.StatusEffects
                .Add(new StatusEffectDataBuilder(mod)
                    .Create<StatusActionApplyX>(nameOnlyLetters + "Aid")
                    .WithType(keywordName)
                    .WithStackable(false)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        var castData = (StatusActionApplyX)data;
                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        castData.visible = true;
                        castData.isStatus = true;
                        castData.iconGroupName = "counter";
                        castData.effectToApply = mod.TryGet<StatusEffectData>("When Hit Apply Shroom To Attacker");
                        castData.MagickCost = cost;

                        data.targetConstraints = TargetConstraintAlliesOnly(mod);
                    })
                );
        }

        private static void CreateCard(HadesFrost mod, string name, string description)
        {
            var nameNoSpaces = name.Replace(" ", "").Replace("'", "");
            mod.Cards.Add(
                new CardDataBuilder(mod)
                    .CreateUnit(nameNoSpaces + "Aid", name + " Aid", idleAnim: "PulseAnimationProfile")
                    .WithCardType("Summoned")
                    .SetSprites("LunarRay" + ".png", "HexBG.png")
                    .SetStats()
                    .WithText($"Leader gains <keyword={Extensions.PrefixGUID("call", mod)}>:\n" + description));
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

        public static void GiveCall(HadesFrost mod, Entity entity)
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

            if (cardName == "AphroditesAid" || cardName == "AresAid")
            {
                statusEffect = mod.SStack(cardName);
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