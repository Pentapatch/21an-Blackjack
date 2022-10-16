namespace CardGame
{
    internal static class Game
    {
        const string Separator = "----------------------------------------------------------";

        enum SortingMethodEnum
        {
            RankThenSuit = 0,
            SuitThenRank,
            None,
        }

        static SortingMethodEnum sortingMethod = SortingMethodEnum.RankThenSuit;
        static bool easyMode = false;

        public static void RunGame()
        {
            Console.Title = "Blackjack (21:an plus) by Dennis Hankvist (a.k.a. Pentapatch)";

            // Stay inside a loop indefinitely (or until the user exiting the application)
            while (true)
            {
                switch (PrintMenu("Blackjack (21:an plus).", true, "Spela mot datorn", "Spela lokal multiplayer", "Visa spelregler", "Inställningar", "Avsluta"))
                {
                    case 0: // Spela mot datorn
                        PlayAgainstTheComputer();
                        break;
                    case 1: // Spela lokal multiplayer
                        PlayLocalMultiplayer();
                        break;
                    case 2: // Visa spelregler
                        DisplayRules();
                        break;
                    case 3: // Inställningar
                        DisplaySettings();
                        break;
                    case 4: // Avsluta
                        Environment.Exit(0);
                        break;
                }
            }
        }

        // Game management methods

        private static void PlayAgainstTheComputer()
        {
            while (true)
            {
                // Declare and initialize a new deck
                Deck deck = new(true);

                // Hand out two cards each
                Hand computerHand = new(deck, 2);
                Hand playerHand = new(deck, 2);

                // Sort the hands in Rank => Suit order
                if (sortingMethod != SortingMethodEnum.None)
                {
                    computerHand.Sort(sortingMethod == SortingMethodEnum.RankThenSuit);
                    playerHand.Sort(sortingMethod == SortingMethodEnum.RankThenSuit);
                }

                // Set up the console and print the initial hands
                Console.Clear();
                DisplayCards("Datorns hand är", computerHand.Cards, ConsoleColor.Blue);
                DisplayCards("Din hand är", playerHand.Cards, ConsoleColor.Cyan);
                Console.WriteLine(Separator);
                Console.WriteLine();

                // The players turn
                int playerScore = PlayersTurn(playerHand, deck);

                // The computers turn
                int computerScore = ComputersTurn(computerHand, deck, playerScore);

                // Determine the winner
                Console.WriteLine();
                Console.WriteLine(Separator);
                if ((computerScore >= playerScore && computerScore <= 21) || playerScore > 21)
                {
                    // The computer won
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Datorn vann denna omgången med {computerScore} poäng vs. dina {playerScore} poäng.");
                }
                else
                {
                    // The player won
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Du vann denna omgången med {playerScore} poäng vs. datorns {computerScore} poäng.");
                }

                // Ask if the player wants to go for another round
                if (ExpectKey("Vill du köra en omgång till?", ConsoleKey.J, ConsoleKey.N) == ConsoleKey.N)
                    break;
            }
        }

        private static void PlayLocalMultiplayer()
        {
            while (true)
            {
                // Declare and initialize a new deck
                Deck deck = new(true);

                // Hand out two cards each
                Hand playerOneHand = new(deck, 2);
                Hand playerTwoHand = new(deck, 2);

                // Sort the hands in Rank => Suit order
                playerOneHand.Sort(true);
                playerTwoHand.Sort(true);

                // Set up the console and print the initial hands
                Console.Clear();
                DisplayCards("Spelare 1's hand är", playerOneHand.Cards, ConsoleColor.Blue);
                DisplayCards("Spelare 2's hand är", playerTwoHand.Cards, ConsoleColor.Cyan);
                Console.WriteLine(Separator);
                Console.WriteLine();

                // Player 1's turn
                int playerOneScore = PlayersTurn(playerOneHand, deck, 1, ConsoleColor.Blue);

                // Player 2's turn
                int playerTwoScore = CountCardValues(playerTwoHand.Cards);
                if (playerOneScore <= 21) playerTwoScore = PlayersTurn(playerTwoHand, deck, 2, ConsoleColor.Cyan);

                // Determine the winner
                Console.WriteLine();
                Console.WriteLine(Separator);
                if ((playerOneScore > playerTwoScore && playerOneScore <= 21) || playerTwoScore > 21)
                {
                    // Player 1 won
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Spelare 1 vann denna omgången med {playerOneScore} poäng vs. {playerTwoScore} poäng.");
                }
                else if ((playerTwoScore > playerOneScore && playerTwoScore <= 21) || playerOneScore > 21)
                {
                    // Player 2 won
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Spelare 2 vann denna omgången med {playerTwoScore} poäng vs. {playerOneScore} poäng.");
                }
                else
                {
                    // A tie
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Det blev lika med {playerOneScore} poäng.");
                }

                // Ask if the players wants to go for another round
                if (ExpectKey("Vill ni köra en omgång till?", ConsoleKey.J, ConsoleKey.N) == ConsoleKey.N)
                    break;
            }
        }

        private static int PlayersTurn(Hand playerHand, Deck deck, int playerIndex = 0, ConsoleColor color = ConsoleColor.Cyan)
        {

            if (playerIndex != 0)
            {
                if (playerIndex == 2)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine(Separator + "\n");
                }
                Console.ForegroundColor = color;
                Console.WriteLine($"> Spelare {playerIndex}'s tur:\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            int playerScore = CountCardValues(playerHand.Cards);
            // Let the player draw as many cards as he wishes
            while (true)
            {
                // Check if the player reached 21 (automatically stop asking for new cards)
                if (CountCardValues(playerHand.Cards) == 21)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Du har nu 21 poäng och stannar automatiskt.");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                }

                // Ask if the player wants to draw another card
                ConsoleKey key = ExpectKey("Vill du dra ett till kort?", ConsoleKey.J, ConsoleKey.N);
                if (key == ConsoleKey.N) break;

                // Draw another card and put it in the players hand
                Card card = deck.Pick();
                playerHand.Take(card);

                // Print the card that was drawn
                Console.WriteLine();
                DisplayCard((playerIndex == 0 ? "Du" : ("Spelare " + playerIndex)) + " plockade kortet", card, color);

                // Check if the drawn card is an ace.
                // Ask if the player if he wants it to be 1 or 10,
                // or automatically select 1 if the player would lose otherwise.
                CheckForAce(card, playerHand);

                // Display the users hand
                if (sortingMethod != SortingMethodEnum.None) playerHand.Sort(sortingMethod == SortingMethodEnum.RankThenSuit);
                DisplayCards((playerIndex == 0 ? "Din" : ("Spelare " + playerIndex) + "'s") + " hand är nu", playerHand.Cards, color);

                playerScore = CountCardValues(playerHand.Cards);

                // Check if the player became fat
                if (CheckIfFat(playerScore, false)) break;
            }

            return playerScore;
        }

        private static int ComputersTurn(Hand computerHand, Deck deck, int playerScore)
        {
            int computerScore = CountCardValues(computerHand.Cards);

            // Check if the player has already lost (no need to draw any cards)
            if (playerScore > 21) return computerScore;
            if (computerScore >= playerScore) return computerScore;

            Console.WriteLine();
            Console.WriteLine(Separator);

            // Let the computer draw new cards until the it has more point or equally as many points as the player
            while ((computerScore < playerScore) && computerScore < 21)
            {
                // Draw another card and put it in the players hand
                Card card = deck.Pick();
                computerHand.Take(card);

                // Print the card that was drawn
                Console.WriteLine();
                DisplayCard("Datorn plockade kortet", card, ConsoleColor.Blue);

                // Check if the drawn card is an ace.
                // The computer will automatically choose the Ace to be worth 10 points (so long doing so will not lose the game)
                CheckForAce(card, computerHand, true);

                // Display the computers hand
                computerHand.Sort(true);
                DisplayCards("Datorns hand är nu", computerHand.Cards, ConsoleColor.Blue);

                // Update the computers score
                computerScore = CountCardValues(computerHand.Cards);

                // Check if the computer became fat
                CheckIfFat(computerScore, true);

                // Wait one second
                Thread.Sleep(1000);
            }

            return computerScore;
        }

        private static void DisplaySettings()
        {
            while (true)
            {
                bool exitLoop = false;
                switch (PrintMenu("Inställningar för spelet.", true, "Kortsortering", "Svårighetsnivå", "Gå tillbaka.."))
                {
                    case 0:
                        switch (PrintMenu("Kortsortering.", true, "Rank => Suit", "Suit => Rank", "Ingen", "Gå tillbaka.."))
                        {
                            case 0: // Rank => Suit
                                sortingMethod = SortingMethodEnum.RankThenSuit;
                                break;
                            case 1: // Suit => Rank
                                sortingMethod = SortingMethodEnum.SuitThenRank;
                                break;
                            case 2: // Ingen
                                sortingMethod = SortingMethodEnum.None;
                                break;
                        }
                        break;
                    case 1:
                        switch (PrintMenu("Svårighetsnivå.", true, "Svårt (Korten skrivs ut i dess fulla namn, ingen totalsumma visas)", "Lätt (Korten skrivs ut som dess värde, totalsumma visas)", "Gå tillbaka.."))
                        {
                            case 0: // Rank => Suit
                                easyMode = false;
                                break;
                            case 1: // Suit => Rank
                                easyMode = true;
                                break;
                        }
                        break;
                    case 2:
                        exitLoop = true;
                        break;
                }

                if (exitLoop) break;
            }
        }

        // Game helper methods

        private static bool CheckIfFat(int score, bool isComputer)
        {
            if (score > 21)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine((isComputer ? "Datorn" : "Du") + " blev fet.");
                Console.ForegroundColor = ConsoleColor.White;
                return true;
            }
            return false;
        }

        private static void CheckForAce(Card card, Hand playerHand, bool isComputer = false)
        {
            if (card.Rank == Card.RankEnum.Ace)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                // Check if the Ace should automatically count as 1 (otherwise the player would lose)
                if (CountCardValues(playerHand.Cards) + 10 > 21)
                {
                    // Automatically make the Ace worth 1 point
                    Console.WriteLine((!isComputer ? "Ditt" : "Datorns") + " Ess räknas automatiskt som 1 poäng (för att inte få över 21 poäng).");
                    card.OverrideValue = 1;
                }
                else
                {
                    if (!isComputer)
                    {
                        // Not automatic: Ask the player if he wants the Ace to be counted as 1 point
                        if (ExpectKey($"Vill du att det ska räknas som 1 poäng?", ConsoleKey.J, ConsoleKey.N) == ConsoleKey.J)
                            card.OverrideValue = 1;
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static int CountCardValues(List<Card> cards)
        {
            int result = 0;
            foreach (Card card in cards)
            {
                result += CountCardValue(card);
            }
            return result;
        }

        private static int CountCardValue(Card card)
        {
            if (card.Value <= 10)
                return card.Value;
            else
                return 10;
        }

        private static string GetEasyCard(Card card) => card.Value switch
        {
            <= 13 => CountCardValue(card).ToString(),
            _ => "A"
        };

        // Console write methods

        private static void DisplayCard(string message, Card card, ConsoleColor color, bool newLine = true) =>
            DisplayCards(message, new List<Card>() { card }, color, newLine);

        private static void DisplayCards(string message, List<Card> cards, ConsoleColor color, bool newLine = true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message + " ");

            for (int i = 0; i < cards.Count; i++)
            {
                Console.ForegroundColor = color;
                Console.Write(easyMode == false ? cards[i] : GetEasyCard(cards[i]));
                Console.ForegroundColor = ConsoleColor.White;
                if (i != cards.Count - 1) Console.Write(", ");
                if (i == cards.Count - 1)
                {
                    if (easyMode && i != 0) Console.Write($" ({CountCardValues(cards)})");
                    Console.Write(".");
                }
            }

            if (newLine) Console.WriteLine();
        }

        private static void DisplayRules()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("SPELARE VERSUS DATOR:");
            Console.WriteLine("Både du och datorn får poäng genom att dra kort, varje kort är värt 1 – 10 poäng.");
            Console.WriteLine("När spelet börjar dras två kort till både dig och datorn.");
            Console.WriteLine("Därefter får du dra hur många kort som du vill tills du är nöjd med din totalpoäng, du vill komma så nära 21 som möjligt utan att få mer än 21 poäng.");
            Console.WriteLine("När du inte vill dra fler kort så kommer datorn att dra kort tills den har mer eller lika många poäng som dig.");
            Console.WriteLine();
            Console.WriteLine("Du vinner om datorn får mer än totalt 21 poäng när den håller på att dra kort.");
            Console.WriteLine("Datorn vinner om den har mer poäng än dig när spelet är slut så länge som datorn inte har mer än 21 poäng.");
            Console.WriteLine("Om det skulle bli lika i poäng så vinner datorn. Om du får mer än 21 poäng när du drar kort så har du förlorat.");

            AwaitUser();

            Console.Clear();
            Console.WriteLine("LOKAL MULTIPLAYER:");
            Console.WriteLine("Fungerar i stort likadant som mot datorn. Skillnaden är att spelare 1 först kör sina omgångar, sedan spelare 2.");
            Console.WriteLine("Spelet blir oavgjort ifall poängen blir lika.");

            AwaitUser();
        }

        // Console helper methods

        private static ConsoleKey ExpectKey(string message, params ConsoleKey[] keys)
        {
            // Write the message
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);

            // Write the available key options
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" (");
            for (int i = 0; i < keys.Length; i++)
            {
                Console.Write(keys[i].ToString());
                if (i != keys.Length - 1) Console.Write("/");
            }
            Console.Write(")");
            Console.ForegroundColor = ConsoleColor.White;

            while (true)
            {
                // Get the key from the user
                ConsoleKey key = Console.ReadKey(true).Key;

                // Check if the key was expected
                if (keys.Contains(key))
                {
                    Console.Write($": {key}\n");
                    return key;
                }
            }
        }

        private static void AwaitUser(string message = "")
        {
            // Set up default message
            if (message == "") message = "Tryck på valfri knapp för att fortsätta...";

            // Print the message then await the user
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey(true);
        }

        private static int PrintMenu(string message, bool wrapArround, params string[] menuItem)
        {
            // Clear the console and write the message
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Tryck på upp- eller nedpil för att navigera i menyn.\n" +
                              "Tryck på enter för att välja alternativ.\n");
            Console.ForegroundColor = ConsoleColor.White;

            // Write the menu items
            int index = 0;
            int menuStartConsoleIndex = Console.CursorTop;
            for (int i = 0; i < menuItem.Length; i++)
            {
                if (i == index)
                    Console.ForegroundColor = ConsoleColor.Blue;
                else
                    Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine(menuItem[i]);
            }

            // Stay inside a loop until the user presses enter
            while (true)
            {
                // Wait for the user to press a key
                bool exitLoop = false;
                int lastIndex = index;
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        index--;
                        break;
                    case ConsoleKey.DownArrow:
                        index++;
                        break;
                    case ConsoleKey.Enter:
                        exitLoop = true;
                        break;
                }

                // If the user pressed enter: Break the loop
                if (exitLoop) break;

                // Check if the index is out of bounds
                if (index < 0)
                {
                    if (wrapArround)
                        index = menuItem.Length - 1;
                    else
                        index = 0;
                }
                if (index > menuItem.Length - 1)
                {
                    if (wrapArround)
                        index = 0;
                    else
                        index = menuItem.Length - 1;
                }

                // Rewrite the last selected menu option (in order to avoid to rewrite the entire menu)
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, lastIndex + menuStartConsoleIndex);
                Console.Write(menuItem[lastIndex]);

                // Rewrite the newly selected menu option (--""--)
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(0, index + menuStartConsoleIndex);
                Console.Write(menuItem[index]);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = true;
            return index;
        }

    }
}