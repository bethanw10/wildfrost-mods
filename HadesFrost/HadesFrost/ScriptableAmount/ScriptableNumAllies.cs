using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using HadesFrost.Utils;
using UnityEngine;

public class ScriptableNumAllies : ScriptableAmount
{
    public override int Get(Entity entity)
    {
        if (!(bool)(Object)entity)
        {
            return 0;
        }
        
        var allies = Battle.GetAllUnits(entity.owner);
        
        var count = allies?.Count(a => a.IsAliveAndExists() && !a.data.IsClunker && a.data.cardType.name != "Leader");
        //
        // var deck = References.Player?.data?.inventory?.deck;
        //
        // var count = deck?.Where(d => d.IsCompanion()).Count();

        Common.Log(count);

        return count ?? 0;
    }
}