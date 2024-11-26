using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.StatusEffects;
using HadesFrost.Utils;
using UnityEngine;

namespace HadesFrost.Mechanics
{
    public static class StatusTypes
    {
        public static void Setup(HadesFrost mod)
        {
            Jolted(mod);
            Hitch(mod);
        }

        private static void Jolted(HadesFrost mod)
        {
            mod.CreateIconKeyword("jolted", "Jolted", "Take damage after triggering | Does not count down!", "joltedicon")
                .ChangeColor(note: new Color(0.98f, 0.89f, 0.61f));

            StatusIcons.CreateIcon(
                "joltedicon",
                mod.ImagePath("joltedicon.png").ToSprite(),
                "jolted",
                "counter",
                Color.black,
                Color.clear,
                new[] { mod.TryGet<KeywordData>("jolted") }, -1);

            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectJolted>("Jolted")
                    .WithCanBeBoosted(true)
                    .WithType("jolted")
                    .WithKeyword("jolted")
                    .WithIsKeyword(true)
                    .WithVisible(true)
                    .WithCanBeBoosted(true)
                    .WithOffensive(true)
                    .WithIconGroupName("health")
                    .WithTextInsert("{a}")
                    .WithStackable(true)
                    .FreeModify(delegate (StatusEffectJolted data)
                    {
                        data.removeOnDiscard = true;
                        data.applyFormatKey = mod.TryGet<StatusEffectData>("Shroom").applyFormatKey;
                        data.targetConstraints = new TargetConstraint[] { ScriptableObject.CreateInstance<TargetConstraintIsUnit>() };
                    })
            );
        }

        private static void Hitch(HadesFrost mod)
        {
            mod.CreateIconKeyword("hitch", "Hitch", "When any damage taken, deal damage to other <sprite name=hitchicon>'d allies | Does not count down!", "hitchicon")
                .ChangeColor(note: new Color(0.98f, 0.89f, 0.61f));

            StatusIcons.CreateIcon(
                "hitchicon",
                mod.ImagePath("hitchicon.png").ToSprite(),
                "hitch",
                "counter",
                new Color(0, 0.047f, 0.176f),
                new Color(0.29f, 0.776f, 1f),
                new[] { mod.TryGet<KeywordData>("hitch") }, 
                -1);

            mod.StatusEffects.Add(
                new StatusEffectDataBuilder(mod)
                    .Create<StatusEffectHitch>("Hitch")
                    .WithCanBeBoosted(true)
                    .WithType("hitch")
                    .WithKeyword("hitch")
                    .WithIsStatus(true)
                    .WithVisible(true)
                    .WithCanBeBoosted(true)
                    .WithOffensive(true)
                    .WithIconGroupName("health")
                    .WithTextInsert("{a}")
                    .WithStackable(true)
                    .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
                    {
                        var castData = (StatusEffectHitch)data;
                        castData.removeOnDiscard = true;
                        castData.applyFormatKey = mod.TryGet<StatusEffectData>("Shroom").applyFormatKey;
                        castData.eventPriority = 2;
                        castData.doesDamage = true;
                    })
            );
        }
    }
}