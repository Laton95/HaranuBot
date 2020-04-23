using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HaranuBot.PBDBG
{
    class Rolling
    {
        public static Random random = new Random();

        private static int[] RollDice(int numDice, int maxRoll)
        {
            int[] rolls = new int[numDice];

            for (int i = 0; i < numDice; i++)
            {
                rolls[i] = random.Next(1, maxRoll + 1);
            }

            return rolls;
        }

        public class Rolls
        {
            public int NormalRoll { get; }
            public int AdvantageRoll { get;  }
            public int GreatAdvantageRoll { get; }
            public int DisadvantageRoll { get; }
            public int GreatDisadvantageRoll { get; }
            public int[] DiceRolled { get; }

            public Rolls(int[] diceRolled)
            {
                var firstThreeOrdered = diceRolled.SkipLast(1).OrderByDescending(i => i);
                var allFourOrdered = diceRolled.OrderByDescending(i => i);

                NormalRoll = diceRolled[0] + diceRolled[1];
                AdvantageRoll = firstThreeOrdered.Max() + firstThreeOrdered.Skip(1).Max();
                GreatAdvantageRoll = allFourOrdered.Max() + allFourOrdered.Skip(1).Max();
                DisadvantageRoll = firstThreeOrdered.Min() + firstThreeOrdered.SkipLast(1).Min();
                GreatDisadvantageRoll = allFourOrdered.Min() + allFourOrdered.SkipLast(1).Min();
                DiceRolled = diceRolled;
            }

            public static Rolls Create()
            {
                return new Rolls(RollDice(4, 6));
            }
        }
    }
}
