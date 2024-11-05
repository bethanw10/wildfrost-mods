using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

internal class StatusEffectInstantCombineCard : StatusEffectInstant
{
    [Serializable]
    public struct Combo
    {
        public string[] CardNames;

        public string ResultingCardName;

        public bool AllCardsInDeck(List<Entity> deck)
        {
            var result = true;
            var array = CardNames;
            foreach (var cardName in array)
            {
                if (!HasCard(cardName, deck))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public List<Entity> FindCards(List<Entity> deck)
        {
            var tooFuse = new List<Entity>();
            var array = CardNames;
            foreach (var cardName in array)
            {
                foreach (var item in deck)
                {
                    if (item.data.name == cardName)
                    {
                        tooFuse.Add(item);
                        break;
                    }
                }
            }

            return tooFuse;
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

    public string[] CardNames;

    public string ResultingCardName;

    public bool CheckHand = true;
    public bool CheckDeck = true;
    public bool CheckBoard = true;

    public bool KeepUpgrades = true;
    public List<CardUpgradeData> ExtraUpgrades;

    public bool SpawnOnBoard = false;

    public bool ChangeDeck = false;

    public override IEnumerator Process()
    {
        CardNames = new[] { target.name };
        var combo = new Combo
        {
            CardNames = CardNames,
            ResultingCardName = ResultingCardName
        };

        var fulldeck = new List<Entity>();
        if (CheckHand)
        {
            fulldeck.AddRange(References.Player.handContainer.ToList());
        }
        if (CheckDeck)
        {
            fulldeck.AddRange(References.Player.drawContainer.ToList());
            fulldeck.AddRange(References.Player.discardContainer.ToList());
        }
        if (CheckBoard)
        {
            fulldeck.AddRange(Battle.GetCardsOnBoard(References.Player).ToList());
            fulldeck.AddRange(Battle.GetCardsOnBoard(References.Battle?.enemy).ToList());
        }

        if (combo.AllCardsInDeck(fulldeck))
        {
            var action = new CombineAction(KeepUpgrades, ExtraUpgrades, SpawnOnBoard, target.containers[0])
                {
                    CombineSceneName = CombineSceneName,
                    ToFuse = combo.FindCards(fulldeck),
                    Combo = combo
                };

            if (ChangeDeck)
            {
                EditDeck(combo.CardNames, combo.ResultingCardName);
            }

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

    public void EditDeck(string[] cardsToRemove, string cardToAdd)
    {
        var oldCards = new List<CardData>();

        foreach (var name in cardsToRemove)
        {
            foreach (var card in References.Player.data.inventory.deck)
            {
                if (card.name == name && !oldCards.Contains(card))
                {
                    oldCards.Add(card);
                    break;
                }
            }
        }

        if (oldCards.Count == cardsToRemove.Length)
        {
            var upgrades = new List<CardUpgradeData> { };

            foreach (var card in oldCards)
            {
                if (KeepUpgrades)
                {
                    upgrades.AddRange(card.upgrades.Select(u => u.Clone()));
                }

                References.Player.data.inventory.deck.Remove(card);
            }

            var cardDataClone = AddressableLoader.GetCardDataClone(cardToAdd);

            upgrades.AddRange(ExtraUpgrades.Select(u => u.Clone()));

            foreach (var upgrade in upgrades)
            {
                upgrade.Assign(cardDataClone);
            }

            References.Player.data.inventory.deck.Add(cardDataClone);
        }
    }

    public class CombineAction : PlayAction
    {
        [SerializeField]
        public string CombineSceneName;

        public Combo Combo;

        public List<Entity> ToFuse;

        public bool KeepUpgrades;

        public List<CardUpgradeData> ExtraUpgrades;

        public bool SpawnOnBoard;

        public CardContainer Row;

        public CombineAction(bool keepUpgrades, List<CardUpgradeData> extraUpgrades, bool spawnOnBoard, CardContainer row)
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

        public IEnumerator CombineSequence(Combo combo, List<Entity> tooFuse)
        {
            CombineCardSequence combineSequence = null;
            yield return SceneManager.Load(CombineSceneName, SceneType.Temporary, delegate (Scene scene)
            {
                combineSequence = scene.FindObjectOfType<CombineCardSequence>();
            });
            if ((bool)combineSequence)
            {
                yield return combineSequence.Run(tooFuse, combo.ResultingCardName, KeepUpgrades, ExtraUpgrades, SpawnOnBoard, Row);
            }

            yield return SceneManager.Unload(CombineSceneName);
        }
    }
}

public static class CombineCardSequenceExtension
{
    public static IEnumerator Run(this CombineCardSequence seq, List<Entity> cardsToCombine, string resultingCard, bool keepUpgrades, List<CardUpgradeData> extraUpgrades, bool spawnOnBoard, CardContainer row)
    {
        var cardDataClone = AddressableLoader.GetCardDataClone(resultingCard);

        var upgrades = new List<CardUpgradeData>();
        if (keepUpgrades)
        {
            foreach (var ent in cardsToCombine)
            {
                upgrades.AddRange(ent.data.upgrades.Select(u => u.Clone()));
            }
        }
        upgrades.AddRange(extraUpgrades.Select(u => u.Clone()));

        foreach (var upgrade in upgrades)
        {
            upgrade.Assign(cardDataClone);
        }


        yield return Run(seq, cardsToCombine.ToArray(), cardDataClone, spawnOnBoard, row);
    }

    public static IEnumerator Run(this CombineCardSequence seq, Entity[] entities, CardData finalCard, bool spawnOnBoard, CardContainer row)
    {
        PauseMenu.Block();
        var card = CardManager.Get(finalCard, Battle.instance.playerCardController, References.Player, inPlay: false, isPlayerCard: true);
        card.transform.localScale = Vector3.one * 1f;
        card.transform.SetParent(seq.finalEntityParent);
        var finalEntity = card.entity;
        var clump = new Routine.Clump();
        var array = entities;
        foreach (var entity in array)
        {
            clump.Add(entity.display.UpdateData());
        }

        clump.Add(finalEntity.display.UpdateData());
        clump.Add(Sequences.Wait(0.5f));
        yield return clump.WaitForEnd();

        array = entities;
        foreach (var entity2 in array)
        {
            entity2.RemoveFromContainers();
        }

        array = entities;
        foreach (var entity in array)
        {
            entity.transform.localScale = Vector3.one * 0.8f;
        }

        seq.fader.In();
        var zero = Vector3.zero;
        array = entities;
        foreach (var entity3 in array)
        {
            zero += entity3.transform.position;
        }

        zero /= (float)entities.Length;

        seq.group.position = zero;
        array = entities;
        foreach (var entity4 in array)
        {
            var transform = UnityEngine.Object.Instantiate(seq.pointPrefab, entity4.transform.position, Quaternion.identity, seq.group);
            transform.gameObject.SetActive(value: true);
            entity4.transform.SetParent(transform);
            entity4.flipper.FlipUp();
            seq.points.Add(transform);
            LeanTween.alphaCanvas(((Card)entity4.display).canvasGroup, 1f, 0.4f).setEaseInQuad();
        }

        foreach (var point in seq.points)
        {
            LeanTween.moveLocal(to: point.localPosition.normalized, gameObject: point.gameObject, time: 0.4f).setEaseInQuart();
        }

        yield return new WaitForSeconds(0.4f);

        Events.InvokeScreenShake(1f, 0f);
        array = entities;
        foreach (var ent in array)
        {
            ent.wobbler.WobbleRandom();
        }

        foreach (var point2 in seq.points)
        {
            LeanTween.moveLocal(to: point2.localPosition.normalized * 3f, gameObject: point2.gameObject, time: 1f).setEase(seq.bounceCurve);
        }

        LeanTween.moveLocal(seq.group.gameObject, new Vector3(0f, 0f, -2f), 1f).setEaseInOutQuad();
        LeanTween.rotateZ(seq.group.gameObject, Dead.PettyRandom.Range(160f, 180f), 1f).setOnUpdateVector3(delegate
        {
            foreach (var point3 in seq.points)
            {
                point3.transform.eulerAngles = Vector3.zero;
            }
        }).setEaseInOutQuad();
        yield return new WaitForSeconds(1f);

        Events.InvokeScreenShake(1f, 0f);
        if ((bool)seq.ps)
        {
            seq.ps.Play();
        }

        seq.combinedFx.SetActive(value: true);

        finalEntity.transform.position = Vector3.zero;
        array = entities;
        foreach (var ent in array)
        {
            CardManager.ReturnToPool(ent);
        }

        seq.group.transform.localRotation = Quaternion.identity;
        finalEntity.curveAnimator.Ping();
        finalEntity.wobbler.WobbleRandom();

        yield return new WaitForSeconds(1f);

        seq.fader.gameObject.Destroy();
        PauseMenu.Unblock();

        var flag = true;
        if (spawnOnBoard)
        {
            if (row.Count != 3)
            {
                yield return Sequences.CardMove(finalEntity, new CardContainer[1] { row });
                finalEntity.inPlay = true;
                flag = false;
            }

            if (flag)
            {
                for (var i = 0; i < 2; i++)
                {
                    row = Battle.instance.GetRow(References.Player, i);
                    if (row.Count != 3)
                    {

                        yield return Sequences.CardMove(finalEntity, new CardContainer[1] { row });
                        finalEntity.inPlay = true;
                        flag = false;

                        break;
                    }
                }
            }
        }

        if (flag)
        {
            yield return Sequences.CardMove(finalEntity, new CardContainer[1] { References.Player.handContainer });
            finalEntity.inPlay = true;
        }

        References.Player.handContainer.TweenChildPositions();
        ActionQueue.Stack(new ActionReveal(finalEntity));
    }
}