namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class ExceptionHandlerTest
{
    readonly object _scope;
    readonly Mock<IStrategy> _getQuantCmd;
    readonly Dictionary<object, object> _exHandlerDict;
    readonly Mock<ICommand> _suitableStrategy;

    public ExceptionHandlerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        _getQuantCmd = new();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Quant",
            (object[] args) => _getQuantCmd.Object.Execute()
        ).Execute();

        _exHandlerDict = new();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.Tree",
            (object[] args) => _exHandlerDict
        ).Execute();

        _suitableStrategy = new(); // "подходящая стратегия"
        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.SuitableStrategy",
            (object[] args) => _suitableStrategy.Object
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.Handler",
            (object[] args) => new ExceptionHandlerCmd(
                (Exception)args[0], (ICommand)args[1])
        ).Execute();
    }

    [Fact]
    public void Default_Handler_Test()
    {
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
        _getQuantCmd.Setup(cmd => cmd.Execute()).Returns(quant);

        Assert.Throws<Exception>(gameAsCommand.Execute);
        exCmd.Verify(cmd => cmd.Execute(), Times.Once());
    }

    [Fact]
    public void Exception_Handler_Test()
    {
        var cmd = new Mock<ICommand>();

        var exCmd = new Mock<ICommand>();
        exCmd.Setup(cmd => cmd.Execute()).Throws<Exception>();

        _exHandlerDict.Add(exCmd.Object, new Exception());

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(exCmd.Object);
        q.Enqueue(cmd.Object);

        var gameAsCommand = new GameAsCommand(
            IoC.Resolve<object>("Scopes.New", _scope), q);

        var quant = 50;
        _getQuantCmd.Setup(cmd => cmd.Execute()).Returns(quant);

        gameAsCommand.Execute();

        exCmd.Verify(cmd => cmd.Execute(), Times.Once());
        _suitableStrategy.Verify(cmd => cmd.Execute(), Times.Once());
    }
}
