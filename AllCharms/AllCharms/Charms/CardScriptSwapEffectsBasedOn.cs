using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;

namespace AllCharms.Charms
{
    public class CardScriptSwapEffectsBasedOn : CardScript
    {
        [SerializeField]
        public StatusEffectData statusA;
        [SerializeField]
        public StatusEffectData statusB;

        public WildfrostMod Mod;

        public override void Run(CardData target)
        {
            foreach (var attackEffect in target.attackEffects)
            {
                if (attackEffect.data.type == statusA.type)
                    attackEffect.data = statusB;
                else if (attackEffect.data.type == statusB.type)
                    attackEffect.data = statusA;
                else if (attackEffect.data is StatusEffectInstantDoubleX data)
                    TrySwap(data, attackEffect, statusA, statusB);
            }

            foreach (var startWithEffect in target.startWithEffects)
            {
                switch (startWithEffect.data)
                {
                    case StatusEffectApplyXWhenYAppliedTo effect1:
                        TrySwap(effect1, startWithEffect, statusA, statusB);
                        break;
                    case StatusEffectApplyXWhenYAppliedToAlly effect2:
                        TrySwap(effect2, startWithEffect, statusA, statusB);
                        break;
                    case StatusEffectApplyXWhenYAppliedToSelf effect3:
                        TrySwap(effect3, startWithEffect, statusA, statusB);
                        break;
                    case StatusEffectApplyX effect4:
                        TrySwap(effect4, startWithEffect, statusA, statusB);
                        break;
                    case StatusEffectBonusDamageEqualToX effect5:
                        TrySwap(effect5, startWithEffect, statusA, statusB);
                        break;
                }
            }
        }

        private bool Swap(
            CardData.StatusEffectStacks stacks,
            StatusEffectData a,
            StatusEffectData b)
        {
            string assetName = stacks.data.name.Replace(a.name, b.name);
            StatusEffectData statusEffectData = AddressableLoader.Get<StatusEffectData>("StatusEffectData", assetName);
            if ((bool)(Object)statusEffectData)
            {
                stacks.data = statusEffectData;
                return true;
            }
            Debug.LogError("[" + assetName + "] effect does not exist! Cannot swap effect [" + stacks.data.name + "] :(");

            statusEffectData = AddressableLoader.Get<StatusEffectData>("StatusEffectData", Extensions.PrefixGUID(assetName, Mod));
            if ((bool)statusEffectData)
            {
                stacks.data = statusEffectData;
                return true;
            }

            Debug.LogError("[" + Extensions.PrefixGUID(assetName, Mod) + "] effect does not exist! Cannot swap effect [" + stacks.data.name + "] :(");
            
            return false;
        }

        private void TrySwap(
            StatusEffectInstantDoubleX effect,
            CardData.StatusEffectStacks stacks,
            StatusEffectData a,
            StatusEffectData b)
        {
            if (!(bool)effect.statusToDouble)
                return;
            if (effect.statusToDouble.type == a.type)
            {
                Swap(stacks, a, b);
            }
            else
            {
                if (effect.statusToDouble.type != b.type)
                    return;
                Swap(stacks, b, a);
            }
        }

        private void TrySwap(
            StatusEffectApplyX effect,
            CardData.StatusEffectStacks stacks,
            StatusEffectData a,
            StatusEffectData b)
        {
            if (!(bool)(Object)effect.effectToApply)
                return;
            if (effect.effectToApply.type == a.type)
            {
                Swap(stacks, a, b);
            }
            else
            {
                if (effect.effectToApply.type != b.type)
                    return;
                Swap(stacks, b, a);
            }
        }

        private void TrySwap(
            StatusEffectApplyXWhenYAppliedTo effect,
            CardData.StatusEffectStacks stacks,
            StatusEffectData a,
            StatusEffectData b)
        {
            if (effect.whenAppliedTypes.Contains<string>(a.type) || (bool)(Object)effect.effectToApply && effect.effectToApply.type == a.type)
            {
                Swap(stacks, a, b);
            }
            else
            {
                if (!effect.whenAppliedTypes.Contains<string>(b.type) && (!(bool)(Object)effect.effectToApply || effect.effectToApply.type != b.type))
                    return;
                Swap(stacks, b, a);
            }
        }

        private void TrySwap(
            StatusEffectApplyXWhenYAppliedToAlly effect,
            CardData.StatusEffectStacks stacks,
            StatusEffectData a,
            StatusEffectData b)
        {
            if (effect.whenAppliedType == a.type || (bool)(Object)effect.effectToApply && effect.effectToApply.type == a.type)
            {
                Swap(stacks, a, b);
            }
            else
            {
                if (effect.whenAppliedType != b.type && (!(bool)(Object)effect.effectToApply || effect.effectToApply.type != b.type))
                    return;
                Swap(stacks, b, a);
            }
        }

        private void TrySwap(
            StatusEffectApplyXWhenYAppliedToSelf effect,
            CardData.StatusEffectStacks stacks,
            StatusEffectData a,
            StatusEffectData b)
        {
            if (effect.whenAppliedType == a.type || (bool)(Object)effect.effectToApply && effect.effectToApply.type == a.type)
            {
                Swap(stacks, a, b);
            }
            else
            {
                if (effect.whenAppliedType != b.type && (!(bool)(Object)effect.effectToApply || effect.effectToApply.type != b.type))
                    return;
                Swap(stacks, b, a);
            }
        }

        private void TrySwap(
            StatusEffectBonusDamageEqualToX effect,
            CardData.StatusEffectStacks stacks,
            StatusEffectData a,
            StatusEffectData b)
        {
            if (effect.effectType == a.type)
            {
                Swap(stacks, a, b);
            }
            else
            {
                if (effect.effectType != b.type)
                    return;
                Swap(stacks, b, a);
            }
        }
    }
}