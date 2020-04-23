using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using static HaranuBot.Currency.CurrencyConversion;

namespace HaranuBot
{
    public static class Utils
    {
        public static string ToTitleCase(this string text)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(text);
        }

        public static string ToTileString(this CoinType coinType)
        {
            return coinType.ToString().ToLower().ToTitleCase();
        }

        public static CoinType GetLowerDenomination(this CoinType coinType)
        {
            switch (coinType)
            {
                case CoinType.SILVER:
                    return CoinType.COPPER;
                case CoinType.CHIMES:
                    return Program.options.ChimeRate > 1 ? CoinType.SILVER : CoinType.COPPER;
                case CoinType.GOLD:
                    return CoinType.SILVER;
                case CoinType.PLATINUM:
                    return CoinType.GOLD;
                case CoinType.TAIJITU:
                    return CoinType.CHIMES;
                default:
                    return CoinType.NONE;
            }
        }

        public static string BuildName(this string[] words)
        {
            return BuildName(words, 0, words.Length - 1);
        }

        public static string BuildName(this string[] words, int endIndex)
        {
            return BuildName(words, 0, endIndex);
        }

        public static string BuildName(this string[] words, int startIndex, int endIndex)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = startIndex; i < endIndex; i++)
            {
                builder.Append(words[i]);
                builder.Append(' ');
            }
            builder.Append(words[endIndex]);

            return builder.ToString();
        }
    }
}
