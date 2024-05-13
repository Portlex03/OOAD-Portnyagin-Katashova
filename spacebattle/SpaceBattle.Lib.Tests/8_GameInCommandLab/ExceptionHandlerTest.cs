namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class ExceptionHandlerTest
{
    readonly object _scope;

    public ExceptionHandlerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.Handler",
            (object[] args) => new ExceptionHandlerCmd(
                (Exception)args[0], (ICommand)args[1])
        ).Execute();
    }

    [Fact]
    public void Default_Handler_Test()
    {
        Mock<IStrategy> getQuantCmd = new();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Quant",
            (object[] args) => getQuantCmd.Object.Invoke()
        ).Execute();

        Dictionary<object, object> exHandlerDict = new();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.Tree",
            (object[] args) => exHandlerDict
        ).Execute();

        Mock<ICommand> suitableStrategy = new();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.SuitableStrategy",
            (object[] args) => suitableStrategy.Object
        ).Execute();

        var cmd = new Mock<ICommand>();

        var exCmd = new Mock<ICommand>();
        exCmd.Setup(cmd => cmd.Execute()).Throws<Exception>();

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(exCmd.Object);
        q.Enqueue(cmd.Object);

        var gameAsCommand = new GameAsCommand(
            IoC.Resolve<object>("Scopes.New", _scope), q);

        var quant = 50;
        getQuantCmd.Setup(cmd => cmd.Invoke()).Returns(quant);

        Assert.Throws<Exception>(gameAsCommand.Execute);
        exCmd.Verify(cmd => cmd.Execute(), Times.Once());
    }

    [Fact]
    public void Exception_Handler_Test()
    {
        Mock<IStrategy> getQuantCmd = new();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Quant",
            (object[] args) => getQuantCmd.Object.Invoke()
        ).Execute();

        Dictionary<object, object> exHandlerDict = new();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.Tree",
            (object[] args) => exHandlerDict
        ).Execute();

        Mock<ICommand> suitableStrategy = new();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.SuitableStrategy",
            (object[] args) => suitableStrategy.Object
        ).Execute();

        var cmd = new Mock<ICommand>();

        var exCmd = new Mock<ICommand>();
        exCmd.Setup(cmd => cmd.Execute()).Throws<Exception>();

        exHandlerDict.Add(exCmd.Object, new Exception());

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(exCmd.Object);
        q.Enqueue(cmd.Object);

        var gameAsCommand = new GameAsCommand(
            IoC.Resolve<object>("Scopes.New", _scope), q);

        var quant = 50;
        getQuantCmd.Setup(cmd => cmd.Invoke()).Returns(quant);

        gameAsCommand.Execute();

        exCmd.Verify(cmd => cmd.Execute(), Times.Once());
        suitableStrategy.Verify(cmd => cmd.Execute(), Times.Once());
    }
}
