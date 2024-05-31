namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateGameStrategy : IStrategy
{
    public object Invoke(params object[] args)
    {
        object scope = new InitGameScopeStrategy().Invoke(args[0]);
        object oldScope = IoC.Resolve<object>("Scopes.Current");
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        Queue<ICommand> queue = IoC.Resolve<Queue<ICommand>>("SpaceBattle.GetQueue");
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();
        ICommand returnCommand = IoC.Resolve<ICommand>("Commands.GameAsCommand", scope, queue);

        return returnCommand;
    }
}
