using UnityEngine;

public class MoveCommand : ICommand
{
    #region Parameters
    public bool IsMultipleMove = false;
    #endregion

    #region Fields
    private CardUI cardui = null;
    private Transform _destinationParent = null;
    private Transform _sourceParent = null;
    private CardArea _sourceArea;
    private int _moveScore = 0;
    #endregion

    #region Methods
    public MoveCommand(CardUI cardui, Transform destinationParent, bool _isMultipleMove, int moveScore)
    {
        this.cardui = cardui;
        _destinationParent = destinationParent;

        _sourceParent = cardui.transform.parent;
        _sourceArea = cardui.CardArea;

        IsMultipleMove = _isMultipleMove;

        _moveScore = moveScore;
    }

    public void Execute()
    {
        AudioManager.Instance.PlayOneShot("MoveCommand_SFX");

        EventsManager.Instance.OnCardMove.Invoke(cardui, _destinationParent);

        EventsManager.Instance.OnScore.Invoke(_moveScore);
    }

    public void Undo()
    {
        EventsManager.Instance.OnUndoCardMove.Invoke(cardui, _sourceParent);
        EventsManager.Instance.OnUndoScore.Invoke(_moveScore);
        cardui.SetCardArea(_sourceArea);
    }
    #endregion
}
