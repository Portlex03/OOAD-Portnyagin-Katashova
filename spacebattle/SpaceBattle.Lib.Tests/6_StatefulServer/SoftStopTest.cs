namespace SpaceBattle.Lib.Tests;
using QueueDict = Dictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;
using ThreadDict = Dictionary<int, ServerThread>;
using System.Collections.Concurrent;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class SoftStopTest
{
    private readonly ICommand _newScope;
    public SoftStopTest()
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

        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SoftStop",
            (object[] args) =>
            {
                Action? action = null;
                if (args.Length > 1)
                    action = (Action)args[1];

                return new SoftStopCommand((ServerThread)args[0], action);
            }
        ).Execute();
    }

    [Fact]
    public void Successful_SoftStop_ServerThread()
    {
        var threadId = 3;

        var serverThread = IoC.Resolve<ServerThread>(
            "Thread.Create&Start", threadId, () => { _newScope.Execute(); });

        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(cmd => cmd.Execute()).Verifiable();

        var mre = new ManualResetEvent(false);

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
            IoC.Resolve<ICommand>(
                "Thread.SoftStop",
                serverThread,
                () => { mre.Set(); }
            )
        ).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        mre.WaitOne();

        usualCommand.Verify(cmd => cmd.Execute(), Times.Exactly(4));

        Assert.True(serverThread.QueueIsEmpty);
    }

    [Fact]
    public void SoftStop_ServerThread_In_Another_Thread_With_Exception()
    {
        int threadId = 4;

        var serverThread = IoC.Resolve<ServerThread>(
            "Thread.Create&Start", threadId, () => { _newScope.Execute(); });

        var softStopCmd = new SoftStopCommand(serverThread);

        Assert.Throws<Exception>(softStopCmd.Execute);

        IoC.Resolve<ICommand>(
            "Thread.SendCommand",
            threadId,
            IoC.Resolve<ICommand>("Thread.SoftStop", serverThread)
        ).Execute();
    }
}
