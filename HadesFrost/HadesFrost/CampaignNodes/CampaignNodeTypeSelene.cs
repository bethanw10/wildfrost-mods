﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HadesFrost.CampaignNodes
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
            //Debug.Log("[hades]" + node);
            var objectOfType = FindObjectOfType<ItemEventRoutine>();
            //Debug.Log("[hades]" + objectOfType);

            objectOfType.node = node;
            yield return objectOfType.Populate();
        }
    
        // public override IEnumerator Run(CampaignNode node)
        // {
        //     yield return (object)Transition.To("Event");
        //    // AsyncOperationHandle<GameObject> task = this.routinePrefabRef.InstantiateAsync(EventManager.EventRoutineHolder);
        //    // yield return (object)new WaitUntil((Func<bool>)(() => task.IsDone));
        //    // EventRoutine eventRoutine = task.Result.GetComponent<EventRoutine>();
        //    // task.Result.AddComponent<AddressableReleaser>().Add((AsyncOperationHandle)task);
        //     var eventRoutine = FindObjectOfType<ItemEventRoutine>();
        //     Events.InvokeEventStart(node, eventRoutine);
        //     yield return (object)this.Populate(node);
        //     Events.InvokeEventPopulated(eventRoutine);
        //     Transition.End();
        //     yield return (object)eventRoutine.Run();
        //     yield return (object)Transition.To("MapNew");
        //     Transition.End();
        //     yield return (object)MapNew.CheckCompanionLimit();
        // }
    }
}