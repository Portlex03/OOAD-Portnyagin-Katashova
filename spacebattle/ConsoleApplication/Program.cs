namespace SpaceBattle.Lib;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class ConsoleApplication
{
    static ConsoleApplication()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        // стратегия установки нового скопа
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        // стратегия создания и запуска сервера
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Start",
            (object[] args) => new StartServerCommand((int)args[0])
        ).Execute();

        // словарь с потоками
        var threadDict = new Dictionary<int, object>();

        // стратегия создания и запуска потока сервера
        var createAndStartServerCommand = new ActionCommand(() => { });
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.Create&Start",
            (object[] args) =>
            {
                // получаем id потока
                var threadId = (int)args[0];

                // регистрируем его в словарь
                threadDict.TryAdd(threadId, $"ServerThread_№{threadId}");

                // возвращаем команду, которая создала и зарегистрировала сервер
                return createAndStartServerCommand;
            }
        ).Execute();

        // стратегия отправки команды по id потока
        var sendToThreadCommand = new ActionCommand(() => { });
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.SendCommand",
            (object[] args) => sendToThreadCommand
        ).Execute();

        // стратегия остановки потока по id
        var softStopCommand = new ActionCommand(() => { });
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.SoftStop",
            (object[] args) => softStopCommand
        ).Execute();

        // стратегия остановки всего сервера
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Stop",
            (object[] args) => new StopServerCommand()
        ).Execute();

        // стратегия получения словаря с потоками
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.GetThreadDict",
            (object[] args) => threadDict
        ).Execute();
    }

    public static void Main(string[] args)
    {
        // количество потоков сервера
        Console.WriteLine("Введите кол-во потоков: ");

        // если метод запустился не как консольное приложение,
        // то кол-во потоков считается с args, которое определяется в тестах
        if (!int.TryParse(Console.ReadLine(), out int threadCount))
            threadCount = int.Parse(args[0]);

        // сообщение о начале процедуры запуска сервера
        Console.WriteLine("Начало запуска сервера...");

        // стратегия запуска сервера с кол-вом потоков threadCount
        IoC.Resolve<ICommand>("Server.Start", threadCount).Execute();

        // сообщение об успешном запуске сервера (после старта всех потоков)
        Console.WriteLine($"Успешно запущен сервер с кол-вом потоков: {threadCount}");

        Console.WriteLine("Для остановки сервера введите любую клавишу... ");

        Console.Read();

        // сообщение начала процедуры остановки сервера
        Console.WriteLine("Начало остановки сервера...");

        // стратегия остановки сервера
        IoC.Resolve<ICommand>("Server.Stop").Execute();

        // сообщение завершения процедуры остановки сервера
        Console.WriteLine("Cервер успешно завершил свою работу. Нажмите любую кнопку для выхода... ");

        Console.Read();
    }
}
