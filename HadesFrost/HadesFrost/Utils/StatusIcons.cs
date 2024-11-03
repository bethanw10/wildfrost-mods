﻿using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.ButtonStatuses;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

namespace HadesFrost.Utils
{
    public static class StatusIcons
    {
        public static StringTable Collection => LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);

        private static StringTable KeyCollection => LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);

        public static GameObject CreateIcon(string name, Sprite sprite, string type, string copyTextFrom, Color textColor, KeywordData[] keys, int posX = 1)
        {
            var gameObject = new GameObject(name);
            Object.DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
            StatusIcon icon = gameObject.AddComponent<HexStatusIcon>();
            var cardIcons = CardManager.cardIcons;
            if (!copyTextFrom.IsNullOrEmpty())
            {
                var text = cardIcons[copyTextFrom].GetComponentInChildren<TextMeshProUGUI>().gameObject.InstantiateKeepName();
                text.transform.SetParent(gameObject.transform);
                icon.textElement = text.GetComponent<TextMeshProUGUI>();
                icon.textColour = textColor;
                icon.textColourAboveMax = textColor;
                icon.textColourBelowMax = textColor;
            }
            icon.onCreate = new UnityEngine.Events.UnityEvent();
            icon.onDestroy = new UnityEngine.Events.UnityEvent();
            icon.onValueDown = new UnityEventStatStat();
            icon.onValueUp = new UnityEventStatStat();
            icon.afterUpdate = new UnityEngine.Events.UnityEvent();
            var image = gameObject.AddComponent<Image>();
            image.sprite = sprite;
            var cardHover = gameObject.AddComponent<CardHover>();
            cardHover.enabled = false;
            cardHover.IsMaster = false;
            var cardPopUp = gameObject.AddComponent<CardPopUpTarget>();
            cardPopUp.keywords = keys;
            cardPopUp.posX = posX;
            cardHover.pop = cardPopUp;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta *= 0.01f;
            gameObject.SetActive(true);
            icon.type = type;
            cardIcons[type] = gameObject;

            return gameObject;
        }

        public static GameObject CreateButtonIcon(this WildfrostMod mod, string name, Sprite sprite, string type, string copyTextFrom, Color textColor, KeywordData[] keys)
        {
            var gameObject = new GameObject(name);

            var pokefrostUI = new GameObject("HadesUI");
            pokefrostUI.SetActive(false);
            Object.DontDestroyOnLoad(pokefrostUI);

            gameObject.transform.SetParent(pokefrostUI.transform);
            gameObject.SetActive(false);
            var icon = gameObject.AddComponent<HexStatusIcon>();
            var cardIcons = CardManager.cardIcons;
            icon.Animator = gameObject.AddComponent<ButtonAnimator>();
            icon.HexButton = gameObject.AddComponent<HexButton>();
            icon.Animator.button = icon.HexButton;
            if (!copyTextFrom.IsNullOrEmpty())
            {
                var text = cardIcons[copyTextFrom].GetComponentInChildren<TextMeshProUGUI>().gameObject.InstantiateKeepName();
                text.transform.SetParent(gameObject.transform);
                icon.textElement = text.GetComponent<TextMeshProUGUI>();
                icon.textColour = textColor;
                icon.textColourAboveMax = textColor;
                icon.textColourBelowMax = textColor;
            }
            icon.onCreate = new UnityEngine.Events.UnityEvent();
            icon.onDestroy = new UnityEngine.Events.UnityEvent();
            icon.onValueDown = new UnityEventStatStat();
            icon.onValueUp = new UnityEventStatStat();
            icon.afterUpdate = new UnityEngine.Events.UnityEvent();
            var image = gameObject.AddComponent<Image>();
            image.sprite = sprite;
            var cardHover = gameObject.AddComponent<CardHover>();
            cardHover.enabled = false;
            cardHover.IsMaster = false;
            var cardPopUp = gameObject.AddComponent<CardPopUpTarget>();
            cardPopUp.keywords = keys;
            cardHover.pop = cardPopUp;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta *= 0.008f;
            gameObject.SetActive(true);
            icon.type = type;
            cardIcons[type] = gameObject;
            gameObject.AddComponent<UINavigationItem>();

            return gameObject;
        }

        public static KeywordData CreateIconKeyword(this WildfrostMod mod, string name, string title, string desc, string icon, bool useSmallPanel = false)
        {
            var data = ScriptableObject.CreateInstance<KeywordData>();
            data.name = name;
            KeyCollection.SetString(data.name + "_text", title);
            data.titleKey = KeyCollection.GetString(data.name + "_text");
            KeyCollection.SetString(data.name + "_desc", desc);
            data.descKey = KeyCollection.GetString(data.name + "_desc");
            data.showIcon = true;
            data.showName = false;
            data.iconName = icon;
            data.ModAdded = mod;
            //data.panelSprite = useSmallPanel ? panelSmall : panel;
            data.panelColor = new Color(0.15f, 0.15f, 0.15f, 0.90f);
            AddressableLoader.AddToGroup("KeywordData", data);
            return data;
        }

        public static KeywordData ChangeColor(this KeywordData data, Color? title = null, Color? body = null, Color? note = null)
        {
            if (title is Color c1) { data.titleColour = c1; }
            if (body is Color c2) { data.bodyColour = c2; }
            if (note is Color c3) { data.noteColour = c3; }
            return data;
        }
    }
}