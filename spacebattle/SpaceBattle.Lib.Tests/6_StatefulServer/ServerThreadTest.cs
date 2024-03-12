namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using QueueDict = Dictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;
using ThreadDict = Dictionary<int, ServerThread>;
using Hwdtech.Ioc;
using Moq;

public class ServerThreadTest
{
    private readonly ICommand _newScope;
    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        // новый скоп
        _newScope = IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        );
        _newScope.Execute();

        // стратегия получения словаря с потоками
        var threadDict = new ThreadDict();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.GetThreadDict",
            (object[] args) => threadDict
        ).Execute();

        // стратегия получения словаря с очередями
        var senderDict = new QueueDict();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.GetSenderDict",
            (object[] args) => senderDict
        ).Execute();

        // стратегия инициализации и запуска сервера
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.Create&Start",
            (object[] args) =>
            {
                var threadId = (int)args[0];
                var actionCommand = new ActionCommand((Action)args[1]);

                var q = new BlockingCollection<ICommand>(100) { actionCommand };
                var serverThread = new ServerThread(q);

                IoC.Resolve<ThreadDict>("Thread.GetThreadDict").TryAdd(threadId, serverThread);
                IoC.Resolve<QueueDict>("Thread.GetSenderDict").TryAdd(threadId, q);

                serverThread.Start();
                return serverThread;
            }
        ).Execute();

        // стратегия получения HardStopCommand 
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.HardStop",
            (object[] args) =>
            {
                List<ICommand> cmdList = new()
                {
                    new HardStopCommand((ServerThread)args[0]),
                    new ActionCommand((Action)args[1])
                };
                return new MacroCommand(cmdList);
            }
        ).Execute();
    }

    [Fact]
    public void ServerThread_Can_Work_With_ExceptionCommands()
    {
        // id потока
        var threadId = 3;

        // создание и запуск сервера с id = 2
        IoC.Resolve<ServerThread>("Thread.Create&Start", threadId, () => { _newScope.Execute(); });

        // получение словаря с потоками
        var threadDict = IoC.Resolve<ThreadDict>("Thread.GetThreadDict");

        // получение словаря с очередями
        var senderDict = IoC.Resolve<QueueDict>("Thread.GetSenderDict");

        // создание обычной команды для потока
        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(cmd => cmd.Execute()).Verifiable();

        // создание команды с ошибкой
        var exceptionCommand = new Mock<ICommand>();
        exceptionCommand.Setup(cmd => cmd.Execute()).Throws<Exception>().Verifiable();

        // создание синхронизатора потока
        var mre = new ManualResetEvent(false);

        // создание команды, обрабатывающей исключения
        var exceptionHandler = new Mock<ICommand>();

        // регистрация обработчика ошибок в поток
        senderDict[threadId].Add(
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Exception.Handler",
                (object[] args) => exceptionHandler.Object
            )
        );

        // добавление обычной команды
        senderDict[threadId].Add(usualCommand.Object);

        // добавление команды с исключением
        senderDict[threadId].Add(exceptionCommand.Object);

        // добавление команды остановки потока
        senderDict[threadId].Add(
            IoC.Resolve<ICommand>(
                "Thread.HardStop",
                threadDict[threadId],
                () => { mre.Set(); }
            )
        );

        // добавление обычной команды
        senderDict[threadId].Add(usualCommand.Object);

        // закрытие калитки
        mre.WaitOne();

        // проверка на то, что исполнилась обычная команда
        usualCommand.Verify(cmd => cmd.Execute(), Times.Once());

        // проверка на то, что исполнилась команда с исключением
        exceptionCommand.Verify(cmd => cmd.Execute(), Times.Once());

        // проверка на то, что в очереди осталась обычная команда
        Assert.Single(senderDict[threadId]);

        // проверка на то, что сервер остановился
        Assert.False(threadDict[threadId].IsAlive);
    }
}
