namespace SpaceBattle.Lib;

using System.Collections.Concurrent;
using Hwdtech;

public class ServerThread
{
    BlockingCollection<ICommand> _q;
    Action _behaviour;
    Thread _t;
    bool _stop = false;

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
    public bool IsAlive { get => _stop == false; }

    public void Start() => _t.Start();

    public void Stop() => _stop = true;

    public Action GetBehaviour() => _behaviour;

    public void UpdateBehaviour(Action newBehaviour) => _behaviour = newBehaviour;
}
