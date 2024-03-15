namespace SpaceBattle.Lib.Tests;

using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class SoftStopTest
{
    private readonly ICommand _newScope;
    public SoftStopTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _newScope = (ICommand)new RegisterIoCScope().RunStrategy();
        _newScope.Execute();

        ((ICommand)new RegisterGetThreadSenderDictCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterSendCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterServerThreadCreateAndStartCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterHardStopCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterSoftStopCommand().RunStrategy()).Execute();
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

        softStopCmd = IoC.Resolve<SoftStopCommand>("Thread.SoftStop", serverThread);

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, softStopCmd).Execute();
    }
}
