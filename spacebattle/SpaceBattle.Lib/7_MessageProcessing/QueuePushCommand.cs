namespace SpaceBattle.Lib;
using Hwdtech;

public class QueuePushCommandStrategy : IStrategy
{
    public object Execute(params object[] args)
    {
        ICommand cmd = (ICommand)args[1];

        var queue = IoC.Resolve<Queue<ICommand>>("SpaceBattle.GetQueue", (string)args[0]);

        return new ActionCommand(() => { queue.Enqueue(cmd); });
    }
}
