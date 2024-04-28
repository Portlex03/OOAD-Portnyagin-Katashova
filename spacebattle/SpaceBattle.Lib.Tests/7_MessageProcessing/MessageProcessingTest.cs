namespace SpaceBattle.Lib.Test;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;


public class MessageProcessingTest
{
    private readonly string gameIdCorrect = "asdfg";
    private readonly string gameIdIncorrect = "01";
    private readonly Mock<IMessage> _message = new();
    private readonly Mock<IStrategy> _getMessage = new();
    private readonly Mock<IStrategy> _sendCommandInGame = new();
    private readonly Mock<ICommand> _sendCmd = new();
    private readonly Mock<IStrategy> _getInterpretateMessageCommand = new();
    private readonly Mock<ICommand> _interpretCmd = new();

    public MessageProcessingTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set", 
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage", 
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetInterpretateMessageCommand",
            (object[] args) => _getInterpretateMessageCommand.Object.Execute(args)
        ).Execute();
    }

/*      
        Сценарий 1 - GetMessage возвращает null:
    Нужно: Mock<IStrategy> для GetMessage, что null
    Необходимо: Mock положить в IoC
    Вызываем: команду и выполняем её
    Ожидаем: исключение

        Сценарий 2 - GetMessage выбрасывает Exception:
    Нужно: Mock<IStrategy> GetMessage, который вызывает Exception
    Необходимо: Mock положить в IoC
    Вызываем: команду и выполняем её
    Ожидаем: исключение

        Сценарий 3 - GetMessage возвращает IMessage с несуществующим id:
    Нужно: Mock<Istrategy> для GetMessage , что не null, 
        и Mock<IMessage> с несуществующим game_id, и Mock<IStrategy> GetInterpretateMessageCommand - not null,
        и Mock<IStrategy> SendCommandInGame, который вызывает Exception
    Необходимо:, чтобы Mock<Istrategy> вернул Mock<IMessage>, и положить в IoC Mock<Istrategy>
    Вызываем: команду и выполняем её
    Ожидаем: исключение

        Сценарий 4 - GetMessage возвращает IMessage с id, но SendCommandInGame - null:
    Нужно: Mock<Istrategy> для GetMessage , что не null, и Mock<IMessage> с правильным game_id, 
        и Mock<IStrategy> GetInterpretateMessageCommand - not null, 
        и Mock<IStrategy> SendCommandInGame возвращает null
    Необходимо:, чтобы Mock<Istrategy> вернул Mock<IMessage>, и положить в IoC Mock<Istrategy>
    ...
    private readonly Mock<IStrategy> _GetMessage = new();
    IoC.Resolve<ICommand>("Ioc.Register", "GetMessage", (object[] args) => _GetMessage.Object.Execute(args)).Execute();
    ...
    Вызываем: команду и выполняем её
    Ожидаем: исключение

        Сценарий 5 - Зеленый:
    Нужно: Mock<Istrategy> для GetMessage , что не null, и Mock<IMessage> с правильным game_id,
        и Mock<IStrategy> GetInterpretateMessageCommand - not null, 
        и Mock<Istrategy> для SendCommandInGame , что не null, и Mock<ICommand>
    Необходимо:, чтобы Mock<Istrategy> GetMessage вернул Mock<IMessage>, 
        и Mock<Istrategy> для SendCommandInGame должен вернуть Mock<ICommand> и положить в IoC Mock<Istrategy>
    Вызываем: команду и выполняем её
    Ожидаем:, что GetMessage вызовется 1 раз и SendCommandInGame 1 раз и у ICommand 1 раз, sendCmd выполнится
    
        Сценарий 6 - GetMessage возвращает IMessage с id, но  SendCommandInGame возвращает Exception:
    Нужно Mock<Istrategy> для GetMessage , что не null, и Mock<IMessage> с правильным game_id,
        и Mock<IStrategy> GetInterpretateMessageCommand - not null, 
        и Mock<IStrategy> для SendCommandInGame, что Exception
    Необходимо, чтобы Mock<Istrategy> вернул Mock<IMessage>, и положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 7 - GetMessage возвращает IMessage с id, GetInterpretateMessageCommand - null:
    Нужно Mock<IStrategy> для GetMessage, что не null, и Mock<IMessage> с правильным game_id,
        и Mock<IStrategy> GetInterpretateMessageCommand - null
    Необходимо, чтобы Mock<IStrategy> вернул Mock<IMessage>, и положить в IoC Mock<IStrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение
        
        Сценарий 8 - GetMessage возвращает IMessage с id, GetInterpretateMessageCommand выбросывает Exception:
    Нужно Mock<IStrategy> для GetMessage, что не null, и Mock<IMessage> с правильным game_id,
        и Mock<IStrategy> GetInterpretateMessageCommand выбрасывает Exception
    Необходимо, чтобы Mock<IStrategy> вернул Mock<IMessage>, и положить в IoC Mock<IStrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 9 - Возвращение не IMessage от Mock<ISrtategy> для GetMessage:
    Нужно Mock<IStrategy> для GetMessage возвращает не IMessage
    Необходимо Mock положить в IoC
    Вызываем команду и выполняем её
    Ожидаем исключение InvalidCastException

        Сценарий 10 - Возвращение не ICommand от Mock<ISrtategy> для GetInterpretateMessageCommand:
    Нужно Mock<IStrategy> для GetMessage, что не null, и Mock<IMessage> с правильным game_id,
        и Mock<IStrategy> GetInterpretateMessageCommand возвращает не ICommand
    Необходимо, чтобы Mock<IStrategy> вернул Mock<IMessage>, и положить в IoC Mock<IStrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение InvalidCastException

        Сценарий 11 - Возвращение не ICommand от Mock<IStrategy> для SendCommandInGame:
    Нужно Mock<Istrategy> для GetMessage , что не null, и Mock<IMessage> с правильным game_id,
        и Mock<IStrategy> GetInterpretateMessageCommand - not null, 
        и Mock<IStrategy> для SendCommandInGame возвращает не ICommand
    Необходимо, чтобы Mock<Istrategy> вернул Mock<IMessage>, и положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение InvalidCastException

        Сценарий 12 - sendCmd не сработал:
    Нужно: Mock<Istrategy> для GetMessage , что не null, и Mock<IMessage> с правильным game_id,
        и Mock<IStrategy> GetInterpretateMessageCommand - not null, 
        и Mock<Istrategy> для SendCommandInGame , что не null, и Mock<ICommand>, sendCmd вернул Exception
    Необходимо:, чтобы Mock<Istrategy> GetMessage вернул Mock<IMessage>, 
        и Mock<Istrategy> для SendCommandInGame должен вернуть Mock<ICommand> и положить в IoC Mock<Istrategy>
    Вызываем: команду и выполняем её
    Ожидаем:, что GetMessage вызовется 1 раз и SendCommandInGame 1 раз и у ICommand 1 раз, sendCmd вернет Exception
    
*/
    // Scenario 1
    [Fact]
    public void GetMessageReturnsNull()
    {
        _getMessage.Setup(strategy => strategy.Execute()).Returns(null).Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<NullCommandException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));
    }

    // Scenario 2
    [Fact]
    public void GetMessageReturnsException()
    {
        _getMessage.Setup(strategy => strategy.Execute()).Throws<InvalidOperationException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidOperationException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));
    }

    // Scenario 3
    [Fact]
    public void GetMessageReturnsGameIdIncorrest()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdIncorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidOperationException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdIncorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // Scenario 4
    [Fact]
    public void SendCommandInGameReturnsNull()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(null).Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<NullCommandException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdCorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // Scenario 5
    [Fact]
    public void SuccessfulMessageProcessing()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _sendCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_sendCmd.Object).Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        try
        {
            act();
        }
        catch (Exception ex)
        {
            Assert.Fail("Test should be performed without exceptions");
        }

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdCorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // Scenario 6
    [Fact]
    public void SendCommandInGameReturnsException()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidOperationException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdCorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // Scenario 7
    [Fact]
    public void GetInterpretateMessageCommandReturnsNull()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(null).Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<NullCommandException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Never());

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // Scenario 8
    [Fact]
    public void GetInterpretateMessageCommandReturnsException()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<NullCommandException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<NullCommandException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Never()); // Never

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // Scenario 9
    [Fact]
    public void GetMessageReturnsNonTypeIMessage()
    {
        _getMessage.Setup(strategy => strategy.Execute()).Throws<InvalidCastException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidCastException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));
    }

    // Scenario 10
    [Fact]
    public void GetInterpretateMessageCommandReturnsNonTypeICommand()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidCastException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidCastException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Never());

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // Scenario 11
    [Fact]
    public void SendCommandInGameReturnsNonTypeICommand()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidCastException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidCastException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdCorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    // Scenario 12
    [Fact]
    public void SendCmdReturnsException()
    {
        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _sendCmd.Setup(cmd => cmd.Execute()).Throws<InvalidOperationException>().Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_sendCmd.Object).Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidOperationException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdCorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }
}


// cmd лег

