namespace SpaceBattle.Lib;
using Hwdtech;

public class InitGameScopeStrategy : IStrategy
{
    public object Invoke(params object [] args)
    {
        object oldScope = IoC.Resolve<object>("Scopes.Current");
        object newScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));

        String gameId = IoC.Resolve<String>("Games.Id.GetNew");
        object quantumTime = args[0];
        Queue<ICommand> queue = new();
        Dictionary<string, IUObject> objects = new();

        IoC.Resolve<ICommand>("Scopes.Current.Set", newScope).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetGameId", (object[] anyArgs) => gameId).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetQuantumTime", (object[] anyArgs) => quantumTime).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetQueue", (object[] anyArgs) => queue).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetObjects", (object[] anyArgs) => objects).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetObject", (object[] anyArgs) => objects[(string) anyArgs[0]]).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.RemoveObject", (object[] anyArgs) => new RemoveCommandInObject(objects, (string) anyArgs[0])).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.QueueDequeue", (object[] anyArgs) => queue.Dequeue()).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.QueueEnqueue", (object[] anyArgs) => new QueueEnqueueCommand(queue, (ICommand)anyArgs[0])).Execute();
        
        IoC.Resolve<ICommand>("Scopes.Current.Set", oldScope).Execute();

        return newScope;
    }
}
