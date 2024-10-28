using System.Collections;
using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

namespace HadesFrost
{
    public static class StatusExt
    {
        public static StringTable Collection => LocalizationHelper.GetCollection("Card Text", SystemLanguage.English);
        public static StringTable KeyCollection => LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);

        public static GameObject CreateIcon(this WildfrostMod mod, string name, Sprite sprite, string type, string copyTextFrom, Color textColor, KeywordData[] keys, int posX = 1)
        {
            GameObject gameObject = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
            StatusIcon icon = gameObject.AddComponent<StatusIconExt>();
            Dictionary<string, GameObject> cardIcons = CardManager.cardIcons;
            if (!copyTextFrom.IsNullOrEmpty())
            {
                GameObject text = cardIcons[copyTextFrom].GetComponentInChildren<TextMeshProUGUI>().gameObject.InstantiateKeepName();
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
            UnityEngine.UI.Image image = gameObject.AddComponent<UnityEngine.UI.Image>();
            image.sprite = sprite;
            CardHover cardHover = gameObject.AddComponent<CardHover>();
            cardHover.enabled = false;
            cardHover.IsMaster = false;
            CardPopUpTarget cardPopUp = gameObject.AddComponent<CardPopUpTarget>();
            cardPopUp.keywords = keys;
            cardPopUp.posX = posX;
            cardHover.pop = cardPopUp;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta *= 0.01f;
            gameObject.SetActive(true);
            icon.type = type;
            cardIcons[type] = gameObject;

            return gameObject;
        }

        public static KeywordData CreateIconKeyword(this WildfrostMod mod, string name, string title, string desc, string icon, bool useSmallPanel = false)
        {
            KeywordData data = ScriptableObject.CreateInstance<KeywordData>();
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

        public class StatusIconExt : StatusIcon
        {
            public ButtonAnimator animator;
            public ButtonExt button;
            private IStatusToken effectToken;

            public override void Assign(Entity entity)
            {
                base.Assign(entity);
                SetText();
                onValueDown.AddListener(delegate { Ping(); });
                onValueUp.AddListener(delegate { Ping(); });
                afterUpdate.AddListener(SetText);
                onValueDown.AddListener(CheckDestroy);

                StatusEffectData effect = entity.FindStatus(type);
                if (effect is IStatusToken effect2)
                {
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
            internal StatusIconExt Icon => GetComponent<StatusIconExt>();

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
            void ButtonCreate(StatusIconExt icon);

            void RunButtonClicked();

            IEnumerator ButtonClicked();


        }

        public static T Register<T>(this T status, WildfrostMod mod, List<StatusEffectData> statuses) where T : StatusEffectData
        {
            status.ModAdded = mod;
            statuses.Add(status);
            AddressableLoader.AddToGroup<StatusEffectData>("StatusEffectData", status);
            return status;
        }
    }
}