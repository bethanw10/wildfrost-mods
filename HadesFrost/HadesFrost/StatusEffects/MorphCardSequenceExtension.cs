using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HadesFrost.Utils;
using UnityEngine;
using WildfrostHopeMod.Utils;

public static class MorphCardSequenceExtension
{
    public static IEnumerator Run(this CombineCardSequence seq, Entity cardToCombine, string resultingCard, bool keepUpgrades, List<CardUpgradeData> extraUpgrades, bool spawnOnBoard, CardContainer row)
    {
        var cardDataClone = AddressableLoader.GetCardDataClone(resultingCard);

        var upgrades = new List<CardUpgradeData>();
        if (keepUpgrades)
        {
            upgrades.AddRange(cardToCombine.data.upgrades.Select(u => u.Clone()));
        }
        upgrades.AddRange(extraUpgrades.Select(u => u.Clone()));

        foreach (var upgrade in upgrades)
        {
            upgrade.Assign(cardDataClone);
        }

        yield return Run(seq, cardToCombine, cardDataClone, spawnOnBoard, row);
    }

    public static IEnumerator Run(this CombineCardSequence seq, Entity entity, CardData finalCard, bool spawnOnBoard, CardContainer row)
    {
        PauseMenu.Block();

        var card = CardManager.Get(finalCard, entity.GetCard().hover.controller, entity.owner, inPlay: false, isPlayerCard: true);
        card.transform.localScale = Vector3.one * 1f;
        card.transform.SetParent(seq.finalEntityParent);
        var finalEntity = card.entity;

        var clump = new Routine.Clump();
        clump.Add(entity.display.UpdateData());
        clump.Add(finalEntity.display.UpdateData());
        //clump.Add(Sequences.Wait(0.5f));
        yield return clump.WaitForEnd();

        entity.RemoveFromContainers();

        entity.transform.localScale = Vector3.one * 0.8f;

        seq.fader.In();

        var zero = Vector3.zero;
        zero += entity.transform.position;
        zero /= 1;

        seq.group.position = zero;

        var transform = Object.Instantiate(seq.pointPrefab, entity.transform.position, Quaternion.identity, seq.group);
        transform.gameObject.SetActive(value: true);
        entity.transform.SetParent(transform);
        entity.flipper.FlipUp();
        seq.points.Add(transform);
        LeanTween.alphaCanvas(((Card)entity.display).canvasGroup, 1f, 0.4f).setEaseInQuad();

        foreach (var point in seq.points)
        {
            LeanTween.moveLocal(to: point.localPosition.normalized, gameObject: point.gameObject, time: 0.4f).setEaseInQuart();
        }

        yield return new WaitForSeconds(0.4f);

        Events.InvokeScreenShake();

        entity.wobbler.WobbleRandom();

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

        Events.InvokeScreenShake();
        if ((bool)seq.ps)
        {
            seq.ps.Play();
        }

        seq.combinedFx.SetActive(value: true);

        finalEntity.transform.position = Vector3.zero;
        CardManager.ReturnToPool(entity);

        seq.group.transform.localRotation = Quaternion.identity;
        finalEntity.curveAnimator.Ping();
        finalEntity.wobbler.WobbleRandom();

        yield return new WaitForSeconds(1f);

        seq.fader.gameObject.Destroy();
        PauseMenu.Unblock();

        var cardNotMoved = true;
        if (spawnOnBoard)
        {
            Common.Log(row.Count.ToString());
            Common.Log(row.ChildCount.ToString());
            if (row.Count != 3)
            {
                yield return Sequences.CardMove(finalEntity, new[] { row });
                finalEntity.inPlay = true;
                cardNotMoved = false;
            }
            else
            {
                for (var i = 0; i < 2; i++)
                {
                    row = Battle.instance.GetRow(entity.owner, i);
                    if (row.Count != 3)
                    {
                        yield return Sequences.CardMove(finalEntity, new[] { row });
                        finalEntity.inPlay = true;
                        cardNotMoved = false;

                        break;
                    }
                }
            }
        }

        // if (cardNotMoved)
        // {
        //     yield return Sequences.CardMove(finalEntity, new[] { References.Player.handContainer });
        //     finalEntity.inPlay = true;
        // }

        References.Player.handContainer.TweenChildPositions();
        ActionQueue.Stack(new ActionReveal(finalEntity));
    }
}