namespace CardGame
{
    internal class Deck
    {
        // Fields
        private readonly List<Card> cards = new();
        private static readonly Random rng = new();

        // Constructors
        public Deck(bool shuffle = false) => Initialize(shuffle);

        // Properties
        public int Remaining { get => cards.Count; }
        
        // Public methods

        public static void Shuffle(List<Card> cards)
        {
            // Fisher-Yates shuffle
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (cards[n], cards[k]) = (cards[k], cards[n]);
            }
        }

        public void Shuffle() => Shuffle(cards);

        public static void Sort(List<Card> cards, bool byValue = false)
        {
            if (!byValue)
            {
                // Sort by Suit => Value
                cards.Sort((X, Y) =>
                {
                    int value = X.Suit.CompareTo(Y.Suit);
                    if (value == 0) value = X.CompareTo(Y);
                    return value;
                });
            }
            else
            {
                // Sort by Value => Suit
                cards.Sort((X, Y) =>
                {
                    int value = X.CompareTo(Y);
                    if (value == 0) value = X.Suit.CompareTo(Y.Suit);
                    return value;
                });
            }
        }

        public void Sort(bool byValue = false) => Sort(cards, byValue);

        public void Print()
        {
            Console.Clear();
            foreach (Card card in cards)
            {
                Console.WriteLine(card);
            }
            Console.ReadKey(true);
        }

        public Card Pick()
        {
            return Pop();
        }
        public List<Card> Pick(int numberOfCards)
        {
            List<Card> list = new();

            for (int i = 0; i < numberOfCards; i++)
            {
                list.Add(Pop());
            }

            return list;
        }

        public void Deal(Hand hand, int numberOfCards = 1)
        {
            // Check for input errors
            if (numberOfCards < 1)
                throw new Exception("numberOfCards cannot be less than 1.");
            if (numberOfCards > cards.Count)
                throw new Exception("The required number of cards does not excist in the deck.");

            // Deal the number of cards to the hand
            for (int i = 0; i < numberOfCards; i++)
            {
                hand.Take(Pop());
            }
        }

        // Private methods
        private void Initialize(bool shuffle)
        {
            // Populate the deck with 52 cards
            for (int Suit = 0; Suit < 4; Suit++)
            {
                for (int Rank = 2; Rank < 15; Rank++)
                {
                    cards.Add(new Card((Card.SuitEnum)Suit, (Card.RankEnum)Rank));
                }
            }

            // Optionally shuffle the deck
            if (shuffle) Shuffle();
        }

        private Card Pop()
        {
            Card card = cards[^1];
            cards.Remove(card);
            return card;
        }

    }
}