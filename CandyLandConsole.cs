using System;
using System.Collections.Generic;

namespace CandyLandConsole
{
    class Program
    {
        // ====================================================================
        // Game Board Constants (Avoids "Magic Numbers" and formatting errors)
        // ====================================================================
        private const int BoardSize = 134;
        private const int StartIndex = 0;
        private const int CastleIndex = 133;

        // Character Space Indices
        private const int PlumIndex = 9;
        private const int PeppermintStickIndex = 17;
        private const int GumdropIndex = 42;
        private const int PeanutBrittleIndex = 75;
        private const int LollipopIndex = 96;
        private const int SnowflakeIndex = 117;

        // Shortcut Indices
        private const int GummyPassStartIndex = 7;
        private const int GummyPassEndIndex = 55;
        private const int PeppermintPassStartIndex = 20;
        private const int PeppermintPassEndIndex = 37;

        // Penalty Indices (Licorice)
        private const int Licorice1Index = 34;
        private const int Licorice2Index = 82;

        // ==========================================
        // DAY 1: Arrays and Constants
        // ==========================================
        // C# Intro 4: Declare and initialize the board array of colors and special spaces. check commits
        private static string[] board;
        
        // Day 2: Card deck collection
        private static List<string> deck;
        private static Random random = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("Welcome to C# Console Candy Land!");
            Console.WriteLine("================================================="); 

            // ==========================================
            // DAY 1 & 3: Initialization & Player Setup
            // ==========================================
            InitializeBoard();
            InitializeDeck();

            // C# Intro 1: Ask user for player count and names (Console.ReadLine/WriteLine)
            int playerCount = 0;
            while (playerCount < 2 || playerCount > 4)
            {
                Console.Write("Enter number of players (2-4): ");
                if (!int.TryParse(Console.ReadLine(), out playerCount) || playerCount < 2 || playerCount > 4)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 2 and 4.");
                }
            }

            // C# Intro 4 & 6: Create an array of Player objects
            Player[] players = new Player[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                Console.Write($"Enter Name for Player {i + 1}: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = $"Player {i + 1}";
                }
                players[i] = new Player(name); // Invokes the parameterized constructor [C# Intro 6]
            }

            Console.WriteLine("\nThe game begins! Press Enter to take turns.");
            Console.ReadLine();

            // ==========================================
            // DAY 3: The Main Turn Loop
            // ==========================================
            // C# Intro 3: Keep the game repeating until a winner is found
            int currentTurnIndex = 0;
            bool gameRunning = true;

            while (gameRunning)
            {
                Player activePlayer = players[currentTurnIndex];

                Console.Clear();
                PrintBoardState(players);
                Console.WriteLine($"\n--- {activePlayer.Name}'s Turn (Current Position: {activePlayer.Position}) ---");

                // C# Intro 2: Conditionals & Logic for stuck players (Licorice)
                if (activePlayer.StuckTurns > 0)
                {
                    Console.WriteLine($"🍭 {activePlayer.Name} is stuck in Licorice! Skipping turn to eat candy...");
                    activePlayer.StuckTurns--; // Decrement the penalty count [C# Intro 1]
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Press Enter to draw a card...");
                    Console.ReadLine();

                    // DAY 2: Draw card and handle movement
                    string drawnCard = DrawCard();
                    Console.WriteLine($"{activePlayer.Name} drew: [{drawnCard}]");

                    MovePlayer(activePlayer, drawnCard);

                    Console.WriteLine($"{activePlayer.Name} is now on Space {activePlayer.Position}: ({board[activePlayer.Position]})");
                    Console.WriteLine("\nPress Enter to end turn...");
                    Console.ReadLine();
                }

                // C# Intro 5: Method call to check if game is over 
                if (IsGameOver(activePlayer))
                {
                    Console.Clear();
                    PrintBoardState(players);
                    Console.WriteLine($"\n*************************************************");
                    Console.WriteLine($"CONGRATULATIONS! {activePlayer.Name} reached King Kandy's Castle and wins!");
                    Console.WriteLine($"*************************************************");
                    gameRunning = false;
                }

                // Turn progression: Rotate to next player (Intro 1 Arithmetic Modulo [%] operator)
                currentTurnIndex = (currentTurnIndex + 1) % playerCount;
            }

