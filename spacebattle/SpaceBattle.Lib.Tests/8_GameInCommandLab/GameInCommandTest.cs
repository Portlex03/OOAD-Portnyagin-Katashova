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
    }

    [Fact]
    public void All_Commands_Execute_In_Game_Queue()
    {
        var getQuantCmd = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Quant",
            (object[] args) => getQuantCmd.Object.Execute(args)
        ).Execute();

        var cmd = new Mock<ICommand>();

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);

        var gameAsCommand = new GameAsCommand(
            IoC.Resolve<object>("Scopes.New", _scope), q);

        var quant = 50;
        getQuantCmd.Setup(cmd => cmd.Execute()).Returns(quant);

        gameAsCommand.Execute();

        Assert.True(q.Count == 0);
    }

    [Fact]
    public void Commands_Executes_While_Time_Compliting_Less_Then_Quant()
    {
        var getQuantCmd = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Quant",
            (object[] args) => getQuantCmd.Object.Execute(args)
        ).Execute();

        var cmd = new Mock<ICommand>();

        var longTimeCmd = new Mock<ICommand>();
        longTimeCmd.Setup(cmd => cmd.Execute()).Callback(
            () => { getQuantCmd.Setup(cmd => cmd.Execute()).Returns(0); }
        ).Verifiable();

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(longTimeCmd.Object);
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);

        var gameAsCommand = new GameAsCommand(
            IoC.Resolve<object>("Scopes.New", _scope), q);

        var quant = 50;
        getQuantCmd.Setup(cmd => cmd.Execute()).Returns(quant);

        gameAsCommand.Execute();

        Assert.True(q.Count == 2);
    }

    [Fact]
    public void Impossible_To_Get_Quant()
    {
        var getQuantCmd = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Quant",
            (object[] args) => getQuantCmd.Object.Execute(args)
        ).Execute();

        var cmd = new Mock<ICommand>();

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(cmd.Object);

        var gameAsCommand = new GameAsCommand(
            IoC.Resolve<object>("Scopes.New", _scope), q);

        getQuantCmd.Setup(cmd => cmd.Execute()).Throws<Exception>();

        Assert.Throws<Exception>(gameAsCommand.Execute);
    }
}
