using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.ButtonStatuses;
using HadesFrost.Setup;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WildfrostHopeMod.SFX;
using WildfrostHopeMod.Utils;
using WildfrostHopeMod.VFX;
using Extensions = Deadpan.Enums.Engine.Components.Modding.Extensions;
using Object = UnityEngine.Object;

/* TODO:
 -Maybe-
Rework apollo unyielding, boring boons?
Selene event background ?
Boon colors - custom panel?
Fix clicking hex button is wonky

-Release 2-
Ascended alternatives 
Eye data
No hex on enemy
Items - keepsakes, weapons etc.
More Charms
Custom battles
Zagreus
More charge items
More pets - hecuba, raki, then companions from hades 1
Pet flags??
Hitch anim
multiple boons - choice when slecting a god??
duo boons!
*/

namespace HadesFrost
{
    public class HadesFrost : WildfrostMod
    {
        public HadesFrost(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.hadesfrost";

        public override string[] Depends => new[]
        {
            "mhcdc9.wildfrost.battle", 
            "hope.wildfrost.configs",
            "hope.wildfrost.vfx"
        };

        public override string Title => "Hades Frost";

        public override string Description =>
            @"
A mod inspired by Hades II (and partly by the first Hades).

As this is still quite new, it's likely I'll be making balancing changes in the future and there may be a some bugs.

If you want a higher chance of finding the new companions/items/charms, I recommend the Increased Odds mod!

Thank you to Lost for Jolted icon, Josh A and Michael C for the Jolted code + permission to use it, FungEMP for the Apollo idea, and everyone else on the Discord who helped me";

        public List<CardDataBuilder> Cards { get; } = new List<CardDataBuilder>();

        public List<StatusEffectDataBuilder> StatusEffects { get; } = new List<StatusEffectDataBuilder>();

        public List<CardUpgradeDataBuilder> CardUpgrades { get; } = new List<CardUpgradeDataBuilder>();

        public List<TraitDataBuilder> Traits { get; } = new List<TraitDataBuilder>();

        public List<KeywordDataBuilder> Keywords { get; } = new List<KeywordDataBuilder>();
        
        public List<ClassDataBuilder> Classes { get; } = new List<ClassDataBuilder>();

        public List<CampaignNodeTypeBuilder> CampaignNodeTypes { get; } = new List<CampaignNodeTypeBuilder>();

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
            Tribes.Setup(this);
            Hexes.Setup(this);
            Calls.Setup(this);
            GiftsOfTheMoon.Setup(this);
            GiftsOfOlympus.Setup(this);
            Eyes.Setup();

            SetupSpriteAssets();
            SetupVFX();

            preLoaded = true;
        }

        private void SetupVFX()
        {
            VFXHelper.VFX = new GIFLoader(this, this.ImagePath("Anim"));
            VFXHelper.VFX.RegisterAllAsApplyEffect();
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

        public override void Load()
        {
            if (!preLoaded)
            {
                CreateModAssets();
            }

            Events.OnEntityChosen += EntityChosen;
            Events.OnCampaignLoadPreset += CampaignLoad;
            Events.OnEntityCreated += Leaders.LeaderImagesFix;
            Events.OnCheckEntityDrag += ActionButton.DisableDrag;
            //Events.OnSceneChanged += CardsPhoto;

            base.Load();

            Tribes.AppendTribe(this);
        }

        public override void Unload()
        {
            preLoaded = false;

            Events.OnEntityChosen -= EntityChosen;
            Events.OnCampaignLoadPreset -= CampaignLoad;
            Events.OnEntityCreated -= Leaders.LeaderImagesFix;
            Events.OnCheckEntityDrag -= ActionButton.DisableDrag;

            GiftsOfTheMoon.Teardown();
            GiftsOfOlympus.Teardown();

            RemoveFromPools();
            base.Unload();
            Tribes.UnAppendTribe(this);
        }

        private void CampaignLoad(ref string[] preset)
        {
            GiftsOfTheMoon.InsertViaPreset(this, ref preset);
            GiftsOfOlympus.InsertViaPreset(this, ref preset);
        }

        private void EntityChosen(Entity entity)
        {
            Boons.GiveBoons(this, entity);
            Hexes.GiveHex(this, entity);
            Calls.GiveCall(this, entity);
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

        public void CardsPhoto(Scene scene)
        {
            if (scene.name == "Town")
            {
                References.instance.StartCoroutine(CardsPhoto2());
            }
        }

        private static IEnumerator CardsPhoto2()
        {
            // var everyGeneration = new[]
            // {
            //     "Ares", "Artemis", "Athena", "Aphrodite", "Apollo", "Demeter", 
            //     "Dionysus", "Hera", "Hermes", "Hestia", "Hephaestus", "Poseidon", "Zeus"
            // };
           // string[] everyGeneration = new string[] { "Frinos", "Toula" };
            string[] everyGeneration = new string[]
            {
                "WitchsStaff","SisterBlades", "MoonstoneAxe", "FrostbittenHorn", "Pom Slice", "Skelly", "Nectar",
                "IridescentFan", "ThunderSignet", "Ambrosia", "AdamantShard"
                , "UmbralFlames", "ArgentSkull", "BlackCoat"
            };
            yield return SceneManager.WaitUntilUnloaded("CardFramesUnlocked");
            yield return SceneManager.Load("CardFramesUnlocked", SceneType.Temporary);
            var sequence = Object.FindObjectOfType<CardFramesUnlockedSequence>();
            var titleObject = sequence.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
            titleObject.text = "New Items!";
            yield return sequence.StartCoroutine("CreateCards", everyGeneration.Select((string s) => $"bethanw10.hadesfrost.{s}").ToArray());
        }

        private void RemoveFromPools()
        {
            try
            {
                string[] poolsToCheck =
                {
                    "GeneralUnitPool",
                    "GeneralItemPool",
                    "GeneralCharmPool",
                    "GeneralModifierPool",
                    "SnowUnitPool",
                    "SnowItemPool",
                    "SnowCharmPool",
                    "BasicUnitPool",
                    "BasicItemPool",
                    "BasicCharmPool",
                    "MagicUnitPool",
                    "MagicItemPool",
                    "MagicCharmPool",
                    "ClunkUnitPool",
                    "ClunkItemPool",
                    "ClunkCharmPool",
                };
                foreach (var pool in poolsToCheck.Select(Extensions.GetRewardPool).Where(pool => pool != null))
                {
                    pool.list.RemoveAllWhere(l => l == null || l.ModAdded == this);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[bethan] error unloading " + e.Message + "\n" + e.StackTrace);
            }
        }
    }

    public class VFXHelper
    {
        public static GIFLoader VFX;
        public static SFXLoader SFX;
    }
}