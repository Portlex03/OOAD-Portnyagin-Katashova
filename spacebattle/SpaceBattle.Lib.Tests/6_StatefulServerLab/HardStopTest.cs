namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

using ThreadDict = System.Collections.Concurrent.ConcurrentDictionary<int, ServerThread>;
using QueueDict = System.Collections.Concurrent.ConcurrentDictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;

public class HardStopTest
{
    private readonly ICommand _newScope;
    public HardStopTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _newScope = (ICommand)new RegisterIoCScope().Invoke();
        _newScope.Execute();

        ((ICommand)new RegisterGetThreadSenderDictCommand().Invoke()).Execute();

        ((ICommand)new RegisterGetThreadDictCommand().Invoke()).Execute();

        ((ICommand)new RegisterSendCommand().Invoke()).Execute();

        ((ICommand)new RegisterServerThreadCreateAndStartCommand().Invoke()).Execute();

        ((ICommand)new RegisterHardStopCommand().Invoke()).Execute();
    }

    [Fact]
    public void Successful_HardStop_ServerThread()
    {
        var threadId = 1;

        IoC.Resolve<ICommand>(
            "Thread.Create&Start",
            threadId,
            () => { _newScope.Execute(); }
        ).Execute();

        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(cmd => cmd.Execute()).Verifiable();

        var mre = new ManualResetEvent(false);

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
             IoC.Resolve<ICommand>(
                "Thread.HardStop", threadId, () => { mre.Set(); }
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

        IoC.Resolve<ICommand>(
            "Thread.Create&Start",
            threadId,
            () => { _newScope.Execute(); }
        ).Execute();

        var hardStopCmd = new HardStopCommand(
            IoC.Resolve<ThreadDict>("Thread.GetDict")[threadId]
        );

        Assert.Throws<Exception>(hardStopCmd.Execute);

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
            IoC.Resolve<ICommand>("Thread.HardStop", threadId)
        ).Execute();
    }
}
