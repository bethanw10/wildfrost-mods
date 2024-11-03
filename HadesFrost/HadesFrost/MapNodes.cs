using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Statuses;
using HadesFrost.Utils;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace HadesFrost
{
    public static class MapNodes
    {
        private static GameObject PrefabHolder;

        public const string SeleneEventLetter = "É";

        public static void Setup(HadesFrost mod)
        {
            PrefabHolder = new GameObject(mod.GUID);
            Object.DontDestroyOnLoad(PrefabHolder);
            PrefabHolder.SetActive(false);

            mod.CampaignNodes.Add(new CampaignNodeTypeBuilder(mod)
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

                        castData.pool = new List<CardData>
                        {
                            mod.TryGet<CardData>("Nectar"),
                            mod.TryGet<CardData>("Ambrosia"),
                            mod.TryGet<CardData>("LunarRay")
                        };

                        //Inside the SubscribeToAfterAllBuildEvent
                        //Some MapNode stuff
                        var mapNode = mod.TryGet<CampaignNodeType>("CampaignNodeGold").mapNodePrefab.InstantiateKeepName(); //There's a lot of things in one of these prefabs
                        mapNode.name = mod.GUID + ".Selene";               //Changing the name                                                   
                        data.mapNodePrefab = mapNode;                  //And assign it to our node type before we forget.

                        var uiText = LocalizationHelper.GetCollection("UI Text", SystemLanguage.English);
                        var key = mapNode.name + "Ribbon";
                        uiText.SetString(key, "Gifts of the Moon");    //Define the Localized string for our ribbon title.
                        mapNode.label.GetComponentInChildren<LocalizeStringEvent>().StringReference = uiText.GetString(key);
                        //Find the LocalizeStringEvent and set it to our own.

                        //The game will randomly pick between the options available. They will also pick the same index for both sprites and cleared sprites, if possible.
                        mapNode.spriteOptions = new[] { ScaledSprite(mod, "portalClosed.png", 200), ScaledSprite(mod, "PortalClosed.png", 200) };
                        mapNode.clearedSpriteOptions = new[] { ScaledSprite(mod, "portal.png", 200), ScaledSprite(mod, "portal.png", 200) };
                        //I am using 360x274px images, but setting pixelsPerUnit to 200 scales it down to 180x137px.

                        var nodeObject = mapNode.gameObject;             //MapNode is a MonoBehaviour, so there is an underlying GameObject.
                        nodeObject.transform.SetParent(PrefabHolder.transform); //Ensures your reference doesn't poof out of existence.
                    })
            );
        }

        public static void Teardown()
        {
            PrefabHolder.Destroy();
        }

        private static Sprite ScaledSprite(HadesFrost mod, string fileName, int pixelsPerUnit = 100)
        {
            var tex = mod.ImagePath(fileName).ToTex();
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, (20f * pixelsPerUnit) / (tex.height * 100f)), pixelsPerUnit);
        }
    }
}