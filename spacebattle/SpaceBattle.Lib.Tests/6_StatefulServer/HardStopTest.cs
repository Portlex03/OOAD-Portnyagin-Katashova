namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

using QueueDict = Dictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;

public class HardStopTest
{
    private readonly ICommand _newScope;
    public HardStopTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _newScope = (ICommand)new RegisterIoCScope().RunStrategy();
        _newScope.Execute();

        ((ICommand)new RegisterGetThreadSenderDictCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterSendCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterServerThreadCreateAndStartCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterHardStopCommand().RunStrategy()).Execute();
    }

    [Fact]
    public void Successful_HardStop_ServerThread()
    {
        var threadId = 1;

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
                "Thread.HardStop", serverThread, () => { mre.Set(); }
            )
        ).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        mre.WaitOne();

        usualCommand.Verify(cmd => cmd.Execute(), Times.Exactly(2));

        Assert.Single(IoC.Resolve<QueueDict>("Thread.GetSenderDict")[threadId]);
    }

    [Fact]
    public void HardStop_Incorrect_ServerThread_With_Exception()
    {
        int threadId = 2;

        var serverThread = IoC.Resolve<ServerThread>(
            "Thread.Create&Start", threadId, () => { _newScope.Execute(); });

        var hardStopCmd = new HardStopCommand(serverThread);

        Assert.Throws<Exception>(hardStopCmd.Execute);

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
             IoC.Resolve<ICommand>("Thread.HardStop", serverThread)
        ).Execute();
    }
}
