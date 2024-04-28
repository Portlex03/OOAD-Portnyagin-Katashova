namespace SpaceBattle.Lib.Test;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class InterpretateCommandTests
{
    private readonly string gameId = "asdfg";
    private readonly Mock<IMessage> _message = new();
    private readonly Mock<IStrategy> _sendCommandInGame = new();
    private readonly Mock<ICommand> _sendCmd = new();
    private readonly Mock<IStrategy> _getCommand = new(); 
    private readonly Mock<ICommand> _getCmd = new();
    public InterpretateCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set", 
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetCommand",
            (object[] args) => _getCommand.Object.Execute(args)
        ).Execute();
    }

// _sendCmd лег

/*
        Сценарий 1 - GetCommand - null:
    Нужно Mock<IStrategy> для GetCommand, что null
    Необходимо Mock положить в IoC
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 2 - GetCommand вызывает Exception:
    Нужно Mock<IStrategy> для GetCommand, который вызывает Exception
    Необходимо Mock положить в IoC
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 3 - GetCommand - не null, SendCommandInGame - null:
    Нужно Mock<Istrategy> для GetCommand , что не null, и Mock<ICommand> для GetCommand, и Mock<IStrategy> для SendCommandInGame - null, 
    Необходимо, чтобы Mock<Istrategy> GetCommand вернул Mock<ICommand>
    положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 4 - GetCommand - не null, SendCommandInGame вызывает Exception:
    Нужно Mock<Istrategy> для GetCommand , что не null, и Mock<ICommand> для GetCommand, и Mock<IStrategy> для SendCommandInGame вызывает Exception, 
    Необходимо, чтобы Mock<Istrategy> GetCommand вернул Mock<ICommand>
    положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 5 - GetCommand - не null, SendCommandInGame - не null:
    Нужно Mock<Istrategy> для GetCommand , что не null, и Mock<ICommand> для GetCommand,  
        и Mock<Istrategy> для SendCommandInGame , что не null, и Mock<ICommand> для SendCommandInGame
    Необходимо, чтобы Mock<Istrategy> GetCommand вернул Mock<ICommand>, Mock<Istrategy> для SendCommandInGame должен вернуть Mock<ICommand>
    положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем, что GetCommand вызовется 1 раз и SendCommandInGame 1 раз и у ICommand 2 раза

        Сценарий 6 - GetCommand возвращает не ICommand:
    Нужно Mock<IStrategy> для GetCommand возвращает не ICommand
    Необходимо Mock положить в IoC
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 7 - SendCommand возвращает не ICommand
    Нужно Mock<Istrategy> для GetCommand , что не null, и Mock<ICommand> для GetCommand, 
        и Mock<IStrategy> для SendCommandInGame возвращает не ICommand, 
    Необходимо, чтобы Mock<Istrategy> GetCommand вернул Mock<ICommand>
    положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 8 - sendCmd не выполнился
    Нужно Mock<Istrategy> для GetCommand , что не null, и Mock<ICommand> для GetCommand,  
        и Mock<Istrategy> для SendCommandInGame , что не null, и Mock<ICommand> для SendCommandInGame
    Необходимо, чтобы Mock<Istrategy> GetCommand вернул Mock<ICommand>, Mock<Istrategy> для SendCommandInGame должен вернуть Mock<ICommand>
    положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем, что GetCommand вызовется 1 раз и SendCommandInGame 1 раз и у ICommand 2 раза

*/

    // Scenario 1
    [Fact]
    public void GetCommandReturnsNull()
    {
        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(null).Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        Assert.Throws<NullCommandException>(act);

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));
    }

    // Scenario 2
    [Fact]
    public void CetCommandReturnsException()
    {
        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        Assert.Throws<InvalidOperationException>(act);

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));
    }

    // Scenario 3
    [Fact]
    public void SendCommandInGameReturnsNull()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameId).Verifiable();
        
        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_getCmd.Object).Verifiable();

        _getCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(null).Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        Assert.Throws<NullCommandException>(act);

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameId && factArg[1] == _getCmd.Object)), Times.Exactly(1));

        _getCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // Scenario 4
    [Fact]
    public void SendCommandInGameReturnsException()
    {
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

    // Scenario 5
    [Fact]
    public void SuccessfulInterpretateCommand()
    {
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

    //Scenario 6
    [Fact]
    public void GetCommandReturnsNonTypeICommand()
    {
        _getCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidCastException>().Verifiable();


        InterpretateCommand interCmd = new InterpretateCommand(_message.Object);

        var act = () => interCmd.Execute();


        Assert.Throws<InvalidCastException>(act);

        object[] expectArgs = new object[] { _message.Object };
        _getCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));
    }

    //Scenario 7
    [Fact]
    public void SendCommandInGameReturnsNonTypeICommand()
    {
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

    [Fact]
    public void NotCreateObjectOfInterpretateCommand()
    {
        Assert.Throws<NullMessageException>(() => new InterpretateCommand(null));
    }
}
