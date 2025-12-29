using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20251229_Blackjack_Game
{
    internal class Game
    {

        public static void StartGame()
        {
            int round = 0;
            int numberOfPlayers = 0;
            Computer computer = new Computer();

            Console.WriteLine("Welcome to Blackjack!");
            Console.WriteLine();

            while (numberOfPlayers <= 0)
            {
                Console.Write("Number of players: ");
                int.TryParse(Console.ReadLine(), out numberOfPlayers);
                if (numberOfPlayers <= 0)
                {
                    Console.WriteLine("Please enter a valid number of players: ");
                }
            }

            for (int num = 0; num < numberOfPlayers; num++)
            {
                Console.Write($"Enter name for Player {num + 1}: ");
                string playerName = Console.ReadLine();

                Player p = new Player(playerName);
                Table.Players.Add(p);
            }

            Console.Clear();

            Console.WriteLine("All players have been added. Starting the game...");

            while (true)
            {
                Console.Clear();
                round++;
                Deck deck = new Deck();
                Console.WriteLine($"ROUND {round}"); 
                Console.WriteLine("Deck created and shuffled.");
                computer.Hand.Hand_Cards.Clear();

                foreach (Player player in Table.Players)
                {
                    player.Hand.Hand_Cards.Clear();
                }

                for (int i = 0; i < 2; i++)
                {
                    Console.WriteLine();
                    DealCardsToDealer(computer, deck);
                    Console.WriteLine($" - {computer.Hand.Hand_Cards[0]._name} of {computer.Hand.Hand_Cards[0]._category}");

                    if (i == 1)
                    {
                        Console.WriteLine(" - [Hidden Card/s]");
                    }

                    foreach (Player player in Table.Players)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Dealing cards to {player._name}...");
                        player.AddCard(deck.deck.Pop());
                        ViewHands(player.Hand.Hand_Cards);

                        string choice = "0";

                        if (i == 1)
                        {
                            Console.WriteLine();

                            Console.Write("Do you want to 'hit' or 'stand'? (Enter '1' if you want to hit): ");
                            choice = Console.ReadLine();

                            if (choice == "1")
                            {
                                player.AddCard(deck.deck.Pop());
                                Console.WriteLine("Your hand:");
                                ViewHands(player.Hand.Hand_Cards);

                            }

                        }

                    }

                    if (i == 1)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Revealing dealer's hidden card...");
                        ViewHands(computer.Hand.Hand_Cards);

                        if (CalculateValues(computer.Hand.Hand_Cards) < 17)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Dealer hits...");
                            computer.AddCard(deck.deck.Pop());
                            ViewHands(computer.Hand.Hand_Cards);
                        }

                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Dealer stands.");
                        }

                    }
                }

                DetermineWinner(computer);
                Console.ReadKey();
            }     
        }

        public static void ViewHands(List<Card> Hand_Cards)
        {
            foreach (Card card in Hand_Cards)
            {
                Console.WriteLine($" - {card._name} of {card._category}");
            }
        }

        public static int CalculateValues(List<Card> Hand_Cards)
        {
            int value = 0;

            foreach (Card card in Hand_Cards)
            {
                value += card._value;
            }

            foreach (Card card in Hand_Cards)
            {
                if (card._name == "Ace" && value + 11 <= 21 && value + 11 >= 17)
                {
                    value += 11;
                }

                else if (card._name == "Ace")
                {
                    value += 1;
                }
            }

            return value;
        }

        public static void DetermineWinner(Computer computer)
        {
            Console.WriteLine(); 

            foreach (Player player in Table.Players)
            {
                int playerValue = CalculateValues(player.Hand.Hand_Cards);
                int computerValue = CalculateValues(computer.Hand.Hand_Cards);

                if (playerValue > 16 && playerValue <= 21)
                {
                    if (computerValue > 21 || playerValue > computerValue)
                    {
                        Console.WriteLine($"{player._name} wins!");
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

        public static void DealCardsToDealer(Computer computer, Deck deck)
        {
            Console.WriteLine("Dealing cards to the dealer...");
            computer.AddCard(deck.deck.Pop());
        }
    }
}
