using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20251229_Blackjack_Game
{
    internal class Player
    {
        /// <summary>
        /// This field holds the name, hand, and score of the player.
        /// </summary>
        public string _name;
        public Hand Hand = new Hand();
        public int Score = 0;

        /// <summary>
        /// This constructor initializes a new Player with the specified name.
        /// </summary>
        /// <param name="name"></param>
        public Player(string name)
        {
            _name = name;
        }

        /// <summary>
        /// This method adds a card to the player's hand.
        /// </summary>
        /// <param name="card"></param>
        public void AddCard(Card card)
        {
            Hand.Hand_Cards.Add(card);
        }
    }
}
