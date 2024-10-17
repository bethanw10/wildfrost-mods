using System;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using FMOD;
using HarmonyLib;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ReactionRod
{
    public class ReactionRod : WildfrostMod
    {
        public ReactionRod(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.reactionrod";

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Punch-In-A-Box";

        public override string Description => "Adds a new item that can trigger reaction cards";

        private readonly List<CardDataBuilder> cards = new List<CardDataBuilder>();

        private bool preLoaded;

        private void CreateModAssets()
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintHasReaction>();

            cards.Add(
                new CardDataBuilder(this)
                    .CreateItem("punchInABox", "Punch-In-A-Box") 
                    .SetSprites("Punch.png", "PunchBG.png")
                    .WithText("Trigger a Reaction<keyword=reaction> card")
                    .WithIdleAnimationProfile("PingAnimationProfile")
                    .NeedsTarget()
                    .AddPool("GeneralItemPool")
                    .WithValue(40)
                    .SetDamage(2)
                    .SubscribeToAfterAllBuildEvent(delegate (CardData data)
                    {
                        data.attackEffects = new[] { SStack("Trigger (High Prio)") };
                        //SStack("Reduce Max Health")
                        // data.attackEffects = new[] { SStack("Trigger (High Prio)"), SStack("Add <+1> to all status effects") };
                        data.targetConstraints = new TargetConstraint[] { constraint };
                    })
            );
            preLoaded = true;
        }

        public override void Load()
        {
            if (!preLoaded) { CreateModAssets(); }
            base.Load();
        }

        public override void Unload()
        {
            preLoaded = false;
            RemoveFromPools();
            base.Unload();
        }

        public override List<T> AddAssets<T, TY>()
        {
            var typeName = typeof(TY).Name;
            switch (typeName)
            {
                case nameof(CardData):
                    return cards.Cast<T>().ToList();
                default:
                    return null;
            }
        }

        private CardData.StatusEffectStacks SStack(string name, int amount = 1) => new CardData.StatusEffectStacks(Get<StatusEffectData>(name), amount);

        private void RemoveFromPools()
        {
            try
            {
                string[] poolsToCheck = { "GeneralItemPool" };
                foreach (var poolName in poolsToCheck)
                {
                    var pool = Extensions.GetRewardPool(poolName);
                    if (pool == null)
                    {
                        continue;
                    }
                    for (var i = pool.list.Count - 1; i >= 0; i--)
                    {
                        if (pool.list[i] == null || pool.list[i]?.ModAdded == this)
                        {
                            pool.list.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[bethan] error removing from reward pool" + e.Message);
            }
        }
    }
}