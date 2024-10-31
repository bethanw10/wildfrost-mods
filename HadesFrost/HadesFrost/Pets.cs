using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Extensions;

namespace HadesFrost
{
    public static class Pets
    {
        public static void Setup(HadesFrost mod)
        {
            Frinos(mod);
            Toula(mod);
        }

        private static void Frinos(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectBlockBarrage>("Block Barrage")
                    .WithCanBeBoosted(true)
                    .WithText("Block <keyword=barrage> attacks from hitting allies behind")
                    .WithType("")
            );

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Frinos", "Frinos", idleAnim: "FloatAnimationProfile")
                .SetSprites("Frinos.png", "FrinosBG.png")
                .SetStats(5)
                .IsPet((ChallengeData)null)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.startWithEffects = new[]
                    {
                        mod.SStack("When Hit Heal Self"),
                        mod.SStack("Block Barrage")
                    };
                }));
        }

        private static void Toula(HadesFrost mod)
        {
            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectTriggerWhenAllyBehindAttacks>("Trigger When Ally Behind Attacks")
                    .WithCanBeBoosted(true)
                    .WithText("Trigger when ally behind attacks")
                    .WithType("")
                    .WithIsReaction(true)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        data.descColorHex = "F99C61";
                    })
            );

            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectApplyXPostAttack>("Snow Self")
                    .WithCanBeBoosted(true)
                    .WithText("Apply <{a}> <keyword=snow> to self")
                    .WithType("")
                    .WithIsReaction(false)
                    .SubscribeToAfterAllBuildEvent(data =>
                    {
                        var castData = (StatusEffectApplyXPostAttack)data;
                        castData.effectToApply = mod.TryGet<StatusEffectData>("Snow");
                        castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Self;
                        castData.queue = true;
                    })
            );

            mod.Cards.Add(new CardDataBuilder(mod)
                .CreateUnit("Toula", "Toula", idleAnim: "FloatAnimationProfile")
                .SetSprites("Toula.png", "ToulaBG.png")
                .SetStats(3, 6)
                .IsPet((ChallengeData)null)
                .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                {
                    data.startWithEffects = new[]
                    {
                        mod.SStack("Trigger When Ally Behind Attacks"),
                        mod.SStack("Snow Self", 2),
                    };
                }));
        }
    }
}