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
    // private readonly Mock<IStrategy> _getCommand = new(); 
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

        // IoC.Resolve<ICommand>("IoC.Register", "GetCommand",
        //     (object[] args) => _getCommand.Object.Execute(args)
        // ).Execute();

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
    Ожидаем:, что GetMessage вызовется 1 раз и SendCommandInGame 1 раз и у ICommand 1 раз
    
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
    
*/
    // Scenario 1
    [Fact]
    public void GetMessageReturnsNull()
    {
        _getMessage.Setup(getMsg => getMsg.Execute()).Throws<NullCommandException>().Verifiable();


        MessageProcessing myCommand = new MessageProcessing();
        var act = () => myCommand.Execute();


        Assert.Throws<NullCommandException>(act);
    }

    // Scenario 2
    [Fact]
    public void GetMessageReturnsException()
    {
        _getMessage.Setup(getMsg => getMsg.Execute()).Throws<InvalidOperationException>().Verifiable();


        MessageProcessing myCommand = new MessageProcessing();
        var act = () => myCommand.Execute();


        Assert.Throws<InvalidOperationException>(act);
    }

    // Scenario 3
    [Fact]
    public void GetMessageReturnsGameIdIncorrest()
    {
        _message.SetupGet(msg => msg.gameId).Returns(gameIdIncorrect).Verifiable();

        _getMessage.Setup(getMsg => getMsg.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(x => x.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(sendCmd => sendCmd.Execute(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();


        MessageProcessing myCommand = new MessageProcessing();
        var act = () => myCommand.Execute();


        Assert.Throws<InvalidOperationException>(act);

        _getMessage.Verify(getMsg => getMsg.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(x => x.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(sendCmd => sendCmd.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdIncorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(x => x.gameId, Times.Exactly(1));

        _interpretCmd.Verify(x => x.Execute(), Times.Never());
    }

    // Scenario 4
    [Fact]
    public void SendCommandInGameReturnsNull()
    {

    }
}
