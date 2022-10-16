namespace CardGame
{
    internal class Hand
    {
        // Fields
        private readonly List<Card> cards = new();

        // Constructors
        public Hand() { }

        public Hand(Deck deck, int numberOfCards) => deck.Deal(this, numberOfCards);

        // Indexer
        public Card this[int index]
        {
            get { return cards[index]; }
            set { cards[index] = value; }
        }

        // Properties
        public int Count { get => cards.Count; }

        public List<Card> Cards { get => cards; }

        // Public methods
        public void Shuffle() => Deck.Shuffle(cards);

        public void Sort(bool byValue = false) => Deck.Sort(cards, byValue);

        public int GetTotalValue()
        {
            int result = 0;

            foreach (Card card in cards)
            {
                result += card.Value;
            }

            return result;
        }

        public void Take(Card card) => cards.Add(card);

        public void Take(Deck deck) => deck.Deal(this);

        public override string ToString() => ToString(false);

        public string ToString(bool numericFormat)
        {
            string result = "";
            for (int i = 0; i < Count; i++)
            {
                result += cards[i].ToString(numericFormat);
                if (i != Count - 1) result += ", ";
            }
            return result;
        }

    }
}