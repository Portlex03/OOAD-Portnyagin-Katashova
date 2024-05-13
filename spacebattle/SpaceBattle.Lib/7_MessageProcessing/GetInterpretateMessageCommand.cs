namespace SpaceBattle.Lib;
using Hwdtech;

public class GetInterpretateMessageCommand : IStrategy
{
    public object Invoke(params object[] args)
    {
        return new InterpretateCommand((IMessage)args[0]);
    }
}
