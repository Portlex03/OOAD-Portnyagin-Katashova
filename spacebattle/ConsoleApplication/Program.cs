namespace SpaceBattle.Lib;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class ConsoleApplication
{
    static ConsoleApplication()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Start",
            (object[] args) => new StartServerCommand((int)args[0])
        ).Execute();

        var threadDict = new Dictionary<int, object>();

        var createAndStartServerCommand = new ActionCommand(() => { });
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.Create&Start",
            (object[] args) =>
            {
                var threadId = (int)args[0];

                threadDict.TryAdd(threadId, $"ServerThread_№{threadId}");

                return createAndStartServerCommand;
            }
        ).Execute();

        var sendToThreadCommand = new ActionCommand(() => { });
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.SendCommand",
            (object[] args) => sendToThreadCommand
        ).Execute();

        var softStopCommand = new ActionCommand(() => { });
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.SoftStop",
            (object[] args) =>
            {
                var actionCmd = new ActionCommand((Action)args[1]);
                actionCmd.Execute();
                return softStopCommand;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Stop",
            (object[] args) => new StopServerCommand()
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.GetThreadDict",
            (object[] args) => threadDict
        ).Execute();
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Введите кол-во потоков: ");

        if (!int.TryParse(Console.ReadLine(), out int threadCount))
            threadCount = int.Parse(args[0]);

        Console.WriteLine("Начало запуска сервера...");

        IoC.Resolve<ICommand>("Server.Start", threadCount).Execute();

        Console.WriteLine($"Успешно запущен сервер с кол-вом потоков: {threadCount}");

        Console.WriteLine("Для остановки сервера введите любую клавишу... ");

        Console.Read();

        Console.WriteLine("Начало остановки сервера...");

        IoC.Resolve<ICommand>("Server.Stop").Execute();

        Console.WriteLine("Cервер успешно завершил свою работу. Нажмите любую кнопку для выхода... ");

        Console.Read();
    }
}
