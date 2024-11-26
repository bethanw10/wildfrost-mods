using System;
using System.Linq;
using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;

namespace HadesFrost.Utils
{
    public static class Common
    {
        public static void Log(object message)
        {
            Debug.Log("[hades]" + message);
        }

        public static T[] RemoveNulls<T>(this WildfrostMod mod, T[] data) where T : DataFile
        {
            var list = data.ToList();
            list.RemoveAll(x => x == null || x.ModAdded == mod);
            return list.ToArray();
        }

        public static T[] DataList<T>(this WildfrostMod mod, params string[] names) where T : DataFile => 
            names.Select(mod.TryGet<T>).ToArray();

        public static ClassDataBuilder TribeCopy(this WildfrostMod mod, string oldName, string newName)
        {
            var data = mod.TryGet<ClassData>(oldName).InstantiateKeepName();
            data.name = mod.GUID + "." + newName;
            var builder = data.Edit<ClassData, ClassDataBuilder>();
            builder.Mod = mod;
            return builder;
        }

        public static CardData.StatusEffectStacks SStack(this WildfrostMod mod, string name, int amount = 1) =>
            new CardData.StatusEffectStacks(mod.TryGet<StatusEffectData>(name), amount);

        public static CardData.TraitStacks TStack(this WildfrostMod mod, string name, int amount = 1) =>
            new CardData.TraitStacks(mod.TryGet<TraitData>(name), amount);

        public static T TryGet<T>(this WildfrostMod mod, string name) where T : DataFile
        {
            T data;
            if (typeof(StatusEffectData).IsAssignableFrom(typeof(T)))
                data = mod.Get<StatusEffectData>(name) as T;
            else
                data = mod.Get<T>(name);

            if (data == null)
                throw new Exception(
                    $"TryGet Error: Could not find a [{typeof(T).Name}] with the name [{name}] or [{Deadpan.Enums.Engine.Components.Modding.Extensions.PrefixGUID(name, mod)}]");

            return data;
        }

        public static CardUpgradeDataBuilder UpgradeCopy(this WildfrostMod mod, string oldName, string newName)
        {
            var data = mod.TryGet<CardUpgradeData>(oldName).InstantiateKeepName();
            data.name = mod.GUID + "." + newName;
            var builder = data.Edit<CardUpgradeData, CardUpgradeDataBuilder>();
            builder.Mod = mod;
            return builder;
        }

        public static StatusEffectDataBuilder StatusCopy(this WildfrostMod mod, string oldName, string newName)
        {
            var data = mod.TryGet<StatusEffectData>(oldName).InstantiateKeepName();
            data.name = mod.GUID + "." + newName;
            var builder = data.Edit<StatusEffectData, StatusEffectDataBuilder>();
            builder.Mod = mod;
            return builder;
        }

        public static CardDataBuilder CardCopy(this WildfrostMod mod, string oldName, string newName)
        {
            var data = mod.TryGet<CardData>(oldName).InstantiateKeepName();
            data.name = mod.GUID + "." + newName;
            var builder = data.Edit<CardData, CardDataBuilder>();
            builder.Mod = mod;
            return builder;
        }

        public static TargetConstraint[] TargetConstraintAlliesOnly(HadesFrost mod)
        {
            var constraint = ScriptableObject.CreateInstance<TargetConstraintIsCardType>();
            constraint.allowedTypes = new[] { mod.TryGet<CardType>("Leader"), mod.TryGet<CardType>("Friendly") };
            return new TargetConstraint[] { constraint };
        }
    }
}