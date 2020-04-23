using System;
using System.Collections.Generic;
using System.Text;

namespace HaranuBot.Currency
{
    public class CurrencyConversion
    {
        public enum CoinType
        {
            COPPER, SILVER, CHIMES, GOLD, PLATINUM, TAIJITU, NONE
        }

        private static int currencyCount;

        private const CoinType COPPER = CoinType.COPPER;
        private const CoinType SILVER = CoinType.SILVER;
        private const CoinType CHIMES = CoinType.CHIMES;
        private const CoinType GOLD = CoinType.GOLD;
        private const CoinType PLATINUM = CoinType.PLATINUM;
        private const CoinType TAIJITU = CoinType.TAIJITU;
        private const CoinType NONE = CoinType.NONE;

        private static double[,] conversionTable;

        public static void CreateConversionTable()
        {
            double rate = Program.options.ChimeRate;

            currencyCount = Enum.GetNames(typeof(CoinType)).Length - 1;

            conversionTable = new double[currencyCount, currencyCount];

            conversionTable[(int)COPPER, (int)COPPER] = 1;
            conversionTable[(int)SILVER, (int)COPPER] = 0.1;
            conversionTable[(int)CHIMES, (int)COPPER] = 1 / (rate * 10);
            conversionTable[(int)GOLD, (int)COPPER] = 0.01;
            conversionTable[(int)PLATINUM, (int)COPPER] = 0.001;
            conversionTable[(int)TAIJITU, (int)COPPER] = 1 / (rate * 1000);

            conversionTable[(int)COPPER, (int)SILVER] = 10;
            conversionTable[(int)SILVER, (int)SILVER] = 1;
            conversionTable[(int)CHIMES, (int)SILVER] = 1 / rate;
            conversionTable[(int)GOLD, (int)SILVER] = 0.1;
            conversionTable[(int)PLATINUM, (int)SILVER] = 0.01;
            conversionTable[(int)TAIJITU, (int)SILVER] = 1 / (rate * 100);

            conversionTable[(int)COPPER, (int)CHIMES] = rate * 10;
            conversionTable[(int)SILVER, (int)CHIMES] = rate;
            conversionTable[(int)CHIMES, (int)CHIMES] = 1;
            conversionTable[(int)GOLD, (int)CHIMES] = rate * 0.1;
            conversionTable[(int)PLATINUM, (int)CHIMES] = rate * 0.01;
            conversionTable[(int)TAIJITU, (int)CHIMES] = 0.01;

            conversionTable[(int)COPPER, (int)GOLD] = 100;
            conversionTable[(int)SILVER, (int)GOLD] = 10;
            conversionTable[(int)CHIMES, (int)GOLD] = rate * 10;
            conversionTable[(int)GOLD, (int)GOLD] = 1;
            conversionTable[(int)PLATINUM, (int)GOLD] = 0.1;
            conversionTable[(int)TAIJITU, (int)GOLD] = 1 / (rate * 10);

            conversionTable[(int)COPPER, (int)PLATINUM] = 1000;
            conversionTable[(int)SILVER, (int)PLATINUM] = 100;
            conversionTable[(int)CHIMES, (int)PLATINUM] = rate * 100;
            conversionTable[(int)GOLD, (int)PLATINUM] = 10;
            conversionTable[(int)PLATINUM, (int)PLATINUM] = 1;
            conversionTable[(int)TAIJITU, (int)PLATINUM] = 1 / rate;
            
            conversionTable[(int)COPPER, (int)TAIJITU] = rate * 1000;
            conversionTable[(int)SILVER, (int)TAIJITU] = rate * 100;
            conversionTable[(int)CHIMES, (int)TAIJITU] = 100;
            conversionTable[(int)GOLD, (int)TAIJITU] = rate * 10;
            conversionTable[(int)PLATINUM, (int)TAIJITU] = 100;
            conversionTable[(int)TAIJITU, (int)TAIJITU] = rate;
        }

        public static ConversionResult Convert(CoinType from, CoinType to, double amount)
        {
            if (conversionTable == null)
            {
                CreateConversionTable();
            }

            double result = amount * conversionTable[(int)to, (int)from];
            double remainder = result % 1;

            int[] remainders = new int[currencyCount];

            CoinType current = to;

            while (current != NONE)
            {
                double conversion = conversionTable[(int)current, (int)to];
                double currentRemainder = Math.Floor(remainder * conversion);
                remainders[(int)current] = (int)currentRemainder;
                remainder = remainder - (currentRemainder / conversion);
                current = current.GetLowerDenomination();
            }

            return new ConversionResult((int)result, to, remainders);
        }

        public struct ConversionResult
        {
            public int result;
            public CoinType resultCurrency;
            public int[] remainders;

            public ConversionResult(int result, CoinType resultCurrency, int[] remainders)
            {
                this.result = result;
                this.resultCurrency = resultCurrency;
                this.remainders = remainders;
            }
        }
    }
}
