using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Utils;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace HadesFrost.Setup
{
    public static class Tribe
    {
        private const string TRIBE_NAME = "Hades";

        public static void Setup(HadesFrost mod)
        {
            mod.Classes.Add(mod.TribeCopy("Basic", TRIBE_NAME) //Snowdweller = "Basic", Shadmancer = "Magic"
                .WithFlag("Images/DrawFlag.png")
                .WithSelectSfxEvent(FMODUnity.RuntimeManager.PathToEventReference("event:/sfx/card/draw_multi"))
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var gameObject = data.characterPrefab.gameObject.InstantiateKeepName();
                    Object.DontDestroyOnLoad(gameObject);
                    gameObject.name = "Player (Hades)";
                    data.characterPrefab = gameObject.GetComponent<Character>();
                    data.leaders = mod.DataList<CardData>("Melinoe");

                    var inventory = ScriptableObject.CreateInstance<Inventory>();
                    inventory.deck.list = mod.DataList<CardData>(
                        // "WitchsStaff", "SisterBlades", "MoonstoneAxe", "ArgentSkull", "UmbralFlames", "BlackCoat", "FrostbittenHorn", "Pom Slice", "Skelly", "Nectar"
                        "WitchsStaff", "WitchsStaff", "SisterBlades", "SisterBlades", "MoonstoneAxe", "FrostbittenHorn", "Pom Slice", "Skelly", "Nectar"
                        )
                        .ToList();
                    data.startingInventory = inventory;

                    DataFile[] units = mod.DataList<CardData>(
                        "Ares", "Artemis", "Athena", "Aphrodite", "Apollo", "Demeter", "Dionysus",
                        "Hera", "Hermes", "Hestia", "Hephaestus", "Poseidon", "Zeus",
                        "Flash" /*vesta*/, "Zoog" /*shen*/, "Zula", "Wort", "Shelly", "Chompom", "Yuki", "Wallop", "Kernel");

                    var unitPool = CreateRewardPool("DrawUnitPool", "Units", units);

                    DataFile[] items = mod.DataList<CardData>(
                        "FlashWhip", "HongosHammer", "NutshellCake", "ScrapPile", "ShellShield", "Shellbo", "SporePack",
                        "IridescentFan", "ThunderSignet", "Ambrosia", "AdamantShard"
                        , "UmbralFlames", "ArgentSkull", "BlackCoat"
                        );

                    var itemPool = CreateRewardPool("DrawItemPool", "Items", items);

                    DataFile[] charms = mod.DataList<CardUpgradeData>(
                        "CardUpgradeOverload", "CardUpgradeConsumeOverload", "CardUpgradeShroomReduceHealth",
                        "CardUpgradeShellOnKill", "CardUpgradeShroom", "CardUpgradeAcorn",
                        "CardUpgradeBlackShawl", "CardUpgradeBoneHourglass", "CardUpgradeDiscordantBell",
                        "CardUpgradeLionFang", "CardUpgradeVividSea", "CardUpgradeCloudBangle");

                    var charmPool = CreateRewardPool("DrawCharmPool", "Charms", charms);

                    data.rewardPools = new[]
                    {
                        unitPool,
                        itemPool,
                        charmPool,
                        Extensions.GetRewardPool("GeneralUnitPool"),
                        Extensions.GetRewardPool("GeneralItemPool"),
                        Extensions.GetRewardPool("GeneralCharmPool"),
                        Extensions.GetRewardPool("GeneralModifierPool"),
                        Extensions.GetRewardPool("SnowUnitPool"),
                        Extensions.GetRewardPool("SnowItemPool"),
                        Extensions.GetRewardPool("SnowCharmPool"),
                    };
                })
            );
        }

        private static RewardPool CreateRewardPool(string name, string type, DataFile[] list)
        {
            var pool = ScriptableObject.CreateInstance<RewardPool>();
            pool.name = name;
            pool.type = type;
            pool.list = list.ToList();
            return pool;
        }

        public static void AppendTribe(HadesFrost mod)
        {
            var gameMode = mod.TryGet<GameMode>("GameModeNormal");
            gameMode.classes = gameMode.classes.Append(mod.TryGet<ClassData>(TRIBE_NAME)).ToArray();
        }

        public static void UnAppendTribe(HadesFrost mod)
        {
            var gameMode = mod.TryGet<GameMode>("GameModeNormal");
            gameMode.classes = mod.RemoveNulls(gameMode.classes);
            UnloadFromClasses(mod);
        }

        private static void UnloadFromClasses(WildfrostMod mod)
        {
            var tribes = AddressableLoader.GetGroup<ClassData>("ClassData");
            foreach (var tribe in tribes)
            {
                if (tribe == null || tribe.rewardPools == null)
                {
                    continue;
                }

                foreach (var pool in tribe.rewardPools.Where(pool => pool != null))
                {
                    pool.list.RemoveAllWhere(item => item == null || item.ModAdded == mod); //Find and remove everything that needs to be removed.
                }
            }
        }

        [HarmonyPatch(typeof(References), nameof(References.Classes), MethodType.Getter)]
        private static class ClassesGetterFix
        {
            static void Postfix(ref ClassData[] __result) => __result = AddressableLoader.GetGroup<ClassData>("ClassData").ToArray();
        }
    }
}