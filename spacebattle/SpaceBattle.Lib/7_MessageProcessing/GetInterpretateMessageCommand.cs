namespace SpaceBattle.Lib;
using Hwdtech;

public class GetInterpretateMessageCommand : IStrategy
{
    public object Execute(params object[] args)
    {
        return new InterpretateCommand((IMessage) args[0]); 
    }
}

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