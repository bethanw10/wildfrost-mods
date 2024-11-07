using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.CampaignNodeTypes;
using HadesFrost.Utils;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace HadesFrost.Setup
{
    public static class GiftsOfTheMoon
    {
        private static GameObject PrefabHolder;

        public const string SeleneEventLetter = "É";

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
                    (data) =>
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

        private static Sprite ScaledSprite(WildfrostMod mod, string fileName, int pixelsPerUnit = 100)
        {
            var tex = mod.ImagePath(fileName).ToTex();
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, (20f * pixelsPerUnit) / (tex.height * 100f)), pixelsPerUnit);
        }
    }
}