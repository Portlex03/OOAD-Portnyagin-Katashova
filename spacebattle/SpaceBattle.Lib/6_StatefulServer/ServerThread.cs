namespace SpaceBattle.Lib;

using System.Collections.Concurrent;
using Hwdtech;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _q;
    private Action _behaviour;
    private readonly Thread _t;
    private bool _stop = false;

    public ServerThread(BlockingCollection<ICommand> q)
    {
        _q = q;

        _behaviour = () =>
        {
            var cmd = q.Take();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<ICommand>("Exception.Handler", cmd, e).Execute();
            }
        };

        _t = new Thread(() =>
        {
            while (!_stop)
                _behaviour();
        });
    }
    public bool QueueIsEmpty { get => _q.Count == 0; }

    public void Start() => _t.Start();

    internal void Stop() => _stop = true;

    public Action GetBehaviour() => _behaviour;

    internal void UpdateBehaviour(Action newBehaviour) => _behaviour = newBehaviour;

    public override bool Equals(object? obj)
    {
        return obj != null && obj is Thread thread && _t == thread;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
