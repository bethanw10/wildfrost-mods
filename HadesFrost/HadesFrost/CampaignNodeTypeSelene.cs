using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Debug = UnityEngine.Debug;

public class CampaignNodeTypeSelene : CampaignNodeTypeItem
{
    [SerializeField]
    public int choices = 3;
    
    [SerializeField]
    public List<CardData> force;
    
    public CampaignNodeTypeEvent itemNode;
    
    
    public AssetReferenceGameObject prefab;
    
    
    public override IEnumerator SetUp(CampaignNode node)
    {
        yield return (object)null;
        CharacterRewards component = References.Player.GetComponent<CharacterRewards>();
        List<CardData> cardDataList = this.force.Clone<CardData>();
        if (cardDataList.Count > 0)
        {
            component.PullOut("Items", cardDataList);
        }
    
        int itemCount = this.choices - cardDataList.Count;
        cardDataList.AddRange(component.Pull<CardData>((object)node, "Items", itemCount));
        node.data = new Dictionary<string, object>()
        {
            {
                "open",
                (object) false
            },
            {
                "cards",
                (object) cardDataList.ToSaveCollectionOfNames<CardData>()
            }
        };
    }
    
    public override bool HasMissingData(CampaignNode node) => MissingCardSystem.HasMissingData(node.data.GetSaveCollection<string>("cards"));
    
    public override IEnumerator Populate(CampaignNode node)
    {
        Debug.Log("[hades]" + node);
        var objectOfType = FindObjectOfType<ItemEventRoutine>();
        Debug.Log("[hades]" + objectOfType);

        objectOfType.node = node;
        yield return (object)objectOfType.Populate();
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