namespace CardGame
{
    internal class Card : IComparable<Card>
    {
        // Enums
        public enum SuitEnum
        {
            Club,
            Diamond,
            Heart,
            Spade,
        }

        public enum RankEnum
        {
            Two = 2,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace,
        }

        // Properties
        public SuitEnum Suit { get; private set; }

        public RankEnum Rank { get; private set; }

        public int Value { get => OverrideValue == -1 ? (int)Rank : OverrideValue; }

        public int OverrideValue { get; set; } = -1;

        // Constructors
        public Card(SuitEnum suit, RankEnum rank)
        {
            Suit = suit;
            Rank = rank;
        }

        // Public methods
        public int CompareTo(Card? other)
        {
            if (other == null) return 0;
            return Value.CompareTo(other.Value);
        }

        public override string ToString() => $"{Rank} of {Suit}s";

        public string ToString(bool numericFormat)
        {
            if (!numericFormat) return ToString();

            string result = Value switch
            {
                11 => "J",
                12 => "Q",
                13 => "K",
                14 => "A",
                _ => Value.ToString(),
            };

            return $"{result} of {Suit}";
        }

    }
}