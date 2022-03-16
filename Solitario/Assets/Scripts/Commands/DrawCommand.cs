public class DrawCommand : ICommand
{
    public DrawCommand()
    {

    }

    public void Execute()
    {
        AudioManager.Instance.Play("MoveCommand_SFX");
    }

    public void Undo()
    {
        if(GameManager.Instance.DrawMode == DrawMode.One)
        {
            EventsManager.Instance.OnUndoDraw.Invoke();
        }
        else
        {
            //code for undo 3 moves
            EventsManager.Instance.OnUndoDraw.Invoke();
        }
    }
}
