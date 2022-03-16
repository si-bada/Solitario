using System;
using UnityEngine;

public class MoveSystem 
{
    #region Fields
    private CardUI draggedCard = null;
    
    private CardUI endpointCard = null;
    
    private PileHandler endpointPile = null;

    private Transform endpointParent = null;

    private CardData draggedCardData = null;

    private CardData endpointCardData = null;
    #endregion

    #region Methods
    public MoveSystem()
    {
        InitEvents();
    }
    public void CheckDoubleClick(CardUI cardui)
    {
        bool canMove = false;
        draggedCard = cardui;
        var acePiles = PilesManager.Instance.AcesPiles;
        int i;
        for (i = 0; i < acePiles.Length; i++)
        {
            if (draggedCard.CurrentCardData.Symbol == acePiles[i].PileSymbol)
            {
                canMove = PossibleMove(draggedCard, acePiles[i]);
                break;
            }
        }
        if (canMove)
        {
            MoveCommand(draggedCard, acePiles[i].transform, false, 10);
            //draggedCard.transform.SetParent(acePiles[i].StackPoint);
            iTween.MoveTo(draggedCard.gameObject, acePiles[i].StackPoint.position, 1f);
            draggedCard = null;
            endpointCard = null;
            endpointPile = null;
            return;
        }
    }
    public void CheckMove()
    {
        if (draggedCard == null || endpointCard == null && endpointPile == null)
        {
            CardFailMove();
            return;
        }

        draggedCardData = draggedCard.CurrentCardData;
        
        // Check move done on another table card
        if(endpointCard != null)
        {
            endpointCardData = endpointCard.CurrentCardData;
            endpointParent = endpointCard.transform.parent;

            // Check Draw Pile Move
            if (endpointCard.CardArea == CardArea.DrawPile)
            {
                CardFailMove();
                return;
            }

            // Check Aces Pile move
            if(endpointCard.CardArea == CardArea.AcesPile)
            {
                if(draggedCardData.Symbol != endpointCardData.Symbol)
                {
                    CardFailMove();
                    return;
                }

                if(draggedCardData.Rank - endpointCardData.Rank != 1)
                {
                    CardFailMove();
                    return;
                }

                MoveCommand(draggedCard, endpointParent, false, 10);
                draggedCard = null;
                endpointCard = null;
                endpointPile = null;
                return;
            }

            if (draggedCard.transform.parent == endpointCard.transform.parent)
            {
                CardFailMove();
                return;
            }

            if (draggedCardData.GetCardColor() == CardColor.Black && endpointCardData.GetCardColor() == CardColor.Black
                || draggedCardData.GetCardColor() == CardColor.Red && endpointCardData.GetCardColor() == CardColor.Red)
            {
                CardFailMove();
                return;
            }

            if (draggedCardData.Rank > endpointCardData.Rank || endpointCardData.Rank - draggedCardData.Rank != 1)
            {
                CardFailMove();
                return;
            }
        } 

        // Check empty Table Pile Move
        if(endpointPile != null)
        {
            endpointParent = endpointPile.transform;

            if(endpointPile.CardArea == CardArea.Table)
            {
                // You can only place Kings on on an empty table pile
                if (draggedCardData.Rank != 13)
                {
                    CardFailMove();
                    return;
                }
            }

            if(endpointPile.CardArea == CardArea.AcesPile)
            {
                if(endpointPile.PileSymbol != draggedCardData.Symbol)
                {
                    CardFailMove();
                    return;
                }
                if(draggedCardData.Rank != 1)
                {
                    CardFailMove();
                    return;
                }
                else
                {
                    // Check if I am trying to position again a 1 of Ace
                    if (endpointPile.Cards.Contains(draggedCard))
                    {
                        CardFailMove();
                        return;
                    }

                    MoveCommand(draggedCard, endpointParent, false, 10);

                    draggedCard = null;
                    endpointCard = null;
                    endpointPile = null;
                    return;
                }
            }
        }

        // Check for multiple cards dragging
        if (draggedCard.CardChilsList.Count > 0)
        {
            // Set the score of the next move
            int moveScore = 0;

            // Check if the last moved top card had a hidden card previous in his list. If so, add 5 points
            if (draggedCard.IsLastFrontCardInPile(draggedCard.CardChilsList.Count + 1))
                moveScore = 5;

            // Move the first card of the dragging cards list
            MoveCommand(draggedCard, endpointParent, true, moveScore);

            for (int i = 0; i < draggedCard.CardChilsList.Count; i++)
            {
                CardUI appendedCard = draggedCard.CardChilsList[i];

                MoveCommand(appendedCard, endpointParent, true, 0);
            }

            // Clear the appended card list
            draggedCard.ReleaseAppendedCards();
        }
        else
        {
            int moveScore = 0;

            if (draggedCard.IsLastFrontCardInPile(1))
                moveScore = 5;

            MoveCommand(draggedCard, endpointParent, false, moveScore);
        }

        draggedCard = null;
        endpointCard = null;
        endpointPile = null;
    }
    #endregion

    #region Implementations
    private void MoveCommand(CardUI movedCard, Transform destinationParent, bool isMultipleMove, int moveScore)
    {
        ICommand moveCommand = new MoveCommand(movedCard, destinationParent, isMultipleMove, moveScore);
        GameManager.Instance.CommandSystem.AddCommand(moveCommand);
        moveCommand.Execute();
    }
    private void CardFailMove()
    {
        EventsManager.Instance.OnCardFailMove.Invoke(draggedCard);
        draggedCard = null;
        endpointCard = null;
    }
    private void InitEvents()
    {
        EventsManager.Instance.OnCardDragging.AddListener(HandleEventCardDragging);
        EventsManager.Instance.OnCardDropped.AddListener(HandleEventCardDropped);
        EventsManager.Instance.OnCardPointerEnter.AddListener(HandleEventCardPointerEnter);
        EventsManager.Instance.OnCardPointerExit.AddListener(HandleEventCardPointerExit);
        EventsManager.Instance.OnPilePointerEnter.AddListener(HandleEventPilePointerEnter);
        EventsManager.Instance.OnPilePointerExit.AddListener(HandleEventPilePointerExit);
        EventsManager.Instance.OnCardDoubleClick.AddListener(HandleEventCardDoubleClick);
    }

    private void HandleEventCardDoubleClick(CardUI cardui)
    {
        CheckDoubleClick(cardui);
    }

    private bool PossibleMove(CardUI cardui, PileHandler pile)
    {
        endpointCard = cardui;
        return cardui.CurrentCardData.Rank == (pile.Cards.Count + 1);
    }

    private void HandleEventCardDragging(CardUI cardui)
    {
        draggedCard = cardui;
    }
    private void HandleEventCardDropped()
    {
        CheckMove();
    }
    private void HandleEventCardPointerEnter(CardUI cardui)
    {
        endpointCard = cardui;
    }
    private void HandleEventCardPointerExit()
    {
        endpointCard = null;
    }
    private void HandleEventPilePointerEnter(PileHandler tablePileHandler)
    {
        endpointPile = tablePileHandler;
    }
    private void HandleEventPilePointerExit()
    {
        endpointPile = null;
    }
    #endregion
}
