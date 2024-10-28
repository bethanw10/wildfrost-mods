using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WildfrostHopeMod.Utils;
using Object = UnityEngine.Object;

/* TODO:
Athena, Dionysus
Rework apollo
Card images
Pets
Items - keepsakes, ambrosia, weapons etc.
Charms
Hitch
Jolted anim
Hades Child trait
Create tribe + Tribe banner
Selene event ? 
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

        public override string[] Depends => Array.Empty<string>();

        public override string Title => "Hades Frost";

        public override string Description => ":)";

        public List<CardDataBuilder> Cards { get; } = new List<CardDataBuilder>();

        public List<StatusEffectDataBuilder> StatusEffects { get; } = new List<StatusEffectDataBuilder>();

        public List<CardUpgradeDataBuilder> CardUpgrades { get; } = new List<CardUpgradeDataBuilder>();

        public List<TraitDataBuilder> Traits { get; } = new List<TraitDataBuilder>();

        public List<KeywordDataBuilder> Keywords { get; } = new List<KeywordDataBuilder>();

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
            var everyGeneration = new string[] { "Apollo", "Artemis", "Ares", "Demeter", "Hera", "Hermes", "Hestia", "Poseidon", "Zeus" };
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

            SpriteAssetsFix();

            preLoaded = true;
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

        public override void Load()
        {
            if (!preLoaded)
            {
                CreateModAssets();
            }

            // Events.OnSceneChanged += CardsPhoto;
            Events.OnEntityChosen += EntityChosen;

            base.Load();
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
                default:
                    return null;
            }
        }
    }
}