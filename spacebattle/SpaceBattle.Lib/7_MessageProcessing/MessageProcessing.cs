namespace SpaceBattle.Lib;
using Hwdtech;

public class MessageProcessing : ICommand
{
    public void Execute()
    {
        IMessage message = IoC.Resolve<IMessage>("GetMessage");

        ICommand interpretcmd = IoC.Resolve<ICommand>("GetInterpretateMessageCommand", message);

        ICommand sendCmd = IoC.Resolve<ICommand>("SendCommandInGame", message.gameId, interpretcmd);

        sendCmd.Execute();
    }
}
