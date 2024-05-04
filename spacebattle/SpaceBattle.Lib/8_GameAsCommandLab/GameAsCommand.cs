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
        _stopwatch.Reset();
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();
        while (_q.Count > 0 && _stopwatch.ElapsedMilliseconds <= IoC.Resolve<int>("Game.Quant"))
        {
            _stopwatch.Start();
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
    }
}
