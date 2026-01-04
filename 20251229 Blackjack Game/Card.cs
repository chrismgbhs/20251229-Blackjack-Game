using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20251229_Blackjack_Game
{
    /// <summary>
    /// Represents a single playing card used by the Blackjack game.
    /// </summary>
    /// <remarks>
    /// Fields use the project convention of a leading underscore. The <see cref="_value"/> is
    /// the numeric value used for scoring; Ace handling (1 or 11) is performed by hand-scoring logic.
    /// </remarks>
    internal class Card
    {
        /// <summary>
        /// Numeric Blackjack value for the card. For an Ace, value may be interpreted as 1 or 11
        /// by scoring logic in other classes.
        /// </summary>
        public int _value;

        /// <summary>
        /// Human-readable name of the card (for example, "Ace of Spades").
        /// </summary>
        public string _name;

        /// <summary>
        /// Suit or category of the card (for example, "Hearts", "Clubs").
        /// </summary>
        public string _category;

        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class.
        /// </summary>
        /// <param name="value">Numeric score value for the card.</param>
        /// <param name="name">Display name of the card.</param>
        /// <param name="category">Suit or category of the card.</param>
        public Card(int value, string name, string category)
        {
            // Use the project's field naming convention (leading underscore) when assigning.
            _value = value;
            _name = name;
            _category = category;
        }
    }
}
