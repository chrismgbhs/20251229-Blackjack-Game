using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20251229_Blackjack_Game
{
    internal class Deck
    {
        public Stack<Card> deck = new Stack<Card>();

        public Deck()
        {
            /// Create a standard 52-card deck and shuffle it
            string[] categories = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] names = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" };
            int[] values = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 1 };

            List<Card> cards = new List<Card>();

            /// Generate the cards
            foreach (string category in categories)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    cards.Add(new Card(values[i], names[i], category));
                }
            }

            List<int> holder = new List<int>();

            Random rand = new Random();

            /// Shuffle the cards into the deck
            while (holder.Count < cards.Count)
            {
                int index = rand.Next(cards.Count);

                if (!holder.Contains(index))
                {
                    holder.Add(index);
                    deck.Push(cards[index]);
                }
            }
        }
    }
}
