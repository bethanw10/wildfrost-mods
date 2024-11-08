using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Utils;
using HarmonyLib;
using UnityEngine;

namespace HadesFrost.Setup
{
    public static class Tribe
    {
        public static void Setup(HadesFrost mod)
        {
            mod.Classes.Add(mod.TribeCopy("Basic", "Hades") //Snowdweller = "Basic", Shadmancer = "Magic"
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
                        "WitchsStaff", "WitchsStaff", "SisterBlades", "MoonstoneAxe", "FrostbittenHorn", "Pom Slice", "Skelly", "Nectar")
                        .ToList();
                    data.startingInventory = inventory;

                    DataFile[] units = mod.DataList<CardData>(
                        "Ares", "Artemis", "Athena", "Aphrodite", "Apollo", "Demeter", "Dionysus",
                        "Hera", "Hermes", "Hestia", "Hephaestus", "Poseidon", "Zeus",
                        "Flash" /*vesta*/, "Zoog" /*shen*/, "Zula", "Wort", "Shelly", "Chompom", "Yuki", "Wallop", "Kernel");

                    var unitPool = CreateRewardPool("DrawUnitPool", "Units", units);

                    DataFile[] items = mod.DataList<CardData>(
                        "FlashWhip", "HongosHammer", "NutshellCake", "ScrapPile", "ShellShield", "Shellbo", "SporePack",
                        "IridescentFan", "ThunderSignet", "Ambrosia", "AdamantShard", "UmbralFlames", "ArgentSkull");

                    var itemPool = CreateRewardPool("DrawItemPool", "Items", items);

                    var charmPool = CreateRewardPool("DrawCharmPool", "Charms", mod.DataList<CardUpgradeData>(
                        "CardUpgradeOverload", "CardUpgradeConsumeOverload", "CardUpgradeShellBecomesSpice",
                        "CardUpgradeShroomReduceHealth",
                        "CardUpgradeShellOnKill", "CardUpgradeShroom", "CardUpgradeAcorn",
                        "CardUpgradeBlackShawl", "CardUpgradeBoneHourglass"));

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
            pool.type = type;            //The usual types are Units, Items, Charms, and Modifiers.
            pool.list = list.ToList();
            return pool;
        }

        [HarmonyPatch(typeof(References), nameof(References.Classes), MethodType.Getter)]
        private static class ClassesGetterFix
        {
            static void Postfix(ref ClassData[] __result) => __result = AddressableLoader.GetGroup<ClassData>("ClassData").ToArray();
        }

    }
}