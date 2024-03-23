namespace SpaceBattle.Lib.Tests;

using Hwdtech.Ioc;
using Hwdtech;
using Moq;

using QueueDict = Dictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;

public class ServerThreadTest
{
    private readonly ICommand _newScope;
    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _newScope = (ICommand)new RegisterIoCScope().Execute();
        _newScope.Execute();

        ((ICommand)new RegisterGetThreadSenderDictCommand().Execute()).Execute();

        ((ICommand)new RegisterGetThreadDictCommand().Execute()).Execute();

        ((ICommand)new RegisterSendCommand().Execute()).Execute();

        ((ICommand)new RegisterServerThreadCreateAndStartCommand().Execute()).Execute();

        ((ICommand)new RegisterHardStopCommand().Execute()).Execute();
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
            (ICommand)new RegisterExceptionHandlerCommand().Execute()
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
