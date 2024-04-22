namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class ExceptionHandlerTest
{
    readonly object _scope;
    readonly Mock<IStrategy> _getQuantCmd = new();
    readonly Dictionary<object, object> _exHandlerDict = new();
    readonly Mock<ICommand> _exHandler = new();
    readonly Mock<ICommand> _defaultExHandler = new();

    public ExceptionHandlerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Quant",
            (object[] args) => _getQuantCmd.Object.Execute()
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.Handler",
            (object[] args) =>
            {
                var ex = (Exception)args[0];
                var cmd = (ICommand)args[1];

                if (_exHandlerDict.ContainsKey(cmd))
                    return _exHandler.Object;

                ex.Data["Command"] = cmd;
                _defaultExHandler.Setup(cmd => cmd.Execute()).Throws(ex).Verifiable();
                return _defaultExHandler.Object;
            }
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
        _defaultExHandler.Verify(cmd => cmd.Execute(), Times.Once);
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
        _exHandler.Verify(cmd => cmd.Execute(), Times.Once());
    }
}
