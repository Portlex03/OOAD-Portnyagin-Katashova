namespace SpaceBattle.Lib.Test;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class InterpretateCommandTests
{
    // private readonly string gameId = "asdfg";
    // private readonly Mock<IMessage> _message = new();
    // private readonly Mock<IStrategy> _sendCommandInGame = new();
    // private readonly Mock<ICommand> _sendCmd = new();
    // private readonly Mock<IStrategy> _getCommand = new();
    // private readonly Mock<ICommand> _getCmd = new();

    public InterpretateCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        // IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
        //     (object[] args) => _sendCommandInGame.Object.Execute(args)
        // ).Execute();

        // IoC.Resolve<ICommand>("IoC.Register", "GetCommand",
        //     (object[] args) => _getCommand.Object.Execute(args)
        // ).Execute();
    }

    // [Fact]
    // public void GetCommandReturnsNull()
    // {
    //     _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(null!).Verifiable();


    //     InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

    //     var act = () => interCmd.Execute();


    //     Assert.Throws<NullCommandException>(act);

    //     object[] expectArgs = new object[] { _message.Object };
    //     _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));
    // }

    [Fact]
    public void CetCommandReturnsException()
    {
        Mock<IMessage> _message = new();
   
        Mock<IStrategy> _getCommand = new();
        IoC.Resolve<ICommand>("IoC.Register", "GetCommand",
            (object[] args) => _getCommand.Object.Execute(args)
        ).Execute();

        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        Assert.Throws<InvalidOperationException>(act);

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));
    }

    // [Fact]
    // public void SendCommandInGameReturnsNull()
    // {
    //     _message.SetupGet(strategy => strategy.gameId).Returns(gameId).Verifiable();

    //     _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_getCmd.Object).Verifiable();

    //     _getCmd.Setup(cmd => cmd.Execute()).Verifiable();

    //     _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(null!).Verifiable();


    //     InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

    //     var act = () => interCmd.Execute();


    //     Assert.Throws<NullCommandException>(act);

    //     object[] expectArgs = new object[] { _message.Object };
    //     _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

    //     _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
    //         factArg => (string)factArg[0] == gameId && factArg[1] == _getCmd.Object)), Times.Exactly(1));

    //     _getCmd.Verify(cmd => cmd.Execute(), Times.Never());
    // }

    [Fact]
    public void SendCommandInGameReturnsException()
    {
        Mock<IMessage> _message = new();
        string gameId = "asdfg";
        Mock<IStrategy> _getCommand = new();
        Mock<ICommand> _getCmd = new();
        Mock<IStrategy> _sendCommandInGame = new();
        

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetCommand",
            (object[] args) => _getCommand.Object.Execute(args)
        ).Execute();


        _message.SetupGet(strategy => strategy.gameId).Returns(gameId).Verifiable();

        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_getCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        Assert.Throws<InvalidOperationException>(act);

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameId && factArg[1] == _getCmd.Object)), Times.Exactly(1));

        _getCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void SuccessfulInterpretateCommand()
    {
        Mock<IMessage> _message = new();
        string gameId = "asdfg";
        Mock<IStrategy> _getCommand = new();
        Mock<ICommand> _getCmd = new();
        Mock<IStrategy> _sendCommandInGame = new();
        Mock<ICommand> _sendCmd = new();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetCommand",
            (object[] args) => _getCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameId).Verifiable();

        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_getCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_sendCmd.Object).Verifiable();

        _sendCmd.Setup(cmd => cmd.Execute()).Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        try
        {
            act();
        }
        catch (Exception ex)
        {
            Assert.Fail("Test should be performed without exceptions" + ex.Message);
        }

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameId && factArg[1] == _getCmd.Object)), Times.Exactly(1));

        _sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(1));

        _getCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void GetCommandReturnsNonTypeICommand()
    {
        Mock<IMessage> _message = new();
        Mock<IStrategy> _getCommand = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetCommand",
            (object[] args) => _getCommand.Object.Execute(args)
        ).Execute();

        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidCastException>().Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        Assert.Throws<InvalidCastException>(act);

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));
    }

    [Fact]
    public void SendCommandInGameReturnsNonTypeICommand()
    {
        Mock<IMessage> _message = new();
        string gameId = "asdfg";
        Mock<IStrategy> _getCommand = new();
        Mock<ICommand> _getCmd = new();
        Mock<IStrategy> _sendCommandInGame = new();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetCommand",
            (object[] args) => _getCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameId).Verifiable();

        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_getCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidCastException>().Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        Assert.Throws<InvalidCastException>(act);

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameId && factArg[1] == _getCmd.Object)), Times.Exactly(1));

        _getCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void SendCmdReturnsException()
    {
        Mock<IMessage> _message = new();
        string gameId = "asdfg";
        Mock<IStrategy> _getCommand = new();
        Mock<ICommand> _getCmd = new();
        Mock<IStrategy> _sendCommandInGame = new();
        Mock<ICommand> _sendCmd = new();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetCommand",
            (object[] args) => _getCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameId).Verifiable();

        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_getCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_sendCmd.Object).Verifiable();

        _sendCmd.Setup(cmd => cmd.Execute()).Throws<InvalidCastException>().Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        Assert.Throws<InvalidCastException>(act);

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameId && factArg[1] == _getCmd.Object)), Times.Exactly(1));

        _sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(1));

        _getCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // [Fact]
    // public void NotCreateObjectOfInterpretateCommand()
    // {
    //     Assert.Throws<NullMessageException>(() => new InterpretateCommand(null!));
    // }
}
