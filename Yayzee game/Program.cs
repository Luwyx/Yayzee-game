using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace YahtzeeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Yahtzee!");

            Console.Write("Enter number of players: ");
            int numPlayers = int.Parse(Console.ReadLine());

            List<Player> players = new List<Player>();
            for (int i = 0; i < numPlayers; i++)
            {
                Console.Write($"Player {i + 1}, enter your name: ");
                string playerName = Console.ReadLine();
                players.Add(new Player(playerName));
            }

            Random random = new Random();

            foreach (Player player in players)
            {
                int rollsLeft = 3;
                player.LoadDice();

                if (player.Dice.Count == 0)
                {
                    // Initial roll at the start of the player's turn
                    for (int i = 0; i < 5; i++)
                    {
                        player.Dice.Add(random.Next(1, 7));
                    }
                }

                Console.WriteLine($"\n{player.Name}, it's your turn.");

                while (rollsLeft > 0)
                {
                    Console.WriteLine($"Rolls left: {rollsLeft}");
                    Console.WriteLine("Dice: " + string.Join(" ", player.Dice.Select(d => d.ToString())));
                    Console.Write("Keep some dice? (y/n): ");
                    string keepInput = Console.ReadLine();

                    if (keepInput.ToLower() == "y")
                    {
                        Console.Write("Enter dice indexes to keep (space-separated, and starting at 0 and ending at 4): ");
                        string indexesInput = Console.ReadLine();
                        int[] indexesToKeep = indexesInput.Split(' ').Select(int.Parse).ToArray();

                        for (int i = 0; i < 5; i++)
                        {
                            if (!indexesToKeep.Contains(i))
                            {
                                player.Dice[i] = random.Next(1, 7);
                            }
                        }

                        player.SaveDice();  // Save dice values after keeping
                    }
                    else
                    {
                        // Roll only the non-kept dice
                        List<int> newDiceKept = new List<int>();
                        for (int i = 0; i < 5; i++)
                        {
                            if (player.DiceKept.Contains(i))
                            {
                                newDiceKept.Add(i);  // Add the index if it was kept
                            }
                            else
                            {
                                player.Dice[i] = random.Next(1, 7);
                            }
                        }

                        player.DiceKept = newDiceKept;  // Update DiceKept with the modified list

                        player.SaveDice();  // Save dice values after rolling
                    }

                    rollsLeft--;
                }

                Console.WriteLine($"\nFinal dice for {player.Name}: " + string.Join(" ", player.Dice.Select(d => d.ToString())));

                Console.WriteLine($"\n{player.Name}'s Turn Over!");
            }

            Console.WriteLine("\nThanks for playing Yahtzee!");
        }
    }

    class Player
    {
        public string Name { get; }
        public List<int> Dice { get; set; }
        public List<int> DiceKept { get; set; }

        public Player(string name)
        {
            Name = name;
            Dice = new List<int>();
            DiceKept = new List<int>();
        }

        public void SaveDice()
        {
            // Create file paths for dice and kept dice data
            string diceFilePath = $"{Name}_dice.txt";
            string keptDiceFilePath = $"{Name}_kept_dice.txt";

            // Write dice and kept dice data to files
            File.WriteAllText(diceFilePath, string.Join(" ", Dice));
            File.WriteAllText(keptDiceFilePath, string.Join(" ", DiceKept));
        }

        public void LoadDice()
        {
            // Create file paths for dice and kept dice data
            string diceFilePath = $"{Name}_dice.txt";
            string keptDiceFilePath = $"{Name}_kept_dice.txt";

            // Load dice data from file, if it exists
            if (File.Exists(diceFilePath))
            {
                string diceString = File.ReadAllText(diceFilePath);
                Dice = ParseDiceValues(diceString); // Parse and populate Dice list
            }
            else
            {
                Dice = new List<int>(); // Initialize Dice list if file doesn't exist
            }

            // Load kept dice data from file, if it exists
            if (File.Exists(keptDiceFilePath))
            {
                string keptDiceString = File.ReadAllText(keptDiceFilePath);
                DiceKept = ParseDiceValues(keptDiceString); // Parse and populate DiceKept list
            }
            else
            {
                DiceKept = new List<int>(); // Initialize DiceKept list if file doesn't exist
            }
        }

        private List<int> ParseDiceValues(string diceString)
        {
            List<int> diceValues = new List<int>();

            // Split diceString by space and parse values
            foreach (string value in diceString.Split(' '))
            {
                if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out int diceValue))
                {
                    diceValues.Add(diceValue);
                }
                else
                {
                    Console.WriteLine($"Error parsing dice value: {value}");
                }
            }

            return diceValues;
        }
    }
}
