using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PileHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Getters
    public List<CardUI> Cards
    {
        get
        {
            return cards;
        }
    }
    public CardArea CardArea
    {
        get
        {
            return cardArea;
        }
    }
    public CardSymbol PileSymbol
    {
        get
        {
            return cardSymbol;
        }
    }
    public Transform StackPoint
    {
        get
        {
            return overrideParent;
        }
    }
    #endregion

    #region Script Parameters

    [SerializeField]
    private List<CardUI> cards = new List<CardUI>();

    [SerializeField]
    private CardArea cardArea = CardArea.Table;

    [SerializeField, Header("If this is a Ace pile")]
    private CardSymbol cardSymbol = CardSymbol.Empty;

    [SerializeField]
    private Transform overrideParent = null;
    #endregion

    #region Unity Methods
    private void Start()
    {
        InitEvents();
        if (overrideParent == null)
            overrideParent = transform;
    }
    #endregion

    #region Implmentations
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardDragging.AddListener(HandleEventCardDragging);
        EventsManager.Instance.OnCardMove.AddListener(HandleEventCardMove);
        EventsManager.Instance.OnUndoCardMove.AddListener(HandleEventUndoCardMove);
    }

    private IEnumerator FillCardTable()
    {
        yield return new WaitForSeconds(0.15f);
        CardUI[] carduiArray = transform.GetComponentsInChildren<CardUI>();

        for (int i = 0; i < carduiArray.Length; i++)
        {
            CardUI cardui = carduiArray[i];
            cards.Add(cardui);
        }
    }

    private void CheckUndoCommand(CardData undoCard, OperationType action)
    {
        if (cards.Count <= 0)
            return;

        CardUI firstCard = cards[cards.Count - 1];

        switch (action)
        {
            case OperationType.Add:

                if (cards.Count > 1)
                {
                    // Check if the second card is front sided too.
                    CardUI secondCard = cards[cards.Count - 2];

                    if (secondCard.CurrentSide == CardSide.Front)
                        return;
                }

                //If there is no second card front sided, then first one has to be back side
                if (firstCard.CurrentSide == CardSide.Front)
                {
                    if (firstCard.CurrentCardData.Rank - undoCard.Rank == 1 && firstCard.CurrentCardData.Symbol != undoCard.Symbol)
                        return;

                    firstCard.FlipCard(CardSide.Back);
                }
                break;

            case OperationType.Remove:
                if (firstCard.CurrentSide == CardSide.Back)
                {
                    firstCard.FlipCard(CardSide.Front);
                }
                break;
        }
    }

    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        StartCoroutine(FillCardTable());
    }

    private void HandleEventCardDragging(CardUI cardui)
    {
        if (!cards.Contains(cardui))
            return;

        int draggingCardIndex = cards.IndexOf(cardui);

        // for list count, check if index + 1 has a card ref
        for (int i = draggingCardIndex + 1; i < cards.Count; i++)
        {
            CardUI hangingCard = cards[i];

            if (hangingCard == null)
                return;

            hangingCard.SetSortingOrder(5 + draggingCardIndex+i);

            cardui.AppendDraggingCards(hangingCard);
        }
    }
    private void HandleEventCardMove(CardUI cardui, Transform destinationParent)
    {
        // If the moved card reference was saved in the Cards list, remove it 
        if (cards.Contains(cardui))
        {
            cards.Remove(cardui);

            if (cards.Count <= 0)
                return;

            // If the first was faced back, turn it front side
            CardUI firstCard = cards[cards.Count - 1];

            if (firstCard.CurrentSide == CardSide.Back)
            {
                firstCard.FlipCard(CardSide.Front);
            }
        }
        else
        {
            // If the moved card reference has this pile as move destination, set this pile as its parent and add it to Cards list
            if (destinationParent.GetComponent<PileHandler>() == this || destinationParent.GetComponentInParent<PileHandler>() == this)
            {
                cards.Add(cardui);
                cardui.transform.SetParent(overrideParent);

                // Set the Card CardArea as this Pile Card Area
                cardui.SetCardArea(cardArea);

                // If this pile is one of the AcePile, add one unit of the CompletedAcePileCount to detect the win conditoin
                if(CardArea == CardArea.AcesPile)
                {
                    if(cards.Count >= 13)
                    {
                        GameManager.Instance.UpdateCompletedAcePileCount(OperationType.Add);
                    }
                }
            }
        }
    }

    private void HandleEventUndoCardMove(CardUI cardui, Transform sourceParent)
    {
        // If the Cards list contained the undo card, remove it
        if (cards.Contains(cardui))
        {
            // If this pile is one of the AcePile and the pile was completed (13 cards) remove one unit of the CompletedAcePileCount to detect the win conditoin
            if (CardArea == CardArea.AcesPile)
            {
                if (cards.Count >= 13)
                {
                    GameManager.Instance.UpdateCompletedAcePileCount(OperationType.Remove);
                }
            }

            cards.Remove(cardui);

            CheckUndoCommand(cardui.CurrentCardData, OperationType.Remove);
        }

        if(sourceParent.GetComponent<PileHandler>() == this)
        {
            CheckUndoCommand(cardui.CurrentCardData, OperationType.Add);

            cards.Add(cardui);
            cardui.transform.SetParent(overrideParent);

            cardui.SetCardArea(cardArea);
        }
    }
    #endregion

    #region Event System Handlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cards.Count <= 0)
        {
            EventsManager.Instance.OnPilePointerEnter.Invoke(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cards.Count <= 0)
        {
            EventsManager.Instance.OnPilePointerExit.Invoke();
        }
    }
    #endregion
}
