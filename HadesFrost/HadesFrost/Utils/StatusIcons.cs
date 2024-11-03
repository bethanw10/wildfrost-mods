using System.Collections;
using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

namespace HadesFrost.Utils
{
    public static class StatusIcons
    {
        public static StringTable Collection => LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
        public static StringTable KeyCollection => LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);

        public static GameObject CreateIcon(this WildfrostMod mod, string name, Sprite sprite, string type, string copyTextFrom, Color textColor, KeywordData[] keys, int posX = 1)
        {
            var gameObject = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
            StatusIcon icon = gameObject.AddComponent<HadesStatusIcon>();
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
            var image = gameObject.AddComponent<UnityEngine.UI.Image>();
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
            GameObject.DontDestroyOnLoad(pokefrostUI);

            gameObject.transform.SetParent(pokefrostUI.transform);
            gameObject.SetActive(false);
            var icon = gameObject.AddComponent<HadesStatusIcon>();
            var cardIcons = CardManager.cardIcons;
            icon.animator = gameObject.AddComponent<ButtonAnimator>();
            icon.button = gameObject.AddComponent<ButtonExt>();
            icon.animator.button = icon.button;
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

        public class HadesStatusIcon : StatusIcon
        {
            public ButtonAnimator animator;
            public ButtonExt button;
            private IStatusToken effectToken;

            public override void Assign(Entity entity)
            {
                Common.Log("assign");

                base.Assign(entity);
                SetText();
                onValueDown.AddListener(delegate { Ping(); });
                onValueUp.AddListener(delegate { Ping(); });
                afterUpdate.AddListener(SetText);
                onValueDown.AddListener(CheckDestroy);

                var effect = entity.FindStatus(type);
                Common.Log("is token?");

                if (effect is IStatusToken effect2)
                {
                    Common.Log("adding listener");
                    effectToken = effect2;
                    effect2.ButtonCreate(this);
                    button.onClick.AddListener(effectToken.RunButtonClicked);
                    onDestroy.AddListener(DisableDragBlocker);
                }
            }

            public void DisableDragBlocker()
            {
                button.DisableDragBlocking();
            }
        }

        public class ButtonExt : Button
        {
            internal HadesStatusIcon Icon => GetComponent<HadesStatusIcon>();

            internal static ButtonExt dragBlocker = null;

            internal Entity Entity => Icon?.target;

            public override void OnPointerEnter(PointerEventData eventData)
            {
                dragBlocker = this;
            }

            public override void OnPointerExit(PointerEventData eventData)
            {
                DisableDragBlocking();
            }

            public void DisableDragBlocking()
            {
                if (dragBlocker == this)
                {
                    dragBlocker = null;
                }
            }

            public static void DisableDrag(ref Entity arg0, ref bool arg1)
            {
                if (dragBlocker == null || arg0 != dragBlocker.Entity)
                {
                    return;
                }
                arg1 = false;
            }
        }

        public interface IStatusToken
        {
            void ButtonCreate(HadesStatusIcon icon);

            void RunButtonClicked();

            IEnumerator ButtonClicked();
        }
    }
}