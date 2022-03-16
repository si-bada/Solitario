using System.Collections.Generic;

public class CommandSystem
{
    #region Fields
    private List<ICommand> _commandList = new List<ICommand>();
    #endregion

    #region Methods
    public void AddCommand(ICommand command)
    {
        _commandList.Add(command);

        EventsManager.Instance.OnCommand.Invoke();
    }

    public void UndoCommand()
    {
        if (_commandList.Count == 0)
            return;

        AudioManager.Instance.PlayOneShot("UndoCommand_SFX");

        EventsManager.Instance.OnCommand.Invoke();

        ICommand lastCommand = _commandList[_commandList.Count - 1];

        if (lastCommand is MoveCommand)
        {
            MoveCommand moveCommand = (MoveCommand)lastCommand;

            // If last command was a MoveCommand called by multiple dragging cards, undo every multiple MoveCommand
            if(moveCommand.IsMultipleMove)
            {
                List<MoveCommand> mulitpleMoveCommands = new List<MoveCommand>();

                for (int i = _commandList.Count - 1; i > 0; i--)
                {
                    MoveCommand multipleMoveCommand = _commandList[i] as MoveCommand;

                    if (multipleMoveCommand == null)
                        break;

                    if (multipleMoveCommand.IsMultipleMove)
                    {
                        mulitpleMoveCommands.Insert(0, multipleMoveCommand);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int i = 0; i < mulitpleMoveCommands.Count; i++)
                {
                    MoveCommand command = mulitpleMoveCommands[i];
                    command.Undo();
                    _commandList.Remove(command);
                }
            }
            else
            {
                lastCommand.Undo();
                _commandList.Remove(lastCommand);
            }
        }

        // If last undo command was a pick command, undo also the move command previous to it, called by the card move
        if (lastCommand is PickCommand)
        {
            if (_commandList[_commandList.Count - 2] is MoveCommand)
            {
                _commandList[_commandList.Count - 2].Undo();
                _commandList.Remove(_commandList[_commandList.Count - 2]);
            }

            lastCommand.Undo();
            _commandList.Remove(lastCommand);
        }

        else
        {
            lastCommand.Undo();
            _commandList.Remove(lastCommand);
        }
    }
    #endregion
}

