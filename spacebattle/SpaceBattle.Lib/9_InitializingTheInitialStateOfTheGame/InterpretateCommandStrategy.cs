namespace SpaceBattle.Lib;

using Hwdtech;

public class InterpretateCommandStrategy : IStrategy
{
    public object Invoke(params object[] args)
    {
        IMessage message = (IMessage)args[0];
        IUObject target = IoC.Resolve<IUObject>("SpaceBattle.GetObject", message.gameItemId);
        ICommand command = IoC.Resolve<ICommand>("SpaceBattle." + message.type, target, message.properties);

        return command;
    }
}
