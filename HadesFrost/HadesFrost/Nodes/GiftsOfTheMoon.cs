using System.Collections.Generic;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Mechanics;
using HadesFrost.Utils;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;

namespace HadesFrost.Nodes
{
    public static class GiftsOfTheMoon
    {
        private static GameObject PrefabHolder;

        private const string SeleneEventLetter = "É";

        public static void Setup(HadesFrost mod)
        {
            PrefabHolder = new GameObject(mod.GUID);
            Object.DontDestroyOnLoad(PrefabHolder);
            PrefabHolder.SetActive(false);

            mod.CampaignNodeTypes.Add(new CampaignNodeTypeBuilder(mod)
                .Create<CampaignNodeTypeSelene>("SeleneNode")
                .WithZoneName("Selene")
                .WithCanEnter(true)
                .WithInteractable(true)
                .WithCanSkip(true)
                .WithCanLink(true) 
                .WithLetter(SeleneEventLetter)
                .SubscribeToAfterAllBuildEvent(
                    data =>
                    {
                        var castData = (CampaignNodeTypeSelene)data;
                        var item = mod.TryGet<CampaignNodeType>("CampaignNodeItem");
                        castData.routinePrefabRef = ((CampaignNodeTypeItem)item).routinePrefabRef;

                        castData.Pool = new List<CardData>
                        {
                            mod.TryGet<CardData>("LunarRay"),
                            mod.TryGet<CardData>("PhaseShift"),
                            mod.TryGet<CardData>("MoonWater"),
                            mod.TryGet<CardData>("WolfHowl"),
                            mod.TryGet<CardData>("DarkSide"),
                            mod.TryGet<CardData>("TwilightCurse"),
                            mod.TryGet<CardData>("NightBloom"),
                            mod.TryGet<CardData>("TotalEclipse"),
                            mod.TryGet<CardData>("SkyFall"),
                        };

                        var mapNode = mod.TryGet<CampaignNodeType>("CampaignNodeGold").mapNodePrefab.InstantiateKeepName();
                        mapNode.name = mod.GUID + ".Selene";                                    
                        data.mapNodePrefab = mapNode;

                        var uiText = LocalizationHelper.GetCollection("UI Text", SystemLanguage.English);
                        var key = mapNode.name + "Ribbon";
                        uiText.SetString(key, "Gifts of the Moon");    //Define the Localized string for our ribbon title.
                        mapNode.label.GetComponentInChildren<LocalizeStringEvent>().StringReference = uiText.GetString(key);
                        //Find the LocalizeStringEvent and set it to our own.

                        //The game will randomly pick between the options available. They will also pick the same index for both sprites and cleared sprites, if possible.
                        mapNode.spriteOptions = new[] { ScaledSprite(mod, "portalClosed.png", 200), ScaledSprite(mod, "PortalClosed.png", 200) };
                        mapNode.clearedSpriteOptions = new[] { ScaledSprite(mod, "portal.png", 200), ScaledSprite(mod, "portal.png", 200) };
                        //I am using 360x274px images, but setting pixelsPerUnit to 200 scales it down to 180x137px.

                        var nodeObject = mapNode.gameObject;
                        nodeObject.transform.SetParent(PrefabHolder.transform);
                    })
            );
        }

        public static void Teardown()
        {
            PrefabHolder.Destroy();
        }

        public static void InsertViaPreset(HadesFrost mod, ref string[] preset)
        {
            if (References.PlayerData?.classData?.ModAdded?.GUID != mod.GUID ||
                References.PlayerData?.classData?.name != Extensions.PrefixGUID(Tribes.MELINOE_TRIBE, mod))
            {
                return;
            }

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
                        preset[0] = preset[0].Insert(i + 1, SeleneEventLetter);
                        for (var j = 1; j < preset.Length; j++)
                        {
                            preset[j] = preset[j].Insert(i + 1, preset[j][i].ToString());
                        }
                        break;
                    }
                }
            }
        }

        public static void InsertSeleneViaSpecialEvent(HadesFrost mod, Scene scene)
        {
            if (scene.name == "Campaign")
            {
                var specialEvents = Object.FindObjectOfType<SpecialEventsSystem>(); //Only 1 of these exists
                var eve = new SpecialEventsSystem.Event
                {
                    requiresUnlock = null,
                    nodeType = mod.TryGet<CampaignNodeType>("SeleneNode"),
                    replaceNodeTypes = new[] { "CampaignNodeReward" },
                    minTier = 3,                                         //After the first boss
                    perTier = new Vector2Int(0, 1),                            //Maximum of 2 per tier
                    perRun = new Vector2Int(1, 1)                              //Between 2 and 4 Selenes per run
                };
                specialEvents.events = specialEvents.events.AddItem(eve).ToArray();
            }
        }

        private static Sprite ScaledSprite(WildfrostMod mod, string fileName, int pixelsPerUnit = 100)
        {
            var tex = mod.ImagePath(fileName).ToTex();
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, (20f * pixelsPerUnit) / (tex.height * 100f)), pixelsPerUnit);
        }
    }
}