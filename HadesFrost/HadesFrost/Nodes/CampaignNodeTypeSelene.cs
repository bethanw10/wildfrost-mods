using System.Collections;
using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;
using UnityEngine.Localization;

namespace HadesFrost.Nodes
{
    public class CampaignNodeTypeSelene : CampaignNodeTypeItem
    {
        // [SerializeField] 
        // private int choices = 3;
    
        // [SerializeField]
        // public List<CardData> force;

        [SerializeField]
        public List<CardData> Pool;

        public override IEnumerator SetUp(CampaignNode node)
        {
            yield return null;

            var component = References.Player.GetComponent<CharacterRewards>();

            var randomChoices = Pool.RandomItems(choices);

            var cardDataList = randomChoices.ToList().Clone();
            if (cardDataList.Count > 0)
            {
                component.PullOut("Items", cardDataList);
            }
    
            // var itemCount = choices - cardDataList.Count;
            // cardDataList.AddRange(component.Pull<CardData>(node, "Items", itemCount));
            node.data = new Dictionary<string, object>
            {
                {
                    "open",
                    false
                },
                {
                    "cards",
                    cardDataList.ToSaveCollectionOfNames()
                }
            };
        }
    
        public override bool HasMissingData(CampaignNode node) => MissingCardSystem.HasMissingData(node.data.GetSaveCollection<string>("cards"));
    
        public override IEnumerator Populate(CampaignNode node)
        {
            var prompts = new[] { "Please make your choice.", "The choice is yours.", "Which suits you?", "Your choice?", "Behold my light." };

            var objectOfType = FindObjectOfType<ItemEventRoutine>();

            objectOfType.node = node;
            var collection = LocalizationHelper.GetCollection("Card Text", new LocaleIdentifier(SystemLanguage.English));
            collection.SetString("Selene_text", prompts.RandomItem());
            objectOfType.chooseKey = collection.GetString("Selene_text");

            yield return objectOfType.Populate();
        }
    }
}