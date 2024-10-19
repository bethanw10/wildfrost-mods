using System;
using Deadpan.Enums.Engine.Components.Modding;

namespace HadesFrost
{
    public static class EffectExt
    {
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
                    $"TryGet Error: Could not find a [{typeof(T).Name}] with the name [{name}] or [{Extensions.PrefixGUID(name, mod)}]");

            return data;
        }

        public static StatusEffectDataBuilder StatusCopy(this WildfrostMod mod, string oldName, string newName)
        {
            var data = mod.TryGet<StatusEffectData>(oldName).InstantiateKeepName();
            data.name = mod.GUID + "." + newName;
            var builder = data.Edit<StatusEffectData, StatusEffectDataBuilder>();
            builder.Mod = mod;
            return builder;
        }
    }
}