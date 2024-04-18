namespace SpaceBattle.Lib;
using Hwdtech;
using System.Diagnostics;

public class GameAsCommand : ICommand
{
    readonly object _scope;
    readonly Queue<ICommand> _q;

    public GameAsCommand(object scope, Queue<ICommand> q)
    {
        _scope = scope;
        _q = q;
    }

    public void Execute()
    {
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        var stopWatch = new Stopwatch();

        stopWatch.Start();
        while (stopWatch.ElapsedMilliseconds <= IoC.Resolve<int>("Game.Quant"))
        {
            if (_q.Count == 0)
                break;
            var cmd = _q.Dequeue();
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                IoC.Resolve<ICommand>("Exception.Handler", ex, cmd).Execute();
            }
        }
        stopWatch.Stop();
    }
}
