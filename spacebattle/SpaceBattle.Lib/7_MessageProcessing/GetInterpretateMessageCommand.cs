namespace SpaceBattle.Lib;
using Hwdtech;

public class GetInterpretateMessageCommand : IStrategy
{
    public object Execute(params object[] args)
    {
        return new InterpretateCommand((IMessage)args[0]);
    }
}
