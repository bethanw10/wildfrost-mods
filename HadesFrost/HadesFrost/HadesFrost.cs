using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Utils;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
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

        public List<CampaignNodeTypeBuilder> CampaignNodes { get; } = new List<CampaignNodeTypeBuilder>();

        public static GameObject PrefabHolder;

        private bool preLoaded;

        private TMP_SpriteAsset hadesSprites;

        public override TMP_SpriteAsset SpriteAsset => hadesSprites;

        private void CreateModAssets()
        {
            Leaders.Setup(this);
            Companions.Setup(this);
            Pets.Setup(this);
            Charms.Setup(this);
            Items.Setup(this);
            StatusTypes.Setup(this);
            Boons.Setup(this);
            Tribe.Setup(this);


            Selene();
            SpriteAssetsFix();

            preLoaded = true;
        }

        private void Selene()
        {
            CampaignNodes.Add(new CampaignNodeTypeBuilder(this)
                .Create<CampaignNodeTypeSelene>("PortalNode")
                .WithZoneName("Portal") //The name of the CampaignNode associated to the map node. Used for special event replacement.
                .WithCanEnter(true)     //Needs to be true to be interactable
                .WithInteractable(true) //Needs to be true to be interactable
                .WithCanSkip(true)      //If you want this node unskippable, replace this line with .WithMustClear(true)
                .WithCanLink(true)      //See below.
                .WithLetter("p")        //See below.
                .SubscribeToAfterAllBuildEvent(
                    (data) =>
                    {
                        var castData = (CampaignNodeTypeSelene)data;
                       // castData.itemNode = this.TryGet<CampaignNodeTypeEvent>("CampaignNodeTypeItem");
                        var item = this.TryGet<CampaignNodeType>("CampaignNodeItem"); //.routinePrefabRef;
                        castData.routinePrefabRef = ((CampaignNodeTypeItem)item).routinePrefabRef;
                        // castData.prefab = item;

                        //Inside the SubscribeToAfterAllBuildEvent
                        //Some MapNode stuff
                        var mapNode = this.TryGet<CampaignNodeType>("CampaignNodeGold").mapNodePrefab.InstantiateKeepName(); //There's a lot of things in one of these prefabs
                        mapNode.name = GUID + ".Portal";               //Changing the name                                                   
                        data.mapNodePrefab = mapNode;                  //And assign it to our node type before we forget.

                        var uiText = LocalizationHelper.GetCollection("UI Text", SystemLanguage.English);
                        var key = mapNode.name + "Ribbon";
                        uiText.SetString(key, "Mysterious Portal");    //Define the Localized string for our ribbon title.
                        mapNode.label.GetComponentInChildren<LocalizeStringEvent>().StringReference = uiText.GetString(key);
                        //Find the LocalizeStringEvent and set it to our own.

                        //The game will randomly pick between the options available. They will also pick the same index for both sprites and cleared sprites, if possible.
                        mapNode.spriteOptions = new Sprite[2] { ScaledSprite("portalClosed.png", 200), ScaledSprite("portalClosed.png", 200) };
                        mapNode.clearedSpriteOptions = new Sprite[2] { ScaledSprite("portal.png", 200), ScaledSprite("portal.png", 200) };
                        //I am using 360x274px images, but setting pixelsPerUnit to 200 scales it down to 180x137px.

                        var nodeObject = mapNode.gameObject;             //MapNode is a MonoBehaviour, so there is an underlying GameObject.
                        nodeObject.transform.SetParent(PrefabHolder.transform); //Ensures your reference doesn't poof out of existence.
                    })
            );
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

        private static void LeaderImagesFix(Entity entity)
        {
            if (entity.display is Card card && !card.hasScriptableImage) //These cards should use the static image
            {
                card.mainImage.gameObject.SetActive(true);               //And this line turns them on
            }
        }

        public override void Load()
        {
            if (!preLoaded)
            {
                CreateModAssets();
            }

            Events.OnEntityCreated += LeaderImagesFix;
            Events.OnEntityChosen += EntityChosen;
            Events.OnSceneLoaded += InsertPortalViaSpecialEvent;
            Events.OnCampaignLoadPreset += InsertPortalViaPreset;
            // Events.OnSceneChanged += CardsPhoto;

            PrefabHolder = new GameObject(GUID);   
            Object.DontDestroyOnLoad(PrefabHolder);
            PrefabHolder.SetActive(false);

            base.Load();



            // AddToPopulator();

            var gameMode = this.TryGet<GameMode>("GameModeNormal");
            gameMode.classes = gameMode.classes.Append(this.TryGet<ClassData>("Hades")).ToArray();
        }

        public override void Unload()
        {
            preLoaded = false;
            // TODO
            Events.OnEntityChosen -= EntityChosen;
            Events.OnEntityCreated -= LeaderImagesFix;
            Events.OnSceneLoaded -= InsertPortalViaSpecialEvent;
            Events.OnCampaignLoadPreset -= InsertPortalViaPreset;

            var gameMode = this.TryGet<GameMode>("GameModeNormal");
            gameMode.classes = RemoveNulls(gameMode.classes);  //Without this, a non-restarted game would crash on tribe selection
            UnloadFromClasses();                               //This tutorial doesn't need it, but it doesn't hurt to clean the pools

            // RemoveFromPopulator();
            PrefabHolder.Destroy();

            base.Unload();
        }

        private void EntityChosen(Entity entity)
        {
            Boons.GiveBoons(this, entity);
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
                case nameof(CampaignNodeType):
                    return CampaignNodes.Cast<T>().ToList();
                default:
                    return null;
            }
        }

        private void UnloadFromClasses()
        {
            var tribes = AddressableLoader.GetGroup<ClassData>("ClassData");
            foreach (var tribe in tribes)
            {
                if (tribe == null || tribe.rewardPools == null) { continue; } //This isn't even a tribe; skip it.

                foreach (var pool in tribe.rewardPools.Where(pool => pool != null))
                {
                    pool.list.RemoveAllWhere((item) => item == null || item.ModAdded == this); //Find and remove everything that needs to be removed.
                }
            }
        }

        private T[] RemoveNulls<T>(T[] data) where T : DataFile
        {
            var list = data.ToList();
            list.RemoveAll(x => x == null || x.ModAdded == this);
            return list.ToArray();
        }

        public void CardsPhoto(Scene scene)
        {
            if (scene.name == "Town")
            {
                References.instance.StartCoroutine(CardsPhoto2());
            }
        }

        private static IEnumerator CardsPhoto2()
        {
            var everyGeneration = new[] { "Ares", "Artemis", "Athena", "Aphrodite", "Apollo", "Demeter", "Dionysus", "Hera", "Hermes", "Hestia", "Hephaestus", "Poseidon", "Zeus" };
            // string[] everyGeneration = new string[] { "Frinos", "Toula" };
            yield return SceneManager.WaitUntilUnloaded("CardFramesUnlocked");
            yield return SceneManager.Load("CardFramesUnlocked", SceneType.Temporary);
            var sequence = Object.FindObjectOfType<CardFramesUnlockedSequence>();
            var titleObject = sequence.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
            titleObject.text = "New Cards!";
            yield return sequence.StartCoroutine("CreateCards", everyGeneration.Select((string s) => ("bethanw10.hadesfrost") + "." + s).ToArray());
        }

        internal Sprite ScaledSprite(string fileName, int pixelsPerUnit = 100)
        {
            var tex = ImagePath(fileName).ToTex();
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, (20f * pixelsPerUnit) / (tex.height * 100f)), pixelsPerUnit);
        }

        //Be sure to hook and unhook to Events.OnCampaignLoadPreset in the Load and Unload methods respectively
        private static void InsertPortalViaPreset(ref string[] preset)
        {
            // todo hades only?
            //See References for the two possible presets.
            //Lines 0 + 1: Node types
            //Line 2: Battle Tier (fight 1, fight 2, etc)
            //Line 3: Zone (Snow Tundra, Ice Caves, Frostlands)
            const char letter = 'S'; //S is for Snowdwell, b is for non-boss, B is for boss.
            var targetAmount = 1; //Stop after the 1st S.

            for (var i = 0; i < preset[0].Length; i++)
            {
                if (preset[0][i] == letter)
                {
                    targetAmount--;
                    if (targetAmount == 0)
                    {
                        preset[0] = preset[0].Insert(i + 1, "p");//"p" for portal
                        for (var j = 1; j < preset.Length; j++)
                        {
                            preset[j] = preset[j].Insert(i + 1, preset[j][i].ToString()); //Whatever the ref node used
                        }
                        break; //Once the portal is placed, no need for other portals.
                    }
                }
            }
        }


        //Floating class fields. Place them inside the main mod class but outside of any methods.
        public int[] addToTiers = new int[] { 0, 1, 2, 3, 4 };//First two Acts
        public int amountToAdd = 2;

        //Load should call this method. If you did the preset manipulation already, feel free to comment out the event hook for that.
        public void AddToPopulator()
        {
            CampaignPopulator populator = this.TryGet<GameMode>("GameModeNormal").populator; //Find the populator
            foreach (int i in addToTiers)                                 //Iterate through the desired tiers
            {
                CampaignTier tier = populator.tiers[i];
                List<CampaignNodeType> list = tier.rewardPool.ToList();  //Convert the array to a list to easier adding
                for (int j = 0; j < amountToAdd; j++)
                {
                    list.Add(this.TryGet<CampaignNodeType>("PortalNode"));    //Add as much times as desired             
                }

                tier.rewardPool = list.ToArray();                        //Replace the old array 
            }
        }

        //Unoad should call this method. If you did the preset manipulation already, feel free to comment out the event unhook for that.
        public void RemoveFromPopulator()
        {
            CampaignPopulator populator = this.TryGet<GameMode>("GameModeNormal").populator; //Find the populator
            foreach (int i in addToTiers)                                 //Iterate through the desired tiers
            {
                CampaignTier tier = populator.tiers[i];
                List<CampaignNodeType> list = tier.rewardPool.ToList();
                list.RemoveAll(x => x == null || x.ModAdded == this);     //Remove everything that needs to be removed
                tier.rewardPool = list.ToArray();
            }
        }

        //Hook this method onto Events.OnSceneLoaded somewhere in your Load method. Also, remember to unhook in Unload.
        //Remember to comment out the other two approaches.
        public void InsertPortalViaSpecialEvent(Scene scene)//The Scene class is from the UnityEngine.SceneManagement namespace
        {
            if (scene.name == "Campaign")
            {
                SpecialEventsSystem specialEvents = GameObject.FindObjectOfType<SpecialEventsSystem>(); //Only 1 of these exists
                SpecialEventsSystem.Event eve = new SpecialEventsSystem.Event()
                {
                    requiresUnlock = null,                                    //Unnecessary as this is default, but really just showing that it exists
                    nodeType = this.TryGet<CampaignNodeType>("PortalNode"),        //Our portal
                    replaceNodeTypes = new string[] { "CampaignNodeReward" }, //If you spell this string wrong, the game will loop endlessly
                    minTier = 3,                                              //After the first boss
                    perTier = new Vector2Int(0, 1),                            //Maximum of 2 per tier
                    perRun = new Vector2Int(1, 1)                              //Between 2 and 4 portals per run
                };
                specialEvents.events = specialEvents.events.AddItem(eve).ToArray();
            }
        }
    }
}