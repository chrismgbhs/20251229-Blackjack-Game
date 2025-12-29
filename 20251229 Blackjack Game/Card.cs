using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20251229_Blackjack_Game
{
    internal class Card
    {
        public int _value;
        public string _name;
        public string _category;

        public Card(int value, string name, string category)
        {
            _value = value;
            _name = name;
            _category = category;
        }
    }
}
