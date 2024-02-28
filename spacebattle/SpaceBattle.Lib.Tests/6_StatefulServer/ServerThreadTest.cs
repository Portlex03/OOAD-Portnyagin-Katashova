namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using QueueDict = Dictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;
using ThreadDict = Dictionary<int, ServerThread>;
using Hwdtech.Ioc;
using Moq;

public class ServerThreadTest
{
    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        // новый скоп
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

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
                return new ActionCommand(() =>
                {
                    new HardStopCommand((ServerThread)args[0]).Execute();
                    new ActionCommand((Action)args[1]).Execute();
                });
            }
        ).Execute();

        // Стратегия получения SoftStopCommand
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SoftStop",
            (object[] args) => new SoftStopCommand((ServerThread)args[0], (Action)args[1])
        ).Execute();
    }

    [Fact]
    public void HardStopMustStopServerThread()
    {
        var threadId = 1;
        var serverThread = IoC.Resolve<ServerThread>("Thread.Create&Start", threadId, () => { });

        var senderDict = IoC.Resolve<QueueDict>("Thread.GetSenderDict");

        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(cmd => cmd.Execute()).Verifiable();

        var mre = new ManualResetEvent(false);

        senderDict[threadId].Add(usualCommand.Object);

        senderDict[threadId].Add(usualCommand.Object);

        senderDict[threadId].Add(IoC.Resolve<ICommand>("Thread.HardStop", serverThread, () => { mre.Set(); }));

        senderDict[threadId].Add(usualCommand.Object);

        mre.WaitOne();

        usualCommand.Verify(cmd => cmd.Execute(), Times.Exactly(2));
        Assert.Single(senderDict[threadId]);
        Assert.False(serverThread.IsAlive);
    }

    [Fact]
    public void SoftStopMustStopServerThread()
    {
        var threadId = 2;
        var serverThread = IoC.Resolve<ServerThread>("Thread.Create&Start", threadId, () => { });

        var senderDict = IoC.Resolve<QueueDict>("Thread.GetSenderDict");

        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(cmd => cmd.Execute()).Verifiable();

        var mre = new ManualResetEvent(false);

        senderDict[threadId].Add(usualCommand.Object);

        senderDict[threadId].Add(usualCommand.Object);

        senderDict[threadId].Add(IoC.Resolve<ICommand>("Thread.SoftStop", serverThread, () => { mre.Set(); }));

        senderDict[threadId].Add(usualCommand.Object);

        senderDict[threadId].Add(usualCommand.Object);

        mre.WaitOne();

        usualCommand.Verify(cmd => cmd.Execute(), Times.Exactly(4));
        Assert.True(serverThread.QueueIsEmpty);
        Assert.False(serverThread.IsAlive);
    }

    [Fact]
    public void ServerThreadCanWorkWithExceptionCommands()
    {
        var threadId = 3;
        var serverThread = IoC.Resolve<ServerThread>("Thread.Create&Start", threadId, () => { });

        var senderDict = IoC.Resolve<QueueDict>("Thread.GetSenderDict");

        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(cmd => cmd.Execute()).Verifiable();

        var exceptionHandler = new Mock<ICommand>();

        var exceptionCommand = new Mock<ICommand>();
        exceptionCommand.Setup(cmd => cmd.Execute()).Throws<Exception>().Verifiable();

        var mre = new ManualResetEvent(false);

        senderDict[threadId].Add(
            IoC.Resolve<ICommand>(
                "Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
            )
        );

        senderDict[threadId].Add(
            IoC.Resolve<ICommand>(
                "IoC.Register", "Exception.Handler",
                (object[] args) => exceptionHandler.Object
            )
        );

        senderDict[threadId].Add(usualCommand.Object);

        senderDict[threadId].Add(exceptionCommand.Object);

        senderDict[threadId].Add(IoC.Resolve<ICommand>("Thread.HardStop", serverThread, () => { mre.Set(); }));

        senderDict[threadId].Add(usualCommand.Object);

        mre.WaitOne();

        usualCommand.Verify(cmd => cmd.Execute(), Times.Once());
        exceptionCommand.Verify(cmd => cmd.Execute(), Times.Once());

        Assert.Single(senderDict[threadId]);
        Assert.False(serverThread.IsAlive);
    }
}
