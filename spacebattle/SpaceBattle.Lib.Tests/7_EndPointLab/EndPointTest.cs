using Hwdtech.Ioc;
using Hwdtech;
using Moq;
using WebHttp;

namespace SpaceBattle.Lib.Tests;

public class EndPointTest
{
    private readonly List<MessageContract> _messagesList;

    public EndPointTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        _messagesList = new List<MessageContract>()
        {
            new() {
                Type = "start movement",
                GameID = "asdfg",
                GameItemID = 1488,
                InitialValues = new() { { "Velocity", 2 } } },
            new() {
                Type = "start rotatement",
                GameID = "asdfg",
                GameItemID = 13,
                InitialValues = new() { {"AngularVelocity", 135 }, { "N", 8 } } },
            new() {
                Type = "stop movement",
                GameID = "asdfg",
                GameItemID = 666 },
            new() {
                Type = "stop shooting",
                GameID = "asdfg",
                GameItemID = 77 }
        };
    }

    [Fact]
    public void WebApi_Gets_Messages_And_Sends_It_To_Thread()
    {
        var getThreadIDByGameID = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.GetThreadIDByGameID",
            (object[] args) => getThreadIDByGameID.Object.Invoke(args)
        ).Execute();

        var createFromMesssageCmd = new Mock<IStrategy>();
        createFromMesssageCmd.Setup(
            cmd => cmd.Invoke(It.IsAny<object[]>())
        ).Returns(new ActionCommand(() => { }));

        IoC.Resolve<ICommand>(
            "IoC.Register", "Command.CreateFromMessage",
            (object[] args) => createFromMesssageCmd.Object.Invoke(args)
        ).Execute();

        var sendCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCmd",
            (object[] args) => sendCmd.Object
        ).Execute();

        var threadID = "thread64";
        getThreadIDByGameID.Setup(
            cmd => cmd.Invoke(It.IsAny<string>())
        ).Returns(threadID);

        var webApi = new WebApi();
        _messagesList.ForEach(webApi.ProcessMessage);

        sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(_messagesList.Count));
    }

    [Fact]
    public void Impossible_To_Find_ThreadId_By_Game_Id()
    {
        var getThreadIDByGameID = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.GetThreadIDByGameID",
            (object[] args) => getThreadIDByGameID.Object.Invoke(args)
        ).Execute();

        var createFromMesssageCmd = new Mock<IStrategy>();
        createFromMesssageCmd.Setup(
            cmd => cmd.Invoke(It.IsAny<object[]>())
        ).Returns(new ActionCommand(() => { }));

        IoC.Resolve<ICommand>(
            "IoC.Register", "Command.CreateFromMessage",
            (object[] args) => createFromMesssageCmd.Object.Invoke(args)
        ).Execute();

        var sendCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCmd",
            (object[] args) => sendCmd.Object
        ).Execute();

        getThreadIDByGameID.Setup(
            cmd => cmd.Invoke(It.IsAny<string>())
        ).Throws<Exception>().Verifiable();

        var webApi = new WebApi();

        var processMessagesCmd = new ActionCommand(() =>
            { _messagesList.ForEach(webApi.ProcessMessage); });

        Assert.Throws<Exception>(processMessagesCmd.Execute);
    }

    [Fact]
    public void Impossible_To_Send_EndPoint_Command_To_Thread()
    {
        var getThreadIDByGameID = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.GetThreadIDByGameID",
            (object[] args) => getThreadIDByGameID.Object.Invoke(args)
        ).Execute();

        var createFromMesssageCmd = new Mock<IStrategy>();
        createFromMesssageCmd.Setup(
            cmd => cmd.Invoke(It.IsAny<object[]>())
        ).Returns(new ActionCommand(() => { }));

        IoC.Resolve<ICommand>(
            "IoC.Register", "Command.CreateFromMessage",
            (object[] args) => createFromMesssageCmd.Object.Invoke(args)
        ).Execute();

        var sendCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCmd",
            (object[] args) => sendCmd.Object
        ).Execute();

        var threadID = "thread256";
        getThreadIDByGameID.Setup(
            cmd => cmd.Invoke(It.IsAny<string>())
        ).Returns(threadID);

        sendCmd.Setup(cmd => cmd.Execute()).Throws<Exception>().Verifiable();

        var webApi = new WebApi();

        var processMessagesCmd = new ActionCommand(() =>
            { _messagesList.ForEach(webApi.ProcessMessage); });

        Assert.Throws<Exception>(processMessagesCmd.Execute);
    }

    [Fact]
    public void Impossible_To_Create_Command_From_Message()
    {
        var getThreadIDByGameID = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.GetThreadIDByGameID",
            (object[] args) => getThreadIDByGameID.Object.Invoke(args)
        ).Execute();

        var createFromMesssageCmd = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Command.CreateFromMessage",
            (object[] args) => createFromMesssageCmd.Object.Invoke(args)
        ).Execute();

        var sendCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCmd",
            (object[] args) => sendCmd.Object
        ).Execute();

        var threadID = "thread512";
        getThreadIDByGameID.Setup(
            cmd => cmd.Invoke(It.IsAny<string>())
        ).Returns(threadID);

        createFromMesssageCmd.Setup(
            cmd => cmd.Invoke(It.IsAny<object[]>())
        ).Throws<Exception>();

        var webApi = new WebApi();

        var processMessagesCmd = new ActionCommand(() =>
            { _messagesList.ForEach(webApi.ProcessMessage); });

        Assert.Throws<Exception>(processMessagesCmd.Execute);
    }
}
