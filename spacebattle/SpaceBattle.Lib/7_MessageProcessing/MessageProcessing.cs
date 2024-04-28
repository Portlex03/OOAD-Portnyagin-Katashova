namespace SpaceBattle.Lib;
using Hwdtech;

public class MessageProcessing : ICommand
{
    public void Execute()
    {
        IMessage message = IoC.Resolve<IMessage>("GetMessage") ?? throw new NullCommandException();

        ICommand interpretcmd = IoC.Resolve<ICommand>("GetInterpretateMessageCommand", message) ?? throw new NullCommandException();

        ICommand sendCmd = IoC.Resolve<ICommand>("SendCommandInGame", message.gameId, interpretcmd) ?? throw new NullCommandException();

        sendCmd.Execute();
    }
}
