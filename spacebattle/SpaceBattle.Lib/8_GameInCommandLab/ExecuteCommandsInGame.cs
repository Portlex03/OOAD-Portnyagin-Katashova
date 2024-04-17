namespace SpaceBattle.Lib;
using Hwdtech;
using System.Diagnostics;

public class ExecuteCommandsInGame : ICommand
{
    readonly Queue<ICommand> _q;

    public ExecuteCommandsInGame(Queue<ICommand> q) => _q = q; 
    
    public void Execute()
    {
        var stopWatch = new Stopwatch();
        var quantum = IoC.Resolve<int>("Game.Quantum");
        
        stopWatch.Start();
        while (stopWatch.ElapsedMilliseconds <= quantum)
        {
            var cmd = IoC.Resolve<ICommand>("Game.Queue.Dequeue", _q);
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<ICommand>("Exception.Handler", e).Execute();
            }
        }
        stopWatch.Stop();
    }    
}