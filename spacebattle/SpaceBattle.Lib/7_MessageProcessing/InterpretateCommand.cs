namespace SpaceBattle.Lib;

using System.Data;
using Hwdtech;

public class InterpretateCommand : ICommand
{
    private readonly IMessage _message;
    public InterpretateCommand(IMessage message)
    {
        _message = message ?? throw new NullMessageException();
    }
    public void Execute()
    {
        ICommand getCmd = IoC.Resolve<ICommand>("GetCommand", _message) ?? throw new NullCommandException(); // 4 mock
        // 1 mock - Mock<Istrategy> для GetCommand, если null
        // 2 mock - Mock<Istrategy> для GetCommand, если не null
        // 3 mock - Mock<IStrategy> для GetCommand, что вызывает Exception
        // 4 mock - Mock<ICommand>
        ICommand sendCmd = IoC.Resolve<ICommand>("SendCommandInGame", _message.gameId, getCmd) ?? throw new NullCommandException(); // 4 mock
        // 1 mock - Mock<IStrategy> для SendCommandInGame, что null
        // 2 mock - Mock<Istrategy> для SendCommandInGame, что не null
        // 3 mock - Mock<ICommand>
        // 4 mock - Mock<IStrategy> для SendCommandInGame, что вызывает Exception
        sendCmd.Execute();
    }
}

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

*/