using System.Collections.Generic;
using System.Linq;
using BattleEditor;
using Deadpan.Enums.Engine.Components.Modding;

namespace ShellBattle
{
    public partial class MoreFights : WildfrostMod
    {
        private static bool preLoaded;

        [ConfigItem(true,
            "",
            "Enable The Seedlings")]
        public bool EnableSeedlings;

        [ConfigItem(false,
            "i.e. the second fight will always be The Seedlings. Changes will be made after a game restart and new run.",
            "Always fight The Seedlings")]
        public bool AlwaysFight;


        // [ConfigItem(true,
        //     "",
        //     "Enable The Ice Kubes")]
        // public bool EnableIce;
        //
        // [ConfigItem(true,
        //     "i.e. the first fight will always be The Ice Kubes. Changes will be made after a game restart and new run.",
        //     "Always fight The Ice Kubes")]
        // public bool AlwaysFightIce;

        private readonly List<CardDataBuilder> cards = new List<CardDataBuilder>();
        private readonly List<StatusEffectDataBuilder> statusEffects = new List<StatusEffectDataBuilder>();

        public MoreFights(string modDirectory) : base(modDirectory)
        {
        }

        public override string GUID => "bethanw10.wildfrost.shellbattle";

        public override string[] Depends => new[] { "mhcdc9.wildfrost.battle" };

        public override string Title => "The Seedlings";

        public override string Description => "Adds a new shell-based 2nd level fight";

        public override void Load()
        {
            if (preLoaded)
            {
                return;
            }

            AddShellCards();
            AddIceCards();

            base.Load();

            if (EnableSeedlings)
            {
                AddShellBattle();
            }

            // if (EnableIce)
            // {
            //     AddIceBattle();
            // }

            preLoaded = true;
        }

        public override List<T> AddAssets<T, TY>()
        {
            var typeName = typeof(TY).Name;
            switch (typeName)
            {
                case nameof(CardData):
                    return cards.Cast<T>().ToList();
                case nameof(StatusEffectData):
                    return statusEffects.Cast<T>().ToList();
                default:
                    return null;
            }
        }
    }
}