namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

using QueueDict = Dictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;
using ThreadDict = Dictionary<int, ServerThread>;

public class ServerThreadTest
{
    private readonly ICommand _newScope;
    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _newScope = IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        );
        _newScope.Execute();

        var senderDict = new QueueDict();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.GetSenderDict",
            (object[] args) => senderDict
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCommand",
            (object[] args) =>
            {
                var threadId = (int)args[0];
                var cmd = (ICommand)args[1];

                var q = IoC.Resolve<QueueDict>("Thread.GetSenderDict")[threadId];

                return new SendCommand(q, cmd);
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.Create&Start",
            (object[] args) =>
            {
                var threadId = (int)args[0];
                var actionCommand = new ActionCommand((Action)args[1]);

                var q = new BlockingCollection<ICommand>(100) { actionCommand };
                var serverThread = new ServerThread(q);

                IoC.Resolve<QueueDict>("Thread.GetSenderDict").TryAdd(threadId, q);

                serverThread.Start();
                return serverThread;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.HardStop",
            (object[] args) =>
            {
                var cmdList = new List<ICommand> { new HardStopCommand((ServerThread)args[0]) };

                if (args.Length > 1)
                    cmdList.Add(new ActionCommand((Action)args[1]));
                
                return new MacroCommand(cmdList);
            }
        ).Execute();
    }

    [Fact]
    public void ServerThread_Can_Work_With_ExceptionCommands()
    {
        var threadId = 3;

        var serverThread = IoC.Resolve<ServerThread>(
            "Thread.Create&Start", threadId, () => { _newScope.Execute(); });

        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(cmd => cmd.Execute()).Verifiable();

        var exceptionCommand = new Mock<ICommand>();
        exceptionCommand.Setup(cmd => cmd.Execute()).Throws<Exception>().Verifiable();

        var mre = new ManualResetEvent(false);

        var exceptionHandler = new Mock<ICommand>();

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
            IoC.Resolve<ICommand>(
                "IoC.Register",
                "Exception.Handler",
                (object[] args) => exceptionHandler.Object
            )
        ).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, exceptionCommand.Object).Execute();

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
             IoC.Resolve<ICommand>(
                "Thread.HardStop",
                serverThread,
                () => { mre.Set(); }
            )
        ).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        mre.WaitOne();

        usualCommand.Verify(cmd => cmd.Execute(), Times.Once());

        exceptionCommand.Verify(cmd => cmd.Execute(), Times.Once());

        Assert.Single(IoC.Resolve<QueueDict>("Thread.GetSenderDict")[threadId]);
    }
}
