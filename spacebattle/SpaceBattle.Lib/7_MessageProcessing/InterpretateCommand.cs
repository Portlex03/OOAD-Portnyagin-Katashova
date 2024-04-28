namespace SpaceBattle.Lib;

using System.Data;
using Hwdtech;

public class InterpretateCommand : ICommand
{
    private readonly IMessage _message;
    public InterpretateCommand(IMessage message)
    {
        _message = message ?? throw new NullMessageException();
    }
    public void Execute()
    {
        ICommand getCmd = IoC.Resolve<ICommand>("GetCommand", _message) ?? throw new NullCommandException();

        ICommand sendCmd = IoC.Resolve<ICommand>("SendCommandInGame", _message.gameId, getCmd) ?? throw new NullCommandException();

        sendCmd.Execute();
    }
}
