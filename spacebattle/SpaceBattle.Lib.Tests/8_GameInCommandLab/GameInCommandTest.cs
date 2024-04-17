namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class GameInCommandTest
{
    readonly object _scope;
    public GameInCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.ExecuteCommands",
            (object[] args) => new ExecuteCommandsInGame((Queue<ICommand>)args[0])
        ).Execute();

        var getQuantumCmd = new Mock<IStrategy>();
        getQuantumCmd.Setup(cmd => cmd.Execute()).Returns(5);

        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Quantum",
            (object[] args) => getQuantumCmd.Object.Execute()
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Queue.Dequeue",
            (object[] args) =>
            {
                var q = (Queue<ICommand>)args[0];
                if(q.TryDequeue(out ICommand? cmd))
                    return cmd;
                throw new Exception();
            }
        ).Execute();
    }

    [Fact]
    public void Test1()
    {
        var q = new Queue<ICommand>();

        var cmd = new Mock<ICommand>();

        Enumerable.Range(0, 3).ToList().ForEach(iter => q.Enqueue(cmd.Object));

        var gameInCommand = new GameInCommand(_scope, q);

        gameInCommand.Execute();

        Assert.True(q.Count == 2);
    }
}