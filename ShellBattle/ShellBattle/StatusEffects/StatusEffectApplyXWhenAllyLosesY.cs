// Decompiled with JetBrains decompiler
// Type: StatusEffectApplyXOnTurn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Modded\Wildfrost_Data\Managed\Assembly-CSharp-Publicized.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectApplyXWhenAllyLosesY : StatusEffectApplyX
{
    [SerializeField]
    public bool trueOnTurn;
    public bool turnTaken;
    public int x;

    public override void Init()
    {
        this.OnCardPlayed += new StatusEffectData.EffectCardPlayEventHandler(this.CheckCardPlay);
        this.OnTurn += new StatusEffectData.EffectEntityEventHandler(this.CheckTurn);
    }

    public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
    {
        if (this.turnTaken || !this.target.enabled || !((Object)entity == (Object)this.target) || !Battle.IsOnBoard(this.target))
            return false;
        if (!this.trueOnTurn)
            return true;
        this.turnTaken = true;
        return false;
    }

    public IEnumerator CheckCardPlay(Entity entity, Entity[] targets) => this.Run(this.GetRandomTargets());

    public override bool RunTurnEvent(Entity entity) => this.trueOnTurn && this.turnTaken && (Object)entity == (Object)this.target && Battle.IsOnBoard(this.target);

    public IEnumerator CheckTurn(Entity entity)
    {
        // ISSUE: reference to a compiler-generated field
        int num = this.x;
        var effectApplyXonTurn = this;
        if (num != 0)
        {
            if (num != 1)
                yield return false;
            // ISSUE: reference to a compiler-generated field
            this.x = -1;
            effectApplyXonTurn.turnTaken = false;
            yield return false;
        }
        // ISSUE: reference to a compiler-generated field
        this.x = -1;
        // ISSUE: reference to a compiler-generated field
        effectApplyXonTurn.Run(effectApplyXonTurn.GetRandomTargets());
        // ISSUE: reference to a compiler-generated field
        this.x = 1;
        yield return true;
    }


    private List<Entity> GetRandomTargets()
    {
        var targets1 = new List<Entity>();
        var cardsOnBoard = Battle.GetCardsOnBoard(this.target.owner);
        this.RemoveIneligible(cardsOnBoard);
        if (cardsOnBoard.Count > 0)
        {
            targets1.Add(cardsOnBoard.RandomItem());

        }

        return targets1;
    }
}
