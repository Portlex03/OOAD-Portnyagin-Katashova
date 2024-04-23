namespace SpaceBattle.Lib;
using Hwdtech;

public class InterpretCommand : ICommand 
{
    private readonly IMessage _message;

    public InterpretCommand(IMessage message) => _message = message;

    public void Execute()
    {
        var cmd = IoC.Resolve<ICommand>("GameCommandCreateFromMessage", _message);

        IoC.Resolve<ICommand>("SpaceBattle.QueuePush", _message.gameId, cmd).Execute();
    }
}
