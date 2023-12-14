namespace SpaceBattle.Lib;
using Hwdtech;


public class LongOperation : ICommand
{
    private readonly string _cmdName;
    private readonly IUObject _target;

    public LongOperation(string cmdName, IUObject target)
    {
        _cmdName = cmdName;
        _target = target;
    }

    public void Execute()
    {
        var stringsList = IoC.Resolve<IEnumerable<string>>(
            "SetupStringOperation." + _cmdName
        );

        var commandsList = stringsList.Select(
            str => IoC.Resolve<ICommand>(str,_target)
        );

        var macroCommand = IoC.Resolve<ICommand>(
            "MacroCommand.Create", commandsList
        );
    }
}

// // имя зависимости длительной операции
// var cmdName = IoC.Resolve<string>(
//     "Operations.Name"
// );

// // объект, для которого выполняется эта операция
// var target = IoC.Resolve<IUObject>(
//     "Operations.Target"
// );

/*
1. Инициализация макрокоманды.
В IoC есть зависимость, которая возвращает 
массив строк, каждая из которых является 
именем зависимости какой-либо команды. 
Написать стратегию разрешения зависимости, 
которая по имени составной операции и 
объекту строит макрокоманду.

2. Длительная операция.
Макрокоманды могут быть длительными операциями. 
Как, например, Move из лабораторной работы №2. 
Реализовать стратегию IoC, которая строит 
произвольную длительную операцию, получая на 
вход лишь имя зависимости длительной операции и 
объект, для которого это операция применяется. 

Указание. Вторая задача обобщает решение 
лабораторной работы №2. Здесь необходимо 
получить стратегию, которая будет работать 
для любой длительной операции, а не только 
для Movement.

Указание. Необходимо сделать так, чтобы обе 
стратегии IoC можно было использовать совместно 
без каких-либо конфликтов имен зависимостей. 
Фактически стратегия из задачи №2 должна 
использовать команду, которая получается с 
помощью стратегии из задачи №1.
*/