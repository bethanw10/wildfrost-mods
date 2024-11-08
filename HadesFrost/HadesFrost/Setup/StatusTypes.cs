using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.StatusEffects;
using HadesFrost.Utils;
using UnityEngine;

namespace HadesFrost.Setup
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
            mod.CreateIconKeyword("hitch", "Hitch", "Damages other Hitched allies when hit | Does not count down!", "hitchicon")
                .ChangeColor(note: new Color(0.98f, 0.89f, 0.61f));

            StatusIcons.CreateIcon(
                "hitchicon",
                mod.ImagePath("hitchicon.png").ToSprite(),
                "hitch",
                "counter",
                Color.black,
                Color.clear,
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

            // mod.StatusEffects.Add(
            //     new StatusEffectDataBuilder(mod)
            //         .Create<StatusEffectApplyXWhenHit>("Hitch")
            //         .WithCanBeBoosted(true)
            //         .WithType("hitch")
            //         .WithKeyword("hitch")
            //         .WithIsStatus(true)
            //         .WithVisible(true)
            //         .WithCanBeBoosted(true)
            //         .WithOffensive(true)
            //         .WithIconGroupName("health")
            //         .WithTextInsert("{a}")
            //         .WithStackable(true)
            //         .SubscribeToAfterAllBuildEvent(delegate (StatusEffectData data)
            //         {
            //             var castData = (StatusEffectApplyXWhenHit)data;
            //             castData.removeOnDiscard = true;
            //             castData.applyFormatKey = mod.TryGet<StatusEffectData>("Shroom").applyFormatKey;
            //             castData.eventPriority = 2;
            //             castData.targetConstraints = new TargetConstraint[]
            //             {
            //                 ScriptableObject.CreateInstance<TargetConstraintIsUnit>()
            //             };
            //
            //             var script = ScriptableObject.CreateInstance<ScriptableCurrentStatus>();
            //             script.statusType = "hitch";
            //
            //             castData.contextEqualAmount = script;
            //             castData.canRetaliate = false;
            //             castData.dealDamage = true;
            //             castData.doesDamage = true;
            //             castData.countsAsHit = false;
            //
            //             var constraint = ScriptableObject.CreateInstance<TargetConstraintHasStatusType>();
            //             constraint.statusType = "hitch";
            //
            //             castData.applyConstraints = new TargetConstraint[] { constraint };
            //
            //             castData.applyToFlags = StatusEffectApplyX.ApplyToFlags.Allies;
            //         })
            // );
        }
    }
}