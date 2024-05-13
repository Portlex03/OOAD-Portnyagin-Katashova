namespace SpaceBattle.Lib.Tests;

using Hwdtech.Ioc;
using Hwdtech;
using Moq;

using QueueDict = System.Collections.Concurrent.ConcurrentDictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;

public class ServerThreadTest
{
    private readonly ICommand _newScope;
    public ServerThreadTest()
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
    public void ServerThread_Can_Work_With_ExceptionCommands()
    {
        var threadId = 3;

        IoC.Resolve<ICommand>(
            "Thread.Create&Start",
            threadId,
            () => { _newScope.Execute(); }
        ).Execute();

        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(cmd => cmd.Execute()).Verifiable();

        var exceptionCommand = new Mock<ICommand>();
        exceptionCommand.Setup(cmd => cmd.Execute()).Throws<Exception>().Verifiable();

        var mre = new ManualResetEvent(false);

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
            (ICommand)new RegisterExceptionHandlerCommand().Invoke()
        ).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, exceptionCommand.Object).Execute();

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
             IoC.Resolve<ICommand>(
                "Thread.HardStop", threadId, () => { mre.Set(); }
            )
        ).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        mre.WaitOne();

        usualCommand.Verify(cmd => cmd.Execute(), Times.Once());

        exceptionCommand.Verify(cmd => cmd.Execute(), Times.Once());

        Assert.Single(IoC.Resolve<QueueDict>("Thread.GetSenderDict")[threadId]);
    }
}
