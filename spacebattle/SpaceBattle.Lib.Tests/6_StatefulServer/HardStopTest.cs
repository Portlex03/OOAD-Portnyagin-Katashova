namespace SpaceBattle.Lib.Tests;
using QueueDict = Dictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;
using ThreadDict = Dictionary<int, ServerThread>;
using System.Collections.Concurrent;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class HardStopTest
{
    private readonly ICommand _newScope;
    public HardStopTest()
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
    public void Successful_HardStop_ServerThread()
    {
        // id сервера
        var threadId = 1;

        // создание и запуск сервера с id = 1
        IoC.Resolve<ServerThread>("Thread.Create&Start", threadId, () => { _newScope.Execute(); });

        // получение словаря с потоками
        var threadDict = IoC.Resolve<ThreadDict>("Thread.GetThreadDict");
        
        // получение словаря с очередями
        var senderDict = IoC.Resolve<QueueDict>("Thread.GetSenderDict");

        // создание обычной команды для потока
        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(cmd => cmd.Execute()).Verifiable();

        // создаём синхронизатор потока
        var mre = new ManualResetEvent(false);

        // отправка 2 обыных команд в очередь
        senderDict[threadId].Add(usualCommand.Object);

        senderDict[threadId].Add(usualCommand.Object);

        // отправка команды остановки сервера
        senderDict[threadId].Add(
            IoC.Resolve<ICommand>(
                "Thread.HardStop",
                threadDict[threadId],
                () => { mre.Set(); }
            )
        );

        // отправка обычной команды
        senderDict[threadId].Add(usualCommand.Object);

        // закрытие калитки
        mre.WaitOne();

        // проверка на то, что обычная команда исполнилась 2 раза
        usualCommand.Verify(cmd => cmd.Execute(), Times.Exactly(2));
        
        // проверка на то, что в очереди осталась одна команда
        Assert.Single(senderDict[threadId]);

        // проверка на то, что сервер остановился
        Assert.False(threadDict[threadId].IsAlive);
    }

    // [Fact]
    // public void HardStop_Incorrect_ServerThread_With_Exception()
    // {

    // }
}