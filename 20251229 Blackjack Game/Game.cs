using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace _20251229_Blackjack_Game
{
    /// <summary>
    /// Orchestrates rounds of the Blackjack game: player setup, dealing, player decisions,
    /// dealer play, and determining winners. All game flow runs via <see cref="StartGame"/>.
    /// </summary>
    internal class Game
    {
        /// <summary>
        /// Entry point for running the interactive Blackjack game loop.
        /// Sets up players, runs rounds indefinitely, and displays round results.
        /// </summary>
        public static void StartGame()
        {
            // Track round count and default blackjack limits.
            int round = 0;
            int numberOfPlayers = 0;
            int upperLimit = 21;
            int lowerLimit = 17;
            Computer computer = new Computer();

            Console.WriteLine("Welcome to Blackjack!");
            Console.WriteLine();

            // Prompt until a valid number of players (> 0) is entered.
            while (numberOfPlayers <= 0)
            {
                Console.Write("Number of players: ");
                int.TryParse(Console.ReadLine(), out numberOfPlayers);
                if (numberOfPlayers <= 0)
                {
                    Console.WriteLine("Please enter a valid number of players: ");
                }
            }

            // Collect player names and add Player instances to the shared table.
            for (int num = 0; num < numberOfPlayers; num++)
            {
                Console.Write($"Enter name for Player {num + 1}: ");
                string playerName = Console.ReadLine();

                Player p = new Player(playerName);
                Table.Players.Add(p);
            }

            Console.Clear();

            Console.WriteLine("All players have been added. Starting the game...");

            // Main game loop: runs until the process is terminated (console read key used between rounds).
            while (true)
            {
                Console.Clear();
                round++;
                Deck deck = new Deck();
                Console.WriteLine($"ROUND {round}");
                Console.WriteLine("Deck created and shuffled.");

                Console.WriteLine();

                // Reset dealer hand for the new round.
                computer.Hand.Hand_Cards.Clear();

                Console.WriteLine("SCORES:");
                // Reset each player's hand and display current scores.
                foreach (Player player in Table.Players)
                {
                    player.Hand.Hand_Cards.Clear();
                    Console.WriteLine($"{player._name}: {player.Score}");
                }

                // Two initial dealing passes: first pass deals first card, second pass completes initial two cards.
                for (int i = 0; i < 2; i++)
                {
                    Console.WriteLine();
                    DealCardsToDealer(computer, deck);

                    // Show the dealer's visible card (first card in hand).
                    Console.WriteLine($" - {computer.Hand.Hand_Cards[0]._name} of {computer.Hand.Hand_Cards[0]._category}");

                    if (i == 1)
                    {
                        // After second card, indicate that additional dealer cards are hidden from players.
                        Console.WriteLine(" - [Hidden Card/s]");
                    }

                    // Deal to players and handle decisions only on the second pass (after each player has 2 cards).
                    foreach (Player player in Table.Players)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Dealing cards to {player._name}...");
                        player.AddCard(deck.deck.Pop());
                        ViewHands(player.Hand.Hand_Cards);

                        // Default choice value: "1" means hit in this implementation.
                        string choice = "1";

                        // Only allow player decisions after they've received 2 cards (i == 1).
                        if (i == 1)
                        {
                            // While the player chooses to hit and their hand is below the upper limit.
                            while (choice == "1" && CalculateValues(player.Hand.Hand_Cards) < upperLimit)
                            {
                                Console.WriteLine();

                                Console.Write($"{player._name}, do you want to 'hit' or 'stand'? (Enter '1' if you want to hit. Enter any key if you want to stand.): ");
                                choice = Console.ReadLine();

                                Console.WriteLine();

                                if (choice == "1")
                                {
                                    // Player hits: draw a card and display updated hand and outcome.
                                    player.AddCard(deck.deck.Pop());
                                    Console.WriteLine($"{player._name}'s hand:");
                                    ViewHands(player.Hand.Hand_Cards);
                                    PlayerOutcome(player, upperLimit);
                                }
                                else
                                {
                                    // Player stands: allow manual ace adjustment, then enforce minimum hit limit.
                                    AdjustAce(player.Hand.Hand_Cards, upperLimit, true);

                                    if (CalculateValues(player.Hand.Hand_Cards) < lowerLimit)
                                    {
                                        // If still below lower limit, force a hit by setting choice back to "1".
                                        Console.WriteLine($"{player._name} must hit as their hand value is below {lowerLimit}.");
                                        choice = "1";
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{player._name} stands.");
                                        PlayerOutcome(player, upperLimit);
                                    }
                                }
                            }
                        }

                    }

                    // After players have been processed on second pass, resolve dealer actions.
                    if (i == 1)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Dealer's cards...");
                        ViewHands(computer.Hand.Hand_Cards);

                        // Automatically set Ace values for the dealer to an appropriate value.
                        AdjustAce(computer.Hand.Hand_Cards, upperLimit, false, true);

                        // Dealer must hit until reaching the lower limit.
                        while (CalculateValues(computer.Hand.Hand_Cards) < lowerLimit)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Dealer hits...");
                            computer.AddCard(deck.deck.Pop());
                            ViewHands(computer.Hand.Hand_Cards);
                        }

                        Console.WriteLine();

                        if (CalculateValues(computer.Hand.Hand_Cards) > upperLimit)
                        {
                            Console.WriteLine("Dealer busts!");
                        }
                        else
                        {
                            Console.WriteLine("Dealer stands.");
                        }
                    }
                }

                // Compare hands and announce winners for the round.
                DetermineWinner(computer);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays immediate outcome messages for a player based on their current hand value.
        /// </summary>
        /// <param name="player">Player whose outcome is evaluated.</param>
        /// <param name="upperLimit">Upper limit (typically 21) that constitutes a bust.</param>
        public static void PlayerOutcome(Player player, int upperLimit)
        {
            if (CalculateValues(player.Hand.Hand_Cards) > upperLimit)
            {
                Console.WriteLine();
                Console.WriteLine($"{player._name} busts!");
            }
            else if (CalculateValues(player.Hand.Hand_Cards) == upperLimit)
            {
                Console.WriteLine();
                Console.WriteLine($"{player._name} hits {upperLimit}! {player._name} automatically wins.");
            }
        }

        /// <summary>
        /// Adjusts the value of any Ace cards in a hand. Can prompt the user for a value
        /// or automatically assign 1 or 11 for dealer automation.
        /// </summary>
        /// <param name="Hand_Cards">The list of cards to inspect and possibly adjust.</param>
        /// <param name="upperLimit">The upper limit used to decide Ace automation.</param>
        /// <param name="adjustAce">If true, prompt the user to set Ace value manually.</param>
        /// <param name="automateAdjustment">If true, automatically set Ace to 1 or 11 to best fit under <paramref name="upperLimit"/>.</param>
        public static void AdjustAce(List<Card> Hand_Cards, int upperLimit, bool adjustAce = true, bool automateAdjustment = false)
        {
            foreach (Card card in Hand_Cards)
            {
                if (card._name == "Ace" && adjustAce)
                {
                    // Prompt the user to set the Ace as 1 or 11; validate input.
                    while (true)
                    {
                        Console.Write($" - {card._name} of {card._category} | Value: ");

                        if (int.TryParse(Console.ReadLine(), out int aceValue))
                        {
                            if (aceValue == 1 || aceValue == 11)
                            {
                                card._value = aceValue;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Please enter a valid value for the Ace (1 or 11).");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please enter a valid value for the Ace (1 or 11).");
                        }

                        Console.WriteLine();
                    }

                }
                else if (card._name == "Ace" && automateAdjustment)
                {
                    // For automated dealer adjustments, prefer 11 when it does not cause a bust.
                    if ((CalculateValues(Hand_Cards) + 11) - 1 <= upperLimit)
                    {
                        card._value = 11;
                    }
                    else
                    {
                        card._value = 1;
                    }
                }
            }
        }

        /// <summary>
        /// Writes each card in a hand to the console in a human-readable format.
        /// </summary>
        /// <param name="Hand_Cards">Cards to display.</param>
        public static void ViewHands(List<Card> Hand_Cards)
        {
            foreach (Card card in Hand_Cards)
            {
                Console.WriteLine($" - {card._name} of {card._category}");
            }
        }

        /// <summary>
        /// Computes the total numeric value of the cards in a hand.
        /// </summary>
        /// <param name="Hand_Cards">Cards whose values are summed.</param>
        /// <returns>The integer sum of all card values.</returns>
        public static int CalculateValues(List<Card> Hand_Cards)
        {
            int value = 0;

            foreach (Card card in Hand_Cards)
            {
                value += card._value;
            }

            return value;
        }

        /// <summary>
        /// Compares each player's final hand with the dealer's and announces results.
        /// Also increments player scores on wins.
        /// </summary>
        /// <param name="computer">Dealer/computer player to compare against.</param>
        public static void DetermineWinner(Computer computer)
        {
            Console.WriteLine();
            Console.WriteLine("===FINAL RESULTS===");

            foreach (Player player in Table.Players)
            {
                int playerValue = CalculateValues(player.Hand.Hand_Cards);
                int computerValue = CalculateValues(computer.Hand.Hand_Cards);

                // Only players above the minimum standing threshold are eligible to beat the dealer.
                if (playerValue > 16 && playerValue <= 21)
                {
                    if (computerValue > 21 || playerValue > computerValue)
                    {
                        Console.WriteLine($"{player._name} wins!");
                        player.Score++;
                    }
                    else if (playerValue == computerValue)
                    {
                        Console.WriteLine($"{player._name} ties with the dealer.");
                    }
                    else
                    {
                        Console.WriteLine($"{player._name} loses.");
                    }
                }
                else if (playerValue > 21)
                {
                    Console.WriteLine($"{player._name} busts!");
                }
                else
                {
                    Console.WriteLine($"{player._name} loses.");
                }
            }
        }

        /// <summary>
        /// Deals a single card to the dealer and writes a short message to the console.
        /// </summary>
        /// <param name="computer">Dealer/computer player who receives the card.</param>
        /// <param name="deck">Source deck to draw the card from.</param>
        public static void DealCardsToDealer(Computer computer, Deck deck)
        {
            Console.WriteLine("Dealing cards to the dealer...");
            computer.AddCard(deck.deck.Pop());
        }
    }
}
