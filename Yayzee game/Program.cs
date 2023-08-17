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

            bool gameFinished = false;

            while (!gameFinished)
            {

                foreach (Player player in players)
                {
                    if (!player.IsAllCategoriesFilled()) // Only continue for players with unfilled categories
                    {
                        gameFinished = false; // At least one player hasn't filled all categories

                        int rollsLeft = 3;

                        Console.WriteLine($"\n{player.Name}, it's your turn.");
                        player.LoadDice();

                        if (player.Dice.Count == 0)
                        {
                            // Initial roll at the start of the player's turn
                            for (int i = 0; i < 5; i++)
                            {
                                player.Dice.Add(random.Next(1, 7));
                            }
                        }

                        while (rollsLeft > 0)
                        {
                            Console.WriteLine($"Rolls left: {rollsLeft}");
                            Console.WriteLine("Dice: " + string.Join(" ", player.Dice.Select(d => d.ToString())));
                            Console.Write("Keep any dice? (y/n): ");
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

                        // Scoring mechanism
                        Console.WriteLine("\nChoose a category to score your roll:");
                        for (int i = 0; i < 13; i++)
                        {
                            Console.WriteLine($"{i + 1}. {GetCategoryName(i + 1)}");
                        }

                        int selectedCategory = int.Parse(Console.ReadLine());
                        int score = CalculateScore(player.Dice, selectedCategory);
                        player.AddScore(score);

                        Console.WriteLine($"{player.Name} scored {score} points in the {GetCategoryName(selectedCategory)} category.");

                        if (!player.IsAllCategoriesFilled())
                        {
                            gameFinished = false; // At least one player hasn't filled all categories
                        }
                    }
                }
                gameFinished = true; // Assume all players have filled all categories

            }

            Console.WriteLine("Game Over. All players have filled all categories.");

            Console.WriteLine("\nThanks for playing Yahtzee!");
        }

        static int CalculateScore(List<int> dice, int category)
        {
            switch (category)
            {
                case 1: // Ones
                case 2: // Twos
                case 3: // Threes
                case 4: // Fours
                case 5: // Fives
                case 6: // Sixes
                    return dice.Where(d => d == category).Sum();

                case 7: // Three of a Kind
                    if (IsNOfAKind(dice, 3))
                        return dice.Sum();
                    return 0;

                case 8: // Four of a Kind
                    if (IsNOfAKind(dice, 4))
                        return dice.Sum();
                    return 0;

                case 9: // Full House
                    if (IsFullHouse(dice))
                        return 25;
                    return 0;

                case 10: // Small Straight
                    if (IsSmallStraight(dice))
                        return 30;
                    return 0;

                case 11: // Large Straight
                    if (IsLargeStraight(dice))
                        return 40;
                    return 0;

                case 12: // Yahtzee
                    if (IsNOfAKind(dice, 5))
                        return 50;
                    return 0;

                case 13: // Chance
                    return dice.Sum();

                default:
                    return 0;
            }
        }

        static string GetCategoryName(int category)
        {
            switch (category)
            {
                case 1:
                    return "Ones";
                case 2:
                    return "Twos";
                case 3:
                    return "Threes";
                case 4:
                    return "Fours";
                case 5:
                    return "Fives";
                case 6:
                    return "Sixes";
                case 7:
                    return "Three of a Kind";
                case 8:
                    return "Four of a Kind";
                case 9:
                    return "Full House";
                case 10:
                    return "Small Straight";
                case 11:
                    return "Large Straight";
                case 12:
                    return "Yahtzee";
                case 13:
                    return "Chance";
                default:
                    return "Unknown";
            }
        }

        static bool IsNOfAKind(List<int> dice, int n)
        {
            var groupedDice = dice.GroupBy(d => d);
            return groupedDice.Any(group => group.Count() >= n);
        }

        static bool IsFullHouse(List<int> dice)
        {
            var groupedDice = dice.GroupBy(d => d);
            return groupedDice.Count() == 2 && (groupedDice.All(group => group.Count() == 2) || groupedDice.All(group => group.Count() == 3));
        }

        static bool IsSmallStraight(List<int> dice)
        {
            return dice.Distinct().Count() >= 4 && (dice.Contains(1) && dice.Contains(2) && dice.Contains(3) && dice.Contains(4)) ||
                (dice.Contains(2) && dice.Contains(3) && dice.Contains(4) && dice.Contains(5)) ||
                (dice.Contains(3) && dice.Contains(4) && dice.Contains(5) && dice.Contains(6));
        }

        static bool IsLargeStraight(List<int> dice)
        {
            return dice.Distinct().Count() == 5 && (dice.Contains(1) && dice.Contains(2) && dice.Contains(3) && dice.Contains(4) && dice.Contains(5)) ||
                (dice.Contains(2) && dice.Contains(3) && dice.Contains(4) && dice.Contains(5) && dice.Contains(6));
        }
    }

    class Player
    {
        public string Name { get; }
        public List<int> Dice { get; set; }
        public List<int> DiceKept { get; set; }
        public int Score { get; private set; }

        public Player(string name)
        {
            Name = name;
            Dice = new List<int>();
            DiceKept = new List<int>();
            Score = 0;
        }

        public void AddScore(int points)
        {
            Score += points;
        }

        public bool IsAllCategoriesFilled()
        {
            // Check if the player has filled all categories
            return Score > 0; // Adjust this condition based on your scoring logic
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
