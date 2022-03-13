using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    #region static
    public static DeckManager Instance;
    #endregion

    #region getters
    public List<CardData> DrawnCards
    {
        get
        {
            return drawnCards;
        }
    }
    #endregion

    #region Fields
    private List<CardData> deckCards = new List<CardData>();
    private List<CardData> drawnCards = new List<CardData>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    private IEnumerator Start()
    {
        InitEvents();

        yield return new WaitForSeconds(1);

        GenerateCards();
    }
    #endregion

    #region Methods
    public bool IsDeckEmpty()
    {
        return deckCards.Count <= 0;
    }
    public CardData DrawCard()
    {
        if(deckCards.Count <= 0)
        {
            ICommand resetCommand = new ResetCommand();
            GameManager.Instance.CommandSystem.AddCommand(resetCommand);
            resetCommand.Execute();
            return null;
        }

        CardData cardToDraw = deckCards[0];
        deckCards.RemoveAt(0);
        drawnCards.Add(cardToDraw);

        ICommand drawCommand = new DrawCommand();
        GameManager.Instance.CommandSystem.AddCommand(drawCommand);
        drawCommand.Execute();

        if(deckCards.Count <= 0)
        {   
            EventsManager.Instance.OnDeckEmpty.Invoke();
        }

        return drawnCards[drawnCards.Count-1];
    }

    public void UndoDrawCard()
    {
        CardData cardToUndo = drawnCards[drawnCards.Count - 1];
        drawnCards.Remove(cardToUndo);
        deckCards.Insert(0, cardToUndo);
    }
    #endregion

    #region Implementations
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardMove.AddListener(HandleEventCardMove);
        EventsManager.Instance.OnPick.AddListener(HandleEventPick);
        EventsManager.Instance.OnUndoPick.AddListener(HandleEventUndoPick);
        EventsManager.Instance.OnUndoDraw.AddListener(HandleEventUndoDraw);
        EventsManager.Instance.OnReset.AddListener(HandleEventReset);
        EventsManager.Instance.OnUndoReset.AddListener(HandleEventUndoReset);
    }
    private void GenerateCards()
    {
        List<CardData> _cardDataList = new List<CardData>();

        // Fill the available suits list. The generation removes one suit each time it generates 13 cards of that suit
        List<CardSymbol> availableSymbols = new List<CardSymbol>();
        availableSymbols.Add(CardSymbol.Clubs);
        availableSymbols.Add(CardSymbol.Diamonds);
        availableSymbols.Add(CardSymbol.Hearts);
        availableSymbols.Add(CardSymbol.Spades);

        // Fill the available deck positions list. The generation removes one position eache time it generates one card in that position
        List<int> availableDeckPositions = new List<int>();

        for (int i = 0; i < 52; i++)
        {
            availableDeckPositions.Add(i);
        }

        while(availableSymbols.Count > 0)
        {
            CardSymbol suitToInit = availableSymbols[availableSymbols.Count - 1];

            for (int i = 1; i < 14; i++)
            {
                int deckPosition = availableDeckPositions[Random.Range(0, availableDeckPositions.Count - 1)];
                CardData cardData = new CardData(i, suitToInit, deckPosition);
                _cardDataList.Add(cardData);

                availableDeckPositions.Remove(deckPosition);
            }

            availableSymbols.Remove(suitToInit);
        }

        _cardDataList = _cardDataList.OrderBy(x => x.DeckPosition).ToList();

        EventsManager.Instance.OnShuffleEnded.Invoke(_cardDataList);
    }
    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        deckCards = cardsData;
    }
    private void HandleEventCardMove(CardUI guiCard, Transform destinationParent)
    {
        // Check if the moved card was in the drawn cards pile
        if (drawnCards.Contains(guiCard.CurrentCardData))
        {
            // Call the Pick Command to save the card drawn pile index
            ICommand pickCommand = new PickCommand(guiCard, drawnCards.IndexOf(guiCard.CurrentCardData));
            GameManager.Instance.CommandSystem.AddCommand(pickCommand);
            pickCommand.Execute();
        }
    }
    private void HandleEventPick(CardUI guiCard)
    {
        drawnCards.Remove(guiCard.CurrentCardData);
    }
    private void HandleEventUndoPick(CardUI guiCard, int drawnCardIndex)
    {
        drawnCards.Insert(drawnCardIndex, guiCard.CurrentCardData);
    }
    private void HandleEventUndoDraw()
    {
        UndoDrawCard();
    }
    private void HandleEventReset()
    {
        for (int i = 0; i < drawnCards.Count; i++)
        {
            CardData drawnCard = drawnCards[i];
            deckCards.Add(drawnCard);
        }

        drawnCards.Clear();
    }
    private void HandleEventUndoReset()
    {
        for (int i = 0; i < deckCards.Count; i++)
        {
            CardData deckCard = deckCards[i];

            drawnCards.Add(deckCard);
        }

        deckCards.Clear();

        EventsManager.Instance.OnDeckEmpty.Invoke();
    }
    #endregion
}