            Console.WriteLine("\nThanks for playing Candy Land! Press Enter to close.");
            Console.ReadLine();
        }

        // ==========================================
        // METHOD SKELETONS (C# Intro 5: Methods)
        // ==========================================

        /// <summary>
        /// DAY 1: Setup the game board array representing the track.
        /// Colors repeat: Red, Yellow, Blue, Purple, Orange, Green.
        /// Special locations are placed at explicit indices.
        /// </summary>
        static void InitializeBoard()
        {
            board = new string[BoardSize]; // start the board with 134 spaces
            string[] colorCycle = { "Red", "Yellow", "Blue", "Purple", "Orange", "Green" }; // repeating color cycle 

            for (int i = 0; i < BoardSize; i++) // Loop through the board size 
            {
                // Fill the board with repeating colors, every time i++, it cycles through the colorCycle for each square. 
                board[i] = colorCycle[i % colorCycle.Length];  
            }

            // Save indexes for special character spots
            board[PlumIndex] = "Plumpy"; // added plumpy to the board index - 9 space
            board[PeppermintStickIndex] = "Mr. Mint"; // added Mr. Mint to the board index - 17 space
            board[GumdropIndex] = "Jolly"; // added Jolly to the board index - 42 space
            board[PeanutBrittleIndex] = "Gramma Nut"; // added Gramma Nut to the board index - 58 space
            board[LollipopIndex] = "Princess Lolly"; // added Princess Lolly to the board index - 73 space
            board[SnowflakeIndex] = "Queen Frostine"; // added Queen Frostine to the board index - 89 space

            // Save Indexes for Licorice spaces and shortcut spaces ( peperment and gummy pass - include start and end indexes)
            board[Licorice1Index] = "Licorice 1"; // added Licoric 1 to the board index - 34 space
            board[Licorice2Index] = "Licorice 2"; // added Licorice 2 to the board index - 82 space
            board[GummyPassStartIndex] = "Gummy Pass Start"; // added Gummy Pass ( start ) to the board index - 7 space
            board[GummyPassEndIndex] = "Gummy Pass End"; // added Gummy Pass ( end ) to the board index - 55 space
            board[PeppermintPassStartIndex] = "Peppermint Pass Start"; // added Peppermint ( start ) Pass to the board index - 20 space
            board[PeppermintPassEndIndex] = "Peppermint Pass End"; // added Peppermint ( end ) Pass to the board index - 37 space
        }

        /// <summary>
        /// DAY 2: Assemble the 64-card playing deck as detailed in the Card List.
        /// Includes 48 Single-color, 12 Double-color, and 4-6 Special Character cards.
        /// </summary>
        static void InitializeDeck()
        {
            // String for color cards and special cards
            string[] singleColor = { "Red", "Yellow", "Blue", "Purple", "Orange", "Green" }; // string of singlecolor cards 
            string[] doubleColor = { "Double Red", "Double Yellow", "Double Blue", "Double Purple", "Double Orange", "Double Green" }; // string of doublecolor cards
            string[] specialCards = { "Plumpy", "Mr. Mint", "Princess Lolly", "Queen Frostine", "Jolly", "Gramma Nut" }; // string of special cards

            
            // TODO: Fill the 'deck' list with:
            // - Single-color cards (e.g. "Red", "Yellow"...)
            // - Double-color cards (e.g. "Double Red", "Double Yellow"...)
            // - Special cards (e.g. "Plumpy", "Mr. Mint", "Princess Lolly", "Queen Frostine", "Jolly", "Gramma Nut")
        }

        /// <summary> 
        /// DAY 2: Draw a card from the deck. If the deck is empty, re-initialize/reshuffle.
        /// </summary>
        static string DrawCard()
        {
            // TODO: Implement drawing logic.
            // 1. If 'deck' is empty, call InitializeDeck() to reshuffle the pile.
            // 2. Select a random card index from the deck.
            // 3. Remove the drawn card from the list to simulate taking it from the deck.
            // 4. Return the card string.
            return "Red"; // Temporary placeholder
        }

        /// <summary>
        /// DAY 2 & 3: Handle player path-finding based on the drawn card.
        /// </summary>
        static void MovePlayer(Player player, string card)
        {
            // TODO: C# Intro 2 - Implement multi-way branch logic (if / else if / else)
            
            // Scenario A: Drawing a special character card
            // - Move player directly (forward or backward) to the index of that character's space on the board.

            // Scenario B: Drawing a single color card (e.g., "Red")
            // - Search forward along the 'board' array starting from the player's current position + 1.
            // - Stop and set the player's position at the FIRST index matching that color.

            // Scenario C: Drawing a double color card (e.g., "Double Yellow")
            // - Search forward starting from player's current position + 1.
            // - Find the SECOND index matching that color, and move the player there.

            // Scenario D: Check Boundaries
            // - Ensure player position does not exceed the board length (clamp to King Kandy's Castle).

            // TODO: Call CheckForShortcuts and CheckForLicorice on the final landed space.
        }

        /// <summary>
        /// DAY 2: Evaluate Gummy Pass and Peppermint Pass rules.
        /// </summary>
        static void CheckForShortcuts(Player player)
        {
            // TODO: C# Intro 2
            // If player landed by EXACT COUNT on:
            // - The Gummy Pass yellow space index -> teleport player to the yellow space above.
            // - The Peppermint Pass blue space index -> teleport player to the yellow space above.
        }

        /// <summary>
        /// DAY 2: Check if player lands on sticky Licorice.
        /// </summary>
        static void CheckForLicorice(Player player)
        {
            // TODO: C# Intro 2
            // If player lands by EXACT COUNT on a Licorice space index:
            // Set the player's StuckTurns field to 1.
        }

        /// <summary>
        /// DAY 3: Render a text representation of the path showing where players are located.
        /// </summary>
        static void PrintBoardState(Player[] players)
        {
            // TODO: C# Intro 4 (Nested loops or loop-array iteration)
            // Iterate over the board array. Print spaces as ASCII/text.
            // Indicate player positions (e.g., "Space 10 [Red] <PlayerName is here>").
            // For brevity, you can show a mini-map or print a list of active positions.
        }

        /// <summary>
        /// DAY 3: Verify if active player landed on or went past the final Castle space.
        /// </summary>
        static bool IsGameOver(Player player)
        {
            // TODO: Return true if the player's position index is >= King Kandy's Castle index.
            return false;
        }
    }

    // ====================================================================
    // DAY 1 & 3: C# Intro 6: Classes and Objects
    // ====================================================================
    class Player
    {
        // 1. Backing fields (encapsulated variables to store state) [C# Intro 6]
        private string _name;
        private int _position;
        private int _stuckTurns;

        // 2. Public Properties (Encapsulated Accessors with validation checks) [C# Intro 6]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Position
        {
            get { return _position; }
            set
            {
                // Protect class state: clamp position to prevent falling below 0
                if (value < 0) _position = 0;
                else _position = value;
            }
        }

        public int StuckTurns
        {
            get { return _stuckTurns; }
            set { _stuckTurns = value >= 0 ? value : 0; }
        }

        // 3. Default Constructor (Parameterless) [C# Intro 6]
        public Player()
        {
            _name = "Gingerbread Pawn";
            _position = 0;
            _stuckTurns = 0;
        }

        // 4. Parameterized Constructor [C# Intro 6]
        public Player(string customName)
        {
            _name = customName;
            _position = 0;
            _stuckTurns = 0;
        }
    }
}