using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20251229_Blackjack_Game
{
    internal class Player
    {
        public string _name;
        public Hand Hand = new Hand();

        public Player(string name)
        {
            _name = name;
        }

        public void AddCard(Card card)
        {
            Hand.Hand_Cards.Add(card);
        }
    }
}
