using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Getters
    public CardData CurrentCardData
    {
        get
        {
            return currentCardData;
        }
    }

    public CardSide CurrentSide
    {
        get
        {
            return currentSide;
        }
    }

    public CardArea CardArea
    {
        get
        {
            return cardArea;
        }
    }

    public List<CardUI> CardChilsList
    {
        get
        {
            return cardChildList;
        }
    }

    public PileHandler PileHandlerParent
    {
        get
        {
            return pileHandlerParent;
        }
    }
    #endregion

    #region Script Parameters
    public Image Body;
    public Image BigSymbol;
    public Image SmallSymbol;
    public TextMeshProUGUI Rank;
    public Sprite CardBack;
    public Sprite CardFront;
    public Sprite[] Symbols;
    #endregion

    #region Fields
    private Vector3 dragStartPos = Vector3.zero;
    private Vector3 currentDragPos = Vector3.zero;
    private bool isBeingDragged = false;
    private CardSide currentSide = CardSide.Back;
    private CardArea cardArea = CardArea.Table;
    private CardData currentCardData;
    private Transform currentParent ;
    private List<CardUI> cardChildList = new List<CardUI>();
    private bool isChild = false;
    private PileHandler pileHandlerParent;
    private CanvasGroup canvasGroup = null;
    private Canvas canvas = null;
    private bool beginDragOverrideSorting = false;
    private int beginDragSortingOrder = 0;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        InitEvents();

        if (canvas == null)
            return;

        beginDragOverrideSorting = canvas.overrideSorting;
        beginDragSortingOrder = canvas.sortingOrder;
    }

    private void Update()
    {
        HandleDrag();
    }
    #endregion

    #region Methods
    public void SetCardData(CardData cardData, CardArea cardArea)
    {
        currentCardData = cardData;

        Rank.text = cardData.Rank.ToString();

        //Sprite symbolSprite = Resources.Load<Sprite>("Sprite_" + cardData.Symbol);
        Sprite symbolSprite = Symbols[(int)cardData.Symbol];
        SmallSymbol.sprite = symbolSprite;
        BigSymbol.sprite = symbolSprite;

        switch (cardData.Rank)
        {
            case 1:
                Rank.text = "A";
                break;

            case 11:
                Rank.text = "J";
                break;

            case 12:
                Rank.text = "Q";
                break;

            case 13:
                Rank.text = "K";
                break;
        }

        this.cardArea = cardArea;
        Rank.color = GetColor(cardData.GetCardColor());
    }
    public void SetCardArea(CardArea cardArea)
    {
        this.cardArea = cardArea;
    }
    public void FlipCard(CardSide sideToShow)
    {
        StartCoroutine(FlipAnimation(sideToShow));
    }
    public void UpdateParent(Transform newParent)
    {
        currentParent = newParent;
    }
    public void SetSortingOrder(int sortingOrder)
    {
        if (sortingOrder < 0)
        {
            canvas.overrideSorting = false;
            return;
        }

        canvas.overrideSorting = true;
        canvas.sortingOrder = sortingOrder;
    }
    public void EnableRaycast(bool value)
    {
        canvasGroup.blocksRaycasts = value;
    }
    public void SetStartAppendDragPosition(Vector3 position)
    {
        dragStartPos = position;
    }
    public void SetAppended(bool value)
    {
        isChild = value;
    }
    public void AppendDraggingCards(CardUI cardToAppend)
    {
        cardToAppend.SetAppended(true);
        cardToAppend.SetStartAppendDragPosition(cardToAppend.transform.position);
        cardToAppend.EnableRaycast(false);
        cardChildList.Add(cardToAppend);   
    }
    public void ReleaseAppendedCards()
    {
        for (int i = 0; i < cardChildList.Count; i++)
        {
            CardUI appendedCard = cardChildList[i];
            appendedCard.SetSortingOrder(-1);
            appendedCard.EnableRaycast(true);
        }

        cardChildList.Clear();
    }
    public bool IsLastFrontCardInPile(int cardsPileCount)
    {
        Debug.Log(cardsPileCount);
        Debug.Log("IsLastFrontCardInPile");
        PileHandler pileHandler = GetComponentInParent<PileHandler>();

        if (pileHandler == null || pileHandler.CardArea != CardArea.Table)
            return false;

        if (pileHandler.Cards.Count < 2)
            return false;

        if (pileHandler.Cards.Count == cardsPileCount)
            return false;

        Debug.Log("PileHandler:" + pileHandler.Cards.Count);

        CardUI previousCardInPile = pileHandler.Cards[pileHandler.Cards.IndexOf(this) - 1];

        Debug.Log("previousCardInPile:" + previousCardInPile.CurrentCardData.Rank);

        if (previousCardInPile == null)
            return false;

        if (previousCardInPile != null)
        {
            if (previousCardInPile.CurrentSide == CardSide.Back)
                return true;
        }
        return false;
    }
    #endregion

    #region Unity Events
    public void OnDrag(PointerEventData eventData)
    {
        if (currentSide == CardSide.Back)
            return;

        isBeingDragged = true;
        currentDragPos = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSide == CardSide.Back)
            return;

        EventsManager.Instance.OnCardDragging.Invoke(this);

        // disable the raycast block to prevent the cursor to detect this card when trying to drop it on a different card
        canvasGroup.blocksRaycasts = false;

        dragStartPos = transform.position;

        //set canvas settings
        beginDragOverrideSorting = canvas.overrideSorting;
        beginDragSortingOrder = canvas.sortingOrder;

        // handle sorting order for the cards to be on top of other cards
        canvas.overrideSorting = true;
        canvas.sortingOrder = 5;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentSide == CardSide.Back)
            return;

        Debug.Log("OnEndDrag: " + CurrentCardData.Rank + " of " + CurrentCardData.Symbol);
        EventsManager.Instance.OnCardDropped.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSide == CardSide.Back)
            return;

        EventsManager.Instance.OnCardPointerEnter.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentSide == CardSide.Back)
            return;

        EventsManager.Instance.OnCardPointerExit.Invoke();
    }
    #endregion

    #region Implementations
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardMove.AddListener(HandleCardMove);
        EventsManager.Instance.OnCardFailMove.AddListener(HandleCardFailMove);
    }
    private IEnumerator FlipAnimation(CardSide sideToShow)
    {
        iTween.ScaleTo(gameObject, new Vector3(0, 1, 1), 0.2f);

        yield return new WaitForSeconds(0.1f);

        if(sideToShow == CardSide.Back)
        {
            Body.sprite = CardBack;
        }
        else
        {
            Body.sprite = CardFront;
        }

        switch (sideToShow)
        {
            case CardSide.Back:
                Rank.transform.gameObject.SetActive(false);
                BigSymbol.gameObject.SetActive(false);
                SmallSymbol.gameObject.SetActive(false);
                break;

            case CardSide.Front:
                Rank.transform.gameObject.SetActive(true);
                BigSymbol.gameObject.SetActive(true);
                SmallSymbol.gameObject.SetActive(true);
                break;
        }

        iTween.ScaleTo(gameObject, new Vector3(1, 1, 1), 0.2f);

        currentSide = sideToShow;
    }
    private void HandleDrag()
    {
        if (!isBeingDragged || currentSide == CardSide.Back)
            return;

        // Move the dragging card
        transform.position = Vector3.Lerp(transform.position, currentDragPos, 20 * Time.deltaTime);

        // Move all the appended cards during drag
        if(cardChildList.Count > 0)
        {
            for (int i = 0; i < cardChildList.Count; i++)
            {
                Transform cardTransform = cardChildList[i].transform;

                if(i == 0)
                {
                    cardTransform.position = Vector3.Lerp(cardTransform.position, transform.position + new Vector3(0, -70f, 0), 17 * Time.deltaTime);
                }

                else
                {
                    cardTransform.position = Vector3.Lerp(cardTransform.position, cardChildList[i-1].transform.position + new Vector3(0, -70f, 0), 17 * Time.deltaTime);
                }
            }
        }
    }
    private Color GetColor(CardColor cardColor)
    {
        if (cardColor == CardColor.Red)
            return Color.red;

        if (cardColor == CardColor.Black)
            return Color.black;

        return Color.black;
    }
    private void MoveToEndDragPosition()
    {
        iTween.MoveTo(gameObject, dragStartPos, 0.5f);

        if (isChild)
            isChild = false;
    }
    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        EnableRaycast(true);

        if (!transform.parent.name.Contains("spawnPos"))
            return;

        transform.parent.gameObject.SetActive(false);
        transform.SetParent(currentParent);
    }
    private void HandleCardMove(CardUI guiCard, Transform destinationParent)
    {
        // Check if the the card moved is this
        if (guiCard != this)
            return;

        if (cardChildList.Count > 0)
        {
            for (int i = 0; i < cardChildList.Count; i++)
            {
                CardUI appendedCard = cardChildList[i];
                appendedCard.SetSortingOrder(-1);
            }
        }

        canvas.overrideSorting = false;
        canvas.sortingOrder = 1;
        canvasGroup.blocksRaycasts = true;
        isBeingDragged = false;
    }
    private void HandleCardFailMove(CardUI guiCard)
    {
        if (guiCard != this)
            return;

        MoveToEndDragPosition();

        if (cardChildList.Count > 0)
        {
            for (int i = 0; i < cardChildList.Count; i++)
            {
                CardUI appendedCard = cardChildList[i];
                appendedCard.MoveToEndDragPosition();
            }

            ReleaseAppendedCards();
        }

        canvas.overrideSorting = beginDragOverrideSorting;
        canvas.sortingOrder = beginDragSortingOrder;
        canvasGroup.blocksRaycasts = true;
        isBeingDragged = false;
    }
    #endregion

}
