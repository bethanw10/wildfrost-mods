using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

internal class StatusEffectInstantMorphCard : StatusEffectInstant
{
    [Serializable]
    public struct Combo
    {
        public string CardName;

        public string ResultingCardName;

        public bool AllCardsInDeck(List<Entity> deck)
        {
            return HasCard(CardName, deck);
        }

        public Entity FindCard(List<Entity> deck)
        {
            foreach (var item in deck)
            {
                if (item.data.name == CardName)
                {
                    return item;
                }
            }

            return null;
        }

        public bool HasCard(string cardName, List<Entity> deck)
        {
            foreach (var item in deck)
            {
                if (item.data.name == cardName)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [SerializeField]
    public string CombineSceneName = "CardCombine";

    public string CardName;

    public string ResultingCardName;

    public bool CheckHand = true;
    public bool CheckDeck = true;
    public bool CheckBoard = true;

    public bool KeepUpgrades = true;
    public List<CardUpgradeData> ExtraUpgrades;

    public bool SpawnOnBoard = false;

    public override IEnumerator Process()
    {
        var combo = new Combo
        {
            CardName = target.name,
            ResultingCardName = ResultingCardName
        };

        var fullDeck = new List<Entity>();
        if (CheckHand)
        {
            fullDeck.AddRange(References.Player.handContainer.ToList());
        }
        if (CheckDeck)
        {
            fullDeck.AddRange(References.Player.drawContainer.ToList());
            fullDeck.AddRange(References.Player.discardContainer.ToList());
        }
        if (CheckBoard)
        {
            fullDeck.AddRange(Battle.GetCardsOnBoard(References.Player).ToList());
            fullDeck.AddRange(Battle.GetCardsOnBoard(References.Battle?.enemy).ToList());
        }

        if (combo.AllCardsInDeck(fullDeck))
        {
            var action = new MorphAction(KeepUpgrades, ExtraUpgrades, SpawnOnBoard, target.containers[0])
                {
                    CombineSceneName = CombineSceneName,
                    ToFuse = combo.FindCard(fullDeck),
                    Combo = combo
                };

            var queueAction = true;
            foreach (var playAction in ActionQueue.instance.queue)
            {
                if (playAction.GetType() == action.GetType())
                {
                    queueAction = false;
                    break;
                }
            }

            if (queueAction)
            {
                ActionQueue.Stack(action);
            }
        }

        yield return base.Process();
    }

    public class MorphAction : PlayAction
    {
        [SerializeField]
        public string CombineSceneName;

        public Combo Combo;

        public Entity ToFuse;

        public bool KeepUpgrades;

        public List<CardUpgradeData> ExtraUpgrades;

        public bool SpawnOnBoard;

        public CardContainer Row;

        public MorphAction(bool keepUpgrades, List<CardUpgradeData> extraUpgrades, bool spawnOnBoard, CardContainer row)
        {
            this.KeepUpgrades = keepUpgrades;
            this.ExtraUpgrades = extraUpgrades;
            this.SpawnOnBoard = spawnOnBoard;
            this.Row = row;
        }

        public override IEnumerator Run()
        {
            return CombineSequence(Combo, ToFuse);
        }

        public IEnumerator CombineSequence(Combo combo, Entity toFuse)
        {
            CombineCardSequence combineSequence = null;
            yield return SceneManager.Load(CombineSceneName, SceneType.Temporary, delegate (Scene scene)
            {
                combineSequence = scene.FindObjectOfType<CombineCardSequence>();
            });
            if ((bool)combineSequence)
            {
                yield return combineSequence.Run(toFuse, combo.ResultingCardName, KeepUpgrades, ExtraUpgrades, SpawnOnBoard, Row);
            }

            yield return SceneManager.Unload(CombineSceneName);
        }
    }
}