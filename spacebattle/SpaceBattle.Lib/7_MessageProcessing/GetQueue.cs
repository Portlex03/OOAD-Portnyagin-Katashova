namespace SpaceBattle.Lib;
using Hwdtech;

public class GetQueueStrategy : IStrategy
{
    public object Execute(params object[] args)
    {
        return IoC.Resolve<IDictionary<string, Queue<ICommand>>>("SpaceBattle.Queue").TryGetValue(
            (string)args[0], out Queue<ICommand>? queue) ? queue : throw new Exception();
    }
}
