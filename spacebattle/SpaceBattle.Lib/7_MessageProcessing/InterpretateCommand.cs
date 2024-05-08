namespace SpaceBattle.Lib;

using System.Data;
using Hwdtech;

public class InterpretateCommand : ICommand
{
    private readonly IMessage _message;
    public InterpretateCommand(IMessage message)
    {
        _message = message;
    }

    public void Execute()
    {
        ICommand getCmd = IoC.Resolve<ICommand>("GetCommand", _message);

        ICommand sendCmd = IoC.Resolve<ICommand>("SendCommandInGame", _message.gameId, getCmd);

        sendCmd.Execute();
    }
}
