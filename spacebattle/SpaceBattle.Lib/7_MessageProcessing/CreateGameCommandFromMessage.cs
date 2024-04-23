namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateCommandFromMessageStrategy : IStrategy
{
    public object Execute(params object[] args)
    {
        IMessage message = (IMessage)args[0];

        var obj = IoC.Resolve<IUObject>("SpaceBattle.GetObject", message.gameItemId);

        message.properties.ToList().ForEach(x => obj.SetProperty(x.Key, x.Value));

        return IoC.Resolve<ICommand>("GameCommand." + message.type, obj);
    }
}
