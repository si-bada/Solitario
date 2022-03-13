public class CardData
{
    // The numeric rank of the card (1 to 13)
    public int Rank = 0;

    // The symbol of the card (Heart, Diamond, Clubs, Spades)
    public CardSymbol Symbol = CardSymbol.Empty;

    // Save the deck position used in the deck generation
    public int DeckPosition = 0;

    public CardData(int rank, CardSymbol symbol, int deckPosition)
    {
        Rank = rank;
        Symbol = symbol;
        DeckPosition = deckPosition;
    }

    public CardColor GetCardColor()
    {
        if (Symbol == CardSymbol.Clubs || Symbol == CardSymbol.Spades)
        {
            return CardColor.Black;
        }
        if (Symbol == CardSymbol.Hearts || Symbol == CardSymbol.Diamonds)
        {
            return CardColor.Red;
        }

        return CardColor.Black;
    }
}
