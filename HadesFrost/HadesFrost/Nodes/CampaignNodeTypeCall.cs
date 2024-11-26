using System.Collections;
using System.Collections.Generic;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;
using UnityEngine.Localization;

namespace HadesFrost.Nodes
{
    public class CampaignNodeTypeCall : CampaignNodeTypeItem
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
            var prompts = new[] { "In the name of Hades...", "Olympus, I accept this message!", "Hear me, on my authority."};

            var objectOfType = FindObjectOfType<ItemEventRoutine>();

            objectOfType.node = node;
            var collection = LocalizationHelper.GetCollection("Card Text", new LocaleIdentifier(SystemLanguage.English));
            collection.SetString("Call_text", prompts.RandomItem());
            objectOfType.openKey = collection.GetString("Call_text");

            yield return objectOfType.Populate();
        }
    }
}