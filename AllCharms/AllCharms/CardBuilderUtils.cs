using Deadpan.Enums.Engine.Components.Modding;
using UnityEngine;

namespace AllCharms
{
    public static class CardBuilderUtils
    {
        public static CardUpgradeDataBuilder WithTextForAllLanguages(this CardUpgradeDataBuilder builder, string title)
        {
            builder.WithText(title);
            builder.WithText(title, SystemLanguage.Korean);
            builder.WithText(title, SystemLanguage.Chinese);
            builder.WithText(title, SystemLanguage.Japanese);
            builder.WithText(title, SystemLanguage.ChineseSimplified);

            return builder;
        }

        public static StatusEffectDataBuilder WithTextForAllLanguages(this StatusEffectDataBuilder builder, string title)
        {
            builder.WithText(title);
            builder.WithText(title, SystemLanguage.Korean);
            builder.WithText(title, SystemLanguage.Chinese);
            builder.WithText(title, SystemLanguage.Japanese);
            builder.WithText(title, SystemLanguage.ChineseTraditional);
            builder.WithText(title, SystemLanguage.ChineseSimplified);

            return builder;
        }
    }
}
