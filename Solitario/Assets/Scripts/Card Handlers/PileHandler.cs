using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PileHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Public Variables References
    /// <summary>
    /// The list of this pile children Cards
    /// </summary>
    public List<CardUI> Cards
    {
        get
        {
            return cards;
        }
    }

    /// <summary>
    /// The table area where this pile was set
    /// </summary>
    public CardArea CardArea
    {
        get
        {
            return _cardArea;
        }
    }

    /// <summary>
    /// In case of AcePile, set the suit of the pile
    /// </summary>
    public CardSymbol CardSuit
    {
        get
        {
            return _cardSuit;
        }
    }
    #endregion

    #region Privave Variables References

    [SerializeField]
    private List<CardUI> cards = new List<CardUI>();

    [SerializeField]
    private CardArea _cardArea = CardArea.Table;

    [SerializeField, Header("If this is a Ace pile")]
    private CardSymbol _cardSuit = CardSymbol.Empty;

    /// <summary>
    /// Set a parent reference for the Cards list. if null, the parent will be this object.
    /// </summary>
    [SerializeField]
    private Transform _overrideParent = null;
    #endregion

    private void Start()
    {
        InitEvents();

        if (_overrideParent == null)
            _overrideParent = transform;
    }

    /// <summary>
    /// After all the cards are dealed, save any child Card reference in Cards list
    /// </summary>
    /// <returns></returns>
    private IEnumerator FillCardTable()
    {
        yield return new WaitForSeconds(0.12f);
        Debug.LogWarning("filling card table");
        CardUI[] carduiArray = transform.GetComponentsInChildren<CardUI>();

        for (int i = 0; i < carduiArray.Length; i++)
        {
            CardUI cardui = carduiArray[i];
            cards.Add(cardui);
        }
    }

    /// <summary>
    /// After any UndoCommand, check the right action to do with the first card of the list
    /// </summary>
    /// <param name="undoCard"></param>
    /// <param name="columnAction"></param>
    private void CheckUndoCommand(CardData undoCard, OperationType columnAction)
    {
        if (cards.Count <= 0)
            return;

        CardUI firstCard = cards[cards.Count - 1];

        switch (columnAction)
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

    #region Event System Handlers
    /// <summary>
    /// Call the pointer enter event when this pile is empty
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(cards.Count <= 0)
        {
            EventsManager.Instance.OnPilePointerEnter.Invoke(this);
        }
    }

    /// <summary>
    /// Call the pointer exit event when this pile is empty
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (cards.Count <= 0)
        {
            EventsManager.Instance.OnPilePointerExit.Invoke();
        }
    }
    #endregion

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardDragging.AddListener(HandleEventCardDragging);
        EventsManager.Instance.OnCardMove.AddListener(HandleEventCardMove);
        EventsManager.Instance.OnUndoCardMove.AddListener(HandleEventUndoCardMove);
    }

    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        StartCoroutine(FillCardTable());
    }

    /// <summary>
    /// When the player started to drag a card in top of a stacked pile, append cards and set their UI sorting order
    /// </summary>
    /// <param name="cardui"></param>
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

    /// <summary>
    /// Handle the MoveCommand called event
    /// </summary>
    /// <param name="cardui"></param>
    /// <param name="destinationParent"></param>
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
                cardui.transform.SetParent(_overrideParent);

                // Set the Card CardArea as this Pile Card Area
                cardui.SetCardArea(_cardArea);

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
            cardui.transform.SetParent(_overrideParent);

            cardui.SetCardArea(_cardArea);
        }
    }
    #endregion
}
