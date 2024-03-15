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

        _newScope = (ICommand)new RegisterIoCScope().RunStrategy();
        _newScope.Execute();

        ((ICommand)new RegisterGetThreadSenderDictCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterSendCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterServerThreadCreateAndStartCommand().RunStrategy()).Execute();

        ((ICommand)new RegisterHardStopCommand().RunStrategy()).Execute();
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

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
            (ICommand)new RegisterExceptionHandlerCommand().RunStrategy()
        ).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, exceptionCommand.Object).Execute();

        IoC.Resolve<ICommand>(
            "Thread.SendCommand", threadId,
             IoC.Resolve<ICommand>(
                "Thread.HardStop", serverThread, () => { mre.Set(); }
            )
        ).Execute();

        IoC.Resolve<ICommand>("Thread.SendCommand", threadId, usualCommand.Object).Execute();

        mre.WaitOne();

        usualCommand.Verify(cmd => cmd.Execute(), Times.Once());

        exceptionCommand.Verify(cmd => cmd.Execute(), Times.Once());

        Assert.Single(IoC.Resolve<QueueDict>("Thread.GetSenderDict")[threadId]);
    }
}
