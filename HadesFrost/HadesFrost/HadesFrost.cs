using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.ButtonStatuses;
using HadesFrost.Setup;
using HadesFrost.Utils;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WildfrostHopeMod.SFX;
using WildfrostHopeMod.Utils;
using WildfrostHopeMod.VFX;
using Object = UnityEngine.Object;

/* TODO:
 -Maybe-
Rework apollo unyielding, boring boons?
 Selene event background ?
Boon colors - custom panel?

-Release 1-
Card images
Items - keepsakes, weapons etc.
Charms
Hitch icon
Tribe banner
Remove logs
Hex button sizes
Hex damage counts for magick
Balancing

-Release 2-
Custom battles
Zagreus
More charge items
More pets
Pet flags??
multiple boons
*/

namespace HadesFrost
{
    public class HadesFrost : WildfrostMod
    {
        public HadesFrost(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.hadesfrost";

        public override string[] Depends => Array.Empty<string>(); // todo add in battle data and config just in case + VFX/SFX

        public override string Title => "Hades Frost";

        public override string Description => ":)";

        public List<CardDataBuilder> Cards { get; } = new List<CardDataBuilder>();

        public List<StatusEffectDataBuilder> StatusEffects { get; } = new List<StatusEffectDataBuilder>();

        public List<CardUpgradeDataBuilder> CardUpgrades { get; } = new List<CardUpgradeDataBuilder>();

        public List<TraitDataBuilder> Traits { get; } = new List<TraitDataBuilder>();

        public List<KeywordDataBuilder> Keywords { get; } = new List<KeywordDataBuilder>();
        
        public List<ClassDataBuilder> Classes { get; } = new List<ClassDataBuilder>();

        public List<CampaignNodeTypeBuilder> CampaignNodeTypes { get; } = new List<CampaignNodeTypeBuilder>();

        private bool preLoaded;

        private TMP_SpriteAsset hadesSprites;
        public static GIFLoader VFX;
        public static SFXLoader SFX;

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
            Hexes.Setup(this);
            GiftsOfTheMoon.Setup(this);

            SetupSpriteAssets();
            SetupVFX();

            preLoaded = true;
        }

        private void SetupVFX()
        {
            VFX = new GIFLoader(this, this.ImagePath("Anim"));
            VFX.RegisterAllAsApplyEffect();
        }

        private void SetupSpriteAssets()
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
            Events.OnSceneLoaded += InsertSeleneViaSpecialEvent;
            Events.OnCampaignLoadPreset += InsertSeleneViaPreset;
            Events.OnCheckEntityDrag += HexButton.DisableDrag;
            // Events.OnSceneChanged += CardsPhoto;

            base.Load();
            // AddToPopulator();

            // todo move to tribe
            var gameMode = this.TryGet<GameMode>("GameModeNormal");
            gameMode.classes = gameMode.classes.Append(this.TryGet<ClassData>("Hades")).ToArray();
        }

        public override void Unload()
        {
            preLoaded = false;
            // TODO remove from pools
            Events.OnEntityChosen -= EntityChosen; // TODO move to boons
            Events.OnEntityCreated -= LeaderImagesFix;
            Events.OnSceneLoaded -= InsertSeleneViaSpecialEvent;
            Events.OnCampaignLoadPreset -= InsertSeleneViaPreset;
            Events.OnCheckEntityDrag -= HexButton.DisableDrag;

            // todo move to Tribes.cs
            var gameMode = this.TryGet<GameMode>("GameModeNormal");
            gameMode.classes = RemoveNulls(gameMode.classes); 
            UnloadFromClasses();

            // RemoveFromPopulator();
            GiftsOfTheMoon.Teardown();

            base.Unload();
        }

        private void EntityChosen(Entity entity)
        {
            Boons.GiveBoons(this, entity);
            Hexes.GiveHex(this, entity);
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
                    return CampaignNodeTypes.Cast<T>().ToList();
                default:
                    return null;
            }
        }

        private void UnloadFromClasses()
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
                    pool.list.RemoveAllWhere(item => item == null || item.ModAdded == this); //Find and remove everything that needs to be removed.
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
            var everyGeneration = new[]
            {
                "Ares", "Artemis", "Athena", "Aphrodite", "Apollo", "Demeter", 
                "Dionysus", "Hera", "Hermes", "Hestia", "Hephaestus", "Poseidon", "Zeus"
            };
            // string[] everyGeneration = new string[] { "Frinos", "Toula" };
            yield return SceneManager.WaitUntilUnloaded("CardFramesUnlocked");
            yield return SceneManager.Load("CardFramesUnlocked", SceneType.Temporary);
            var sequence = Object.FindObjectOfType<CardFramesUnlockedSequence>();
            var titleObject = sequence.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
            titleObject.text = "New Cards!";
            yield return sequence.StartCoroutine("CreateCards", everyGeneration.Select((string s) => ("bethanw10.hadesfrost") + "." + s).ToArray());
        }
        
        private void InsertSeleneViaPreset(ref string[] preset)
        {
            if (References.PlayerData?.classData?.ModAdded?.GUID != GUID)
            {
                return;
            }
            // todo hades only?
            //See References for the two possible presets.
            //Lines 0 + 1: Node types
            //Line 2: Battle Tier (fight 1, fight 2, etc)
            //Line 3: Zone (Snow Tundra, Ice Caves, Frostlands)
            //const char letter = 'B'; //S is for Snowdwell, b is for non-boss, B is for boss.
            const char letter = 'S'; //S is for Snowdwell, b is for non-boss, B is for boss.
            var targetAmount = 1; //Stop after the 1st S.

            for (var i = 0; i < preset[0].Length; i++)
            {
                if (preset[0][i] == letter)
                {
                    targetAmount--;
                    if (targetAmount == 0)
                    {
                        preset[0] = preset[0].Insert(i + 1, GiftsOfTheMoon.SeleneEventLetter);
                        for (var j = 1; j < preset.Length; j++)
                        {
                            preset[j] = preset[j].Insert(i + 1, preset[j][i].ToString());
                        }
                        break;
                    }
                }
            }
        }

        public void InsertSeleneViaSpecialEvent(Scene scene)//The Scene class is from the UnityEngine.SceneManagement namespace
        {
            if (scene.name == "Campaign")
            {
                var specialEvents = Object.FindObjectOfType<SpecialEventsSystem>(); //Only 1 of these exists
                var eve = new SpecialEventsSystem.Event()
                {
                    requiresUnlock = null,                                    //Unnecessary as this is default, but really just showing that it exists
                    nodeType = this.TryGet<CampaignNodeType>("SeleneNode"),        //Our Selene
                    replaceNodeTypes = new string[] { "CampaignNodeReward" }, //If you spell this string wrong, the game will loop endlessly
                    minTier = 3,                                         //After the first boss
                    perTier = new Vector2Int(0, 1),                            //Maximum of 2 per tier
                    perRun = new Vector2Int(1, 1)                              //Between 2 and 4 Selenes per run
                };
                specialEvents.events = specialEvents.events.AddItem(eve).ToArray();
            }
        }
    }
}