using Hwdtech;
using System.Diagnostics;

namespace SpaceBattle.Lib;

public class GameAsCommand : ICommand
{
    readonly object _scope;
    readonly Queue<ICommand> _q;
    readonly Stopwatch _stopwatch;

    public GameAsCommand(object scope, Queue<ICommand> q)
    {
        _scope = scope;
        _q = q;
        _stopwatch = new();
    }

    public void Execute()
    {
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();
        while (_stopwatch.ElapsedMilliseconds <= IoC.Resolve<int>("Game.Quant"))
        {
            _stopwatch.Start();
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
            _stopwatch.Stop();
        }
        _stopwatch.Reset();
    }
}
