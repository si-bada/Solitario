public class ResetCommand : ICommand
{
    private int lastScoreCount = 0;

    public ResetCommand()
    {

    }

    public void Execute()
    {
        EventsManager.Instance.OnReset.Invoke();

        lastScoreCount = UIManager.Instance.Score;

        EventsManager.Instance.OnUndoScore.Invoke(lastScoreCount);
    }

    public void Undo()
    {
        EventsManager.Instance.OnUndoReset.Invoke();

        EventsManager.Instance.OnScore.Invoke(lastScoreCount);
    }
}
