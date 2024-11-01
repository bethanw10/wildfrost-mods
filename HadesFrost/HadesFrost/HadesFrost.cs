using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Utils;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WildfrostHopeMod.Utils;
using Extensions = Deadpan.Enums.Engine.Components.Modding.Extensions;
using Object = UnityEngine.Object;

/* TODO:
Rework apollo unyielding, boring boons?
Card images
Pets
Items - keepsakes, ambrosia, weapons etc.
Charms
Knockback fix
Hitch fix + icon
Jolted anim
Hades Child trait
Create tribe + Tribe banner
Selene event ? 
Boon colors - custom panel?
Custom battles
*/

namespace HadesFrost
{
    public class HadesFrost : WildfrostMod
    {
        public HadesFrost(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.hadesfrost";

        public override string[] Depends => Array.Empty<string>(); // todo add in battle data and config just in case

        public override string Title => "Hades Frost";

        public override string Description => ":)";

        public List<CardDataBuilder> Cards { get; } = new List<CardDataBuilder>();

        public List<StatusEffectDataBuilder> StatusEffects { get; } = new List<StatusEffectDataBuilder>();

        public List<CardUpgradeDataBuilder> CardUpgrades { get; } = new List<CardUpgradeDataBuilder>();

        public List<TraitDataBuilder> Traits { get; } = new List<TraitDataBuilder>();

        public List<KeywordDataBuilder> Keywords { get; } = new List<KeywordDataBuilder>();

        public List<ClassDataBuilder> Classes { get; } = new List<ClassDataBuilder>();

        private bool preLoaded;

        private TMP_SpriteAsset hadesSprites;

        public override TMP_SpriteAsset SpriteAsset => hadesSprites;

        public void CardsPhoto(Scene scene)
        {
            if (scene.name == "Town")
            {
                References.instance.StartCoroutine(CardsPhoto2());
            }
        }

        private static IEnumerator CardsPhoto2()
        {
            var everyGeneration = new string[] { "Ares", "Artemis", "Athena", "Aphrodite", "Apollo", "Demeter", "Dionysus" , "Hera", "Hermes", "Hestia", "Hephaestus", "Poseidon", "Zeus" };
            // string[] everyGeneration = new string[] { "Frinos", "Toula" };
            yield return SceneManager.WaitUntilUnloaded("CardFramesUnlocked");
            yield return SceneManager.Load("CardFramesUnlocked", SceneType.Temporary);
            var sequence = Object.FindObjectOfType<CardFramesUnlockedSequence>();
            var titleObject = sequence.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
            titleObject.text = "New Cards!";
            yield return sequence.StartCoroutine("CreateCards", everyGeneration.Select((string s) => ("bethanw10.hadesfrost") + "." + s).ToArray());
        }
        private void CreateModAssets()
        {
            Leaders.Setup(this);
            Companions.Setup(this);
            Pets.Setup(this);
            Charms.Setup(this);
            Items.Setup(this);
            StatusTypes.Setup(this);
            Boons.Setup(this);

            Classes.Add(this.TribeCopy("Basic", "Hades") //Snowdweller = "Basic", Shadmancer = "Magic"
                .WithFlag("Images/DrawFlag.png") //Loads your DrawFlag.png in your Images subfolder of your mod folder
                .WithSelectSfxEvent(FMODUnity.RuntimeManager.PathToEventReference("event:/sfx/card/draw_multi"))
                .SubscribeToAfterAllBuildEvent( (data) =>   
                    {
                        var gameObject = data.characterPrefab.gameObject.InstantiateKeepName();
                        Object.DontDestroyOnLoad(gameObject);                             
                        gameObject.name = "Player (Hades)";                                   
                        data.characterPrefab = gameObject.GetComponent<Character>();
                        data.leaders = this.DataList<CardData>("Melinoe");

                        var inventory = ScriptableObject.CreateInstance<Inventory>();
                        inventory.deck.list = this.DataList<CardData>(
                            "SnowGlobe", "Sword", "Sword", "Coronacht", "Pom Slice", "Sword", "Skelly", "Nectar").ToList(); //Some odds and ends
                        inventory.upgrades.Add(this.TryGet<CardUpgradeData>("CardUpgradeCritical"));
                        data.startingInventory = inventory;

                        DataFile[] units = this.DataList<CardData>(
                            "Ares", "Artemis", "Athena", "Aphrodite", "Apollo", "Demeter", "Dionysus", 
                            "Hera", "Hermes", "Hestia", "Hephaestus", "Poseidon", "Zeus",
                            "Flash" /*vesta*/, "Zoog" /*shen*/, "Zula", "Wort", "Shelly", "Chompom", "Yuki", "Wallop", "Kernel");

                        var unitPool = CreateRewardPool("DrawUnitPool", "Units", units);

                        var itemPool = CreateRewardPool("DrawItemPool", "Items", this.DataList<CardData>(
                             "FlashWhip", "HongosHammer", "NutshellCake", "ScrapPile", "ShellShield", "Shellbo",
                             "IridescentFan", "ThunderSignet", "Ambrosia", "SporePack"
                        ));

                        var charmPool = CreateRewardPool("DrawCharmPool", "Charms", this.DataList<CardUpgradeData>(
                            "CardUpgradeOverload", "CardUpgradeConsumeOverload", "CardUpgradeShellBecomesSpice", "CardUpgradeShroomReduceHealth", 
                            "CardUpgradeShellOnKill", "CardUpgradeShroom", "CardUpgradeAcorn", 
                            "CardUpgradeBlackShawl", "CardUpgradeBoneHourglass"));

                        data.rewardPools = new RewardPool[]
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

            SpriteAssetsFix();

            preLoaded = true;
        }

        //Helper Method
        public static RewardPool CreateRewardPool(string name, string type, DataFile[] list)
        {
            var pool = ScriptableObject.CreateInstance<RewardPool>();
            pool.name = name;
            pool.type = type;            //The usual types are Units, Items, Charms, and Modifiers.
            pool.list = list.ToList();
            return pool;
        }

        private void SpriteAssetsFix()
        {
            hadesSprites = HopeUtils.CreateSpriteAsset(
                "hadesSprites",
                directoryWithPNGs: ImagePath("Sprites"),
                textures: new Texture2D[] { },
                sprites: new Sprite[] { });

            var text = Object.FindObjectOfType<FloatingText>(true);
            text.textAsset.spriteAsset.fallbackSpriteAssets.Add(hadesSprites);
        }

        //Remember to hook this method onto Events.OnEntityCreated in the Load/Unload (see Tutorial 1 or the full code for more details).
        private static void LeaderImagesFix(Entity entity)
        {
            if (entity.display is Card card && !card.hasScriptableImage) //These cards should use the static image
            {
                card.mainImage.gameObject.SetActive(true);               //And this line turns them on
            }
        }

        [HarmonyPatch(typeof(References), nameof(References.Classes), MethodType.Getter)]
        private static class ClassesGetterFix
        {
            static void Postfix(ref ClassData[] __result) => __result = AddressableLoader.GetGroup<ClassData>("ClassData").ToArray();
        }

        public override void Load()
        {
            if (!preLoaded)
            {
                CreateModAssets();
            }

            Events.OnEntityCreated += LeaderImagesFix;
            Events.OnSceneChanged += CardsPhoto;
            Events.OnEntityChosen += EntityChosen;

            base.Load();

            var gameMode = this.TryGet<GameMode>("GameModeNormal"); //GameModeNormal is the standard game mode. 
            gameMode.classes = gameMode.classes.Append(this.TryGet<ClassData>("Hades")).ToArray();
        }

        private void EntityChosen(Entity entity)
        {
            Boons.GiveBoons(this, entity);
        }

        public override void Unload()
        {
            preLoaded = false;
            // TODO
            Events.OnEntityChosen -= EntityChosen;
            Events.OnEntityCreated -= LeaderImagesFix;

            var gameMode = this.TryGet<GameMode>("GameModeNormal");
            gameMode.classes = RemoveNulls(gameMode.classes); //Without this, a non-restarted game would crash on tribe selection
            UnloadFromClasses();                               //This tutorial doesn't need it, but it doesn't hurt to clean the pools

            base.Unload();
        }

        public override List<T> AddAssets<T, TY>()
        {
            var typeName = typeof(TY).Name;
            switch (typeName)
            {
                case nameof(CardUpgradeData):
                    return CardUpgrades.Cast<T>().ToList();
                case nameof(CardData):
                    return Cards.Cast<T>().ToList();
                case nameof(StatusEffectData):
                    return StatusEffects.Cast<T>().ToList();
                case nameof(TraitData):
                    return Traits.Cast<T>().ToList();
                case nameof(KeywordData):
                    return Keywords.Cast<T>().ToList();
                case nameof(ClassData):
                    return Classes.Cast<T>().ToList();
                default:
                    return null;
            }
        }

        public void UnloadFromClasses()
        {
            var tribes = AddressableLoader.GetGroup<ClassData>("ClassData");
            foreach (var tribe in tribes)
            {
                if (tribe == null || tribe.rewardPools == null) { continue; } //This isn't even a tribe; skip it.

                foreach (var pool in tribe.rewardPools)
                {
                    if (pool == null) { continue; }; //This isn't even a reward pool; skip it.

                    pool.list.RemoveAllWhere((item) => item == null || item.ModAdded == this); //Find and remove everything that needs to be removed.
                }
            }
        }

        internal T[] RemoveNulls<T>(T[] data) where T : DataFile
        {
            var list = data.ToList();
            list.RemoveAll(x => x == null || x.ModAdded == this);
            return list.ToArray();
        }
    }
}