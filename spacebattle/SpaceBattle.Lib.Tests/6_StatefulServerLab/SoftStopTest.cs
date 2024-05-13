namespace SpaceBattle.Lib.Tests;

using Hwdtech.Ioc;
using Hwdtech;
using Moq;

using ThreadDict = System.Collections.Concurrent.ConcurrentDictionary<int, ServerThread>;
using QueueDict = System.Collections.Concurrent.ConcurrentDictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;

public class SoftStopTest
{
    private readonly ICommand _newScope;
    public SoftStopTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _newScope = (ICommand)new RegisterIoCScope().Invoke();
        _newScope.Execute();

        ((ICommand)new RegisterGetThreadSenderDictCommand().Invoke()).Execute();

        ((ICommand)new RegisterGetThreadDictCommand().Invoke()).Execute();

        ((ICommand)new RegisterSendCommand().Invoke()).Execute();

        ((ICommand)new RegisterServerThreadCreateAndStartCommand().Invoke()).Execute();

        ((ICommand)new RegisterHardStopCommand().Invoke()).Execute();

        ((ICommand)new RegisterSoftStopCommand().Invoke()).Execute();
    }

    [Fact]
    public void Successful_SoftStop_ServerThread()
    {
        var threadId = 4;

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
                "Thread.SoftStop",
                threadId,
                () => { mre.Set(); }
            )
        ).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        mre.WaitOne();

        usualCommand.Verify(cmd => cmd.Execute(), Times.Exactly(4));

        Assert.Empty(IoC.Resolve<QueueDict>("Thread.GetSenderDict")[threadId]);
    }

    [Fact]
    public void SoftStop_ServerThread_In_Another_Thread_With_Exception()
    {
        int threadId = 5;

        IoC.Resolve<ICommand>(
            "Thread.Create&Start",
            threadId,
            () => { _newScope.Execute(); }
        ).Execute();

        var softStopCmd = new SoftStopCommand(
            IoC.Resolve<ThreadDict>("Thread.GetDict")[threadId]
        );

        Assert.Throws<Exception>(softStopCmd.Execute);

        softStopCmd = IoC.Resolve<SoftStopCommand>("Thread.SoftStop", threadId);

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, softStopCmd).Execute();
    }
}
