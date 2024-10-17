using UnityEngine;

namespace AllCharms.Charms
{
    [CreateAssetMenu(menuName = "Scriptable Amount/Cards In Hand", fileName = "CardsInHand")]
    public class ScriptableBellCount : ScriptableAmount
    {
        public override int Get(Entity entity)
        {
            var bell = GameObject.Find("BellChargeIcon");
            var charge = bell?.GetComponentInChildren<StatusIconCharge>();

            var count = charge?.value.current ?? 0;

            return count;
        }
    }
}