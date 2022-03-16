public class PickCommand : ICommand
{
    private CardUI carduiRef = null;
    private int drawnCardsIndex = 0;

    public PickCommand(CardUI carduiRef, int drawnCardsIndex)
    {
        this.carduiRef = carduiRef;
        this.drawnCardsIndex = drawnCardsIndex;
    }

    public void Execute()
    {
        EventsManager.Instance.OnPick.Invoke(carduiRef);
        EventsManager.Instance.OnScore.Invoke(5);
    }

    public void Undo()
    {
        EventsManager.Instance.OnUndoPick.Invoke(carduiRef, drawnCardsIndex);
        EventsManager.Instance.OnUndoScore.Invoke(5);
    }
}
