namespace SpaceBattle.Lib;
using Hwdtech;

public class MessageProcessing : ICommand
{
    public void Execute()
    {
        IMessage message = IoC.Resolve<IMessage>("GetMessage") ?? throw new NullCommandException(); // 5 mock
        // 1 mock - Mock<IMessage> с правильным game_id
        // 2 mock - Mock<IMessage> с несуществующим game_id
        // 3 mock - Mock<IStrategy> для GetMessage, что null
        // 4 mock - Mock<Istrategy> для GetMessage , что не null
        // 5 mock - Mock<IStrategy>, который вызывает Exception 
        // => 11-13 один
        ICommand interpretcmd = IoC.Resolve<ICommand>("GetInterpretateMessageCommand", message) ?? throw new NullCommandException(); // 4 mock
        // 1 mock - Mock<Istrategy> для GetInterpretateMessageCommand - null
        // 2 mock - Mock<IStrategy> для GetInterpretateMessageCommand, что не null
        // 3 mock - Mock<IStrategy> для GetInterpretateMessageCommand вызывает Exception
        // 4 mock - Mock<ICommand>
        ICommand cmd = IoC.Resolve<ICommand>("SendCommandInGame", message.gameId, interpretcmd) ?? throw new NullCommandException(); // 4 mock
        // 1 mock - Mock<IStrategy> для SendCommandInGame, что null
        // 2 mock - Mock<Istrategy> для SendCommandInGame , что не null
        // 3 mock - Mock<ICommand>
        // 4 mock - Mock<IStrategy> SendCommandInGame, который вызывает Exception
        cmd.Execute(); 
    }
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
