using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawCardsHandler : MonoBehaviour, IPointerDownHandler
{
    public GameObject CardUIPrefab;
    public Transform[] CardDeckPositions;
    public List<CardUI> CardUIPile = new List<CardUI>();
    public Transform DrawDeckParent;
    public Sprite CardBack;
    public Sprite CardTransparent;
    [SerializeField]
    private Image DeckImage;

    private List<CardData> _hiddenCards = new List<CardData>();

    private int _pileCounter = 0;

    private bool canDraw = false;

    private void Start()
    {
        InitEvents();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canDraw)
            return;

        DrawCard();
    }

    private void DrawCard()
    {
        CardData drawCardData = DeckManager.Instance.DrawCard();

        if (drawCardData == null)
            return;

        RefillDrawPile();

        CardUI cardToDraw;
        if (_pileCounter >= 3)
        {
            cardToDraw = CardUIPile[_pileCounter];
            cardToDraw.SetCardData(drawCardData, CardArea.DrawPile);
            cardToDraw.gameObject.SetActive(true);
            cardToDraw.FlipCard(CardSide.Front);
            cardToDraw.SetSortingOrder(4);
            cardToDraw.EnableRaycast(true);
            iTween.MoveTo(cardToDraw.gameObject, CardDeckPositions[2].position, 0.4f);
        }
        else
        {
            cardToDraw = CardUIPile[_pileCounter];
            cardToDraw.gameObject.SetActive(true);
            cardToDraw.SetCardData(drawCardData, CardArea.DrawPile);
            cardToDraw.FlipCard(CardSide.Front);
            cardToDraw.SetSortingOrder(_pileCounter + 1);
            cardToDraw.EnableRaycast(true);
            iTween.MoveTo(cardToDraw.gameObject, CardDeckPositions[_pileCounter].position, 0.4f);
        }

        ShiftToLeft();
    }

    private void ShiftToRight() 
    {
        RefillDrawPile();

        if (_hiddenCards.Count > 0)
        {
            CardUI hiddenCard = null;

            // Use one disabled UICard to the hidden card
            for (int i = 0; i < CardUIPile.Count; i++)
            {
                CardUI card = CardUIPile[i];
                if (!CardUIPile[i].gameObject.activeInHierarchy)
                    hiddenCard = card;
            }

            iTween.MoveTo(hiddenCard.gameObject, CardDeckPositions[0].position, 0.1f);
            hiddenCard.gameObject.SetActive(true);
            hiddenCard.SetCardData(_hiddenCards[_hiddenCards.Count - 1], CardArea.DrawPile);
            hiddenCard.FlipCard(CardSide.Front);
            hiddenCard.SetSortingOrder(1);
            hiddenCard.EnableRaycast(false);
            _hiddenCards.Remove(hiddenCard.CurrentCardData);

            CardUIPile[0].SetSortingOrder(2);
            CardUIPile[0].EnableRaycast(false);
            iTween.MoveTo(CardUIPile[0].gameObject, CardDeckPositions[1].position, 0.4f);

            CardUIPile[1].SetSortingOrder(3);
            CardUIPile[1].EnableRaycast(true);
            iTween.MoveTo(CardUIPile[1].gameObject, CardDeckPositions[2].position, 0.4f);

            /// Shift the last card reference in the list to be the first
            CardUI listCardShift = CardUIPile[CardUIPile.Count - 1];
            CardUIPile.Remove(listCardShift);
            CardUIPile.Insert(0, listCardShift);
        }
        else
        {
            _pileCounter--;

            if(_pileCounter > 0)
            {
                CardUIPile[_pileCounter -1].EnableRaycast(true);
            }
        }
    }

    private void ShiftToLeft()
    {
        RefillDrawPile();

        if (_pileCounter >= 3)
        {
            CardUIPile[1].SetSortingOrder(1);
            CardUIPile[1].EnableRaycast(false);
            iTween.MoveTo(CardUIPile[1].gameObject, CardDeckPositions[0].position, 0.4f);

            CardUIPile[2].SetSortingOrder(2);
            CardUIPile[2].EnableRaycast(false);
            iTween.MoveTo(CardUIPile[2].gameObject, CardDeckPositions[1].position, 0.4f);

            CardUI cardToHide = CardUIPile[0];
            cardToHide.gameObject.transform.position = DrawDeckParent.position;
            cardToHide.gameObject.SetActive(false);
            CardUIPile.Remove(cardToHide);
            CardUIPile.Add(cardToHide);
            _hiddenCards.Add(cardToHide.CurrentCardData);
        }
        else
        {
            if (_pileCounter > 0)
            {
                CardUIPile[_pileCounter - 1].EnableRaycast(false);   
            }

            _pileCounter++;
        }
    }

    private IEnumerator PutCardOnDeck()
    {
        CardUI shiftInDeckCard = CardUIPile[_pileCounter];
        shiftInDeckCard.FlipCard(CardSide.Back);

        iTween.MoveTo(shiftInDeckCard.gameObject, DrawDeckParent.position, 0.4f);

        yield return new WaitForSeconds(0.4f);

        shiftInDeckCard.gameObject.SetActive(false);
    }

    private IEnumerator ResetPile(int pileToReset)
    {
        if (CardUIPile[pileToReset].gameObject.activeInHierarchy)
        {
            iTween.MoveTo(CardUIPile[pileToReset].gameObject, DrawDeckParent.position, 0.4f);
            CardUIPile[pileToReset].FlipCard(CardSide.Back);
            yield return new WaitForSeconds(0.4f);
            CardUIPile[pileToReset].gameObject.SetActive(false);
        }
    }

    private IEnumerator UndoResetPile(int pileToUndo)
    {
        CardUI cardToUndo = CardUIPile[pileToUndo];
        cardToUndo.gameObject.SetActive(true);
        cardToUndo.SetCardData(DeckManager.Instance.DrawnCards[(DeckManager.Instance.DrawnCards.Count - 1) - (2 - pileToUndo)], CardArea.DrawPile);
        cardToUndo.FlipCard(CardSide.Front);
        cardToUndo.SetSortingOrder(pileToUndo + 1);
        iTween.MoveTo(cardToUndo.gameObject, CardDeckPositions[pileToUndo].position, 0.4f);

        yield return null;
    }

    private void RefillDrawPile()
    {
        // Check if the current pile cards in one less, due to a previous pick command
        if (CardUIPile.Count <= 3)
        {
            // Check if there is the minumum amount of card reference in the cards list to prevent null references
            while (CardUIPile.Count < 4)
            {
                CardUI newCard = Instantiate(CardUIPrefab.GetComponent<CardUI>(), DrawDeckParent);
                CardUIPile.Add(newCard);
                newCard.gameObject.SetActive(false);
            }
        }
    }

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardMove.AddListener(HandleEventCardMove);
        EventsManager.Instance.OnUndoCardMove.AddListener(HandleEventUndoCardMove);
        EventsManager.Instance.OnUndoDraw.AddListener(HandleEventUndoDraw);
        EventsManager.Instance.OnDeckEmpty.AddListener(HandleEventDeckEmpty);
        EventsManager.Instance.OnReset.AddListener(HandleEventReset);
        EventsManager.Instance.OnUndoReset.AddListener(HandleEventUndoReset);
    }

    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        canDraw = true;
    }

    private void HandleEventCardMove(CardUI cardui, Transform destinationParent)
    {
        if (!CardUIPile.Contains(cardui))
            return;

        CardUIPile.Remove(cardui);
        ShiftToRight();
    }

    private void HandleEventUndoCardMove(CardUI cardui, Transform sourceParent)
    {
        if (sourceParent.name != DrawDeckParent.name)
            return;

        ShiftToLeft();
        CardUIPile.Insert(_pileCounter - 1, cardui);
        iTween.MoveTo(cardui.gameObject, CardDeckPositions[_pileCounter-1].position, 0.4f);
        cardui.transform.SetParent(DrawDeckParent);
        cardui.SetSortingOrder(_pileCounter);
        cardui.SetCardArea(CardArea.DrawPile);
    }

    private void HandleEventUndoDraw()
    {
        DeckImage.sprite = CardBack;

        ShiftToRight();
        StartCoroutine(PutCardOnDeck());
    }

    private void HandleEventDeckEmpty()
    {
        DeckImage.sprite = CardTransparent;
    }

    private void HandleEventReset()
    {
        //Debug.Log("HandleEventReset");

        DeckImage.sprite = CardBack;

        StartCoroutine(ResetPile(0));
        StartCoroutine(ResetPile(1));
        StartCoroutine(ResetPile(2));

        _hiddenCards.Clear();
        _pileCounter = 0;
    }

    private void HandleEventUndoReset()
    {
        //Debug.Log("HandleEventUndoReset");

        DeckImage.sprite = CardTransparent;

        StartCoroutine(UndoResetPile(0));
        StartCoroutine(UndoResetPile(1));
        StartCoroutine(UndoResetPile(2));

        int totalCardsActive = 0;

        for (int i = 0; i < DrawDeckParent.childCount; i++)
        {
            GameObject cardObj = DrawDeckParent.GetChild(i).gameObject;
            if (cardObj.activeInHierarchy)
                totalCardsActive++;
        }

        for (int i = 0; i < DeckManager.Instance.DrawnCards.Count - 3; i++)
        {
            CardData drawCard = DeckManager.Instance.DrawnCards[i];
            _hiddenCards.Add(drawCard);
        }

        _pileCounter = totalCardsActive;
    }
    #endregion
}
