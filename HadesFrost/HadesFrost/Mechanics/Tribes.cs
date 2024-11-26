using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using FMODUnity;
using HadesFrost.Utils;
using HarmonyLib;
using UnityEngine;

namespace HadesFrost.Mechanics
{
    public static class Tribes
    {
        public const string MELINOE_TRIBE = "Hades";
        public const string ZAGREUS_TRIBE = "Zag";

        public static void Setup(HadesFrost mod)
        {
            Zagreus(mod);
            Melinoe(mod);
        }

        private static void Melinoe(HadesFrost mod)
        {
            mod.Classes.Add(mod.TribeCopy("Basic", MELINOE_TRIBE) //Snowdweller = "Basic", Shadmancer = "Magic"
                .WithFlag("Images/DrawFlag.png")
                .WithSelectSfxEvent(RuntimeManager.PathToEventReference("event:/sfx/card/draw_multi"))
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
                            "WitchsStaff", "WitchsStaff", "SisterBlades", "SisterBlades", "MoonstoneAxe", "FrostbittenHorn",
                            "Pom Slice", "Schelemeus", "Nectar"
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
                        "IridescentFan", "ThunderSignet", "Ambrosia", "AdamantShard", "SunRod"
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

        private static void Zagreus(HadesFrost mod)
        {
            mod.Classes.Add(mod.TribeCopy("Basic", ZAGREUS_TRIBE) //Snowdweller = "Basic", Shadmancer = "Magic"
                .WithFlag("Images/DrawFlag.png")
                .WithSelectSfxEvent(RuntimeManager.PathToEventReference("event:/sfx/card/draw_multi"))
                .SubscribeToAfterAllBuildEvent(data =>
                {
                    var gameObject = data.characterPrefab.gameObject.InstantiateKeepName();
                    Object.DontDestroyOnLoad(gameObject);
                    gameObject.name = "Player (Zag)";
                    data.characterPrefab = gameObject.GetComponent<Character>();
                    data.leaders = mod.DataList<CardData>("Zagreus");

                    var inventory = ScriptableObject.CreateInstance<Inventory>();
                    inventory.deck.list = mod.DataList<CardData>(
                            "StygianBlade", "StygianBlade", "EternalSpear", "ShieldOfChaos", "FrostbittenHorn",
                            "LambentPlume", "LambentPlume", "Pom Slice", "Skelly", "Nectar"
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
                        "IridescentFan", "ThunderSignet", "Ambrosia", "AdamantShard", "SunRod"
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
            gameMode.classes = gameMode.classes.Append(mod.TryGet<ClassData>(ZAGREUS_TRIBE)).ToArray();
            gameMode.classes = gameMode.classes.Append(mod.TryGet<ClassData>(MELINOE_TRIBE)).ToArray();
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