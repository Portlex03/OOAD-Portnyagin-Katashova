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
        ICommand interpretcmd = IoC.Resolve<ICommand>("GetInterpretateMessageCommand", message) ?? throw new NullCommandException();
        ICommand cmd = IoC.Resolve<ICommand>("SendCommandInGame", message.gameId, interpretcmd) ?? throw new NullCommandException(); // 4 mock
        // 1 mock - Mock<IStrategy> для SendCommandInGame, что null
        // 2 mock - Mock<Istrategy> для SendCommandInGame , что не null
        // 3 mock - Mock<ICommand>
        // 4 mock - Mock<IStrategy> SendCommandInGame, который вызывает Exception
        cmd.Execute(); 
    }
}




// Return для случаев, когда Mock<Istrategy> вернул Mock<IMessage>
//

/*      Сценарий 1 - GetMessage возвращает null:
    Нужно Mock<IStrategy> для GetMessage, что null
    Необходимо Mock положить в IoC
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 2 - GetMessage возвращает IMessage с несуществующим id:
    Нужно Mock<Istrategy> для GetMessage , что не null, 
        и Mock<IMessage> с несуществующим game_id, и Mock<IStrategy> SendCommandInGame, который вызывает Exception
    Необходимо, чтобы Mock<Istrategy> вернул Mock<IMessage>, и положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 3 - GetMessage выбрасывает Exception:
    Нужно Mock<IStrategy> GetMessage, который вызывает Exception
    Необходимо Mock положить в IoC
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 4 - GetMessage возвращает IMessage с id, но происходит ошибка в SendCommandInGame:
    Нужно Mock<Istrategy> для GetMessage , что не null, и Mock<IMessage> с правильным game_id, 
        и Mock<IStrategy> для SendCommandInGame, что null, и Mock<IStrategy> SendCommandInGame, который вызывает Exception
    Необходимо, чтобы Mock<Istrategy> вернул Mock<IMessage>, и положить в IoC Mock<Istrategy>
    ...
    private readonly Mock<IStrategy> _GetMessage = new();
    IoC.Resolve<ICommand>("Ioc.Register", "GetMessage", (object[] args) => _GetMessage.Object.Execute(args)).Execute();
    ...
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 5 - Зеленый:
    Нужно Mock<Istrategy> для GetMessage , что не null, и Mock<IMessage> с правильным game_id, 
        и Mock<Istrategy> для SendCommandInGame , что не null, и Mock<ICommand>
    Необходимо, чтобы Mock<Istrategy> GetMessage вернул Mock<IMessage>, Mock<Istrategy> для SendCommandInGame должен вернуть Mock<ICommand>
    положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем, что GetMessage вызовется 1 раз и SendCommandInGame 1 раз и у ICommand 1 раз
    
        Сценарий 6 - GetMessage возвращает IMessage с id, но  SendCommandInGame возвращает null:
    Нужно Mock<Istrategy> для GetMessage , что не null, и Mock<IMessage> с правильным game_id, 
        и Mock<IStrategy> для SendCommandInGame, что null, и Mock<IStrategy> SendCommandInGame, который возвращает null
    Необходимо, чтобы Mock<Istrategy> вернул Mock<IMessage>, и положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 7 и 8 -  _getInterpretateMessageCommand либо null, либо выброс Exception:

        Сценарий 9-11 - Возвращение не того типа для IoC.Resolve
    Ожидаем InvalidCastException
*/

// public class InterpretateCommand : ICommand
// {
//     private readonly IMessage _message;
//     public InterpretateCommand(IMessage message)
//     {
//         _message = message ?? throw new Exception();
//     }
//     public void Execute()
//     {
//         ICommand cmd = IoC.Resolve<ICommand>("GetCommand", _message) ?? throw new Exception(); // 4 mock
//         // 1 mock - Mock<Istrategy> для GetCommand, если null
//         // 2 mock - Mock<Istrategy> для GetCommand, если не null
//         // 3 mock - Mock<IStrategy> для GetCommand, что вызывает Exception
//         // 4 mock - Mock<ICommand>
//         ICommand sendCmd = IoC.Resolve<ICommand>("SendCommandInGame", _message.gameId, cmd) ?? throw new Exception(); // 4 mock
//         // 1 mock - Mock<IStrategy> для SendCommandInGame, что null
//         // 2 mock - Mock<Istrategy> для SendCommandInGame, что не null
//         // 3 mock - Mock<ICommand>
//         // 4 mock - Mock<IStrategy> для SendCommandInGame, что вызывает Exception
//         sendCmd.Execute();
//     }
// }

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
    Нужно Mock<Istrategy> для GetMessage , что не null, и Mock<ICommand> для GetMessage, и Mock<IStrategy> для SendCommandInGame - null, 
    Необходимо, чтобы Mock<Istrategy> GetMessage вернул Mock<ICommand>
    положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 4 - GetCommand - не null, SendCommandInGame вызывает Exception:
    Нужно Mock<Istrategy> для GetMessage , что не null, и Mock<ICommand> для GetMessage, и Mock<IStrategy> для SendCommandInGame вызывает Exception, 
    Необходимо, чтобы Mock<Istrategy> GetMessage вернул Mock<ICommand>
    положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем исключение

        Сценарий 5 - GetCommand - не null, SendCommandInGame - не null:
    Нужно Mock<Istrategy> для GetMessage , что не null, и Mock<ICommand> для GetMessage,  
        и Mock<Istrategy> для SendCommandInGame , что не null, и Mock<ICommand> для SendCommandInGame
    Необходимо, чтобы Mock<Istrategy> GetMessage вернул Mock<ICommand>, Mock<Istrategy> для SendCommandInGame должен вернуть Mock<ICommand>
    положить в IoC Mock<Istrategy>
    Вызываем команду и выполняем её
    Ожидаем, что GetMessage вызовется 1 раз и SendCommandInGame 1 раз и у ICommand 2 раза

*/


// public class GetInterpretateMessageCommand : IStrategy
// {
//     public object Execute(params object[] args)
//     {
//         return new InterpretateCommand((IMessage) args[0]); 
//     }
// }

// args пустой 
// args[0] не является IMessage
// зеленый

/*
        Сценарий 1 - переданный args - пустой:
    Нужно args - null
    Ожидаем исключение

        Сценарий 2 - args[0] не является IMessage:
    Нужно args не IMessage
    Ожидаем исключение

        Сценарий 3 - Зеленый:

*/
