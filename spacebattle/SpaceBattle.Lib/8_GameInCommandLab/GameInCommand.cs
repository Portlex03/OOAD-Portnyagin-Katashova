namespace SpaceBattle.Lib;
using Hwdtech;

public class GameInCommand : ICommand
{
    object _scope;
    Queue<ICommand> _q;

    public GameInCommand(object scope, Queue<ICommand> q)
    {
        _scope = scope;
        _q = q;
    }

    public void Execute()
    {
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        IoC.Resolve<ICommand>("Game.ExecuteCommands", _q).Execute();
    }
}