namespace SpaceBattle.Lib;
using Hwdtech;

public class DeleteGameStrategy : IStrategy
{
    public object Invoke(params object[] args)
    {
        ICommand returnCommand = IoC.Resolve<ICommand>("Games.DeleteGame", args[0]);
        return returnCommand;
    }
}
