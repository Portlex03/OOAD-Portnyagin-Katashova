using Hwdtech.Ioc;
using Hwdtech;
using Moq;
using WebHttp;

namespace SpaceBattle.Lib.Tests;

public class EndPointTest
{
    private readonly Mock<IStrategy> _getThreadIDByGameID = new();
    private readonly Mock<IStrategy> _createFromMesssageCmd = new();
    private readonly Mock<ICommand> _sendCmd = new();
    private readonly List<MessageContract> _messagesList = new();

    public EndPointTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.GetThreadIDByGameID",
            (object[] args) => _getThreadIDByGameID.Object.Execute(args)
        ).Execute();

        _createFromMesssageCmd.Setup(
            cmd => cmd.Execute(It.IsAny<object[]>())
        ).Returns(new ActionCommand(() => { }));

        IoC.Resolve<ICommand>(
            "IoC.Register", "Command.CreateFromMessage",
            (object[] args) => _createFromMesssageCmd.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCmd",
            (object[] args) => _sendCmd.Object
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
        _sendCmd.Setup(cmd => cmd.Execute()).Verifiable();

        var threadID = "thread64";
        _getThreadIDByGameID.Setup(
            cmd => cmd.Execute(It.IsAny<string>())
        ).Returns(threadID);

        var webApi = new WebApi();
        _messagesList.ForEach(webApi.ProcessMessage);

        _sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(_messagesList.Count));
    }

    [Fact]
    public void Impossible_To_Find_ThreadId_By_Game_Id()
    {
        _getThreadIDByGameID.Setup(
            cmd => cmd.Execute(It.IsAny<string>())
        ).Throws<Exception>().Verifiable();

        var webApi = new WebApi();

        var processMessagesCmd = new ActionCommand(() =>
            { _messagesList.ForEach(webApi.ProcessMessage); });

        Assert.Throws<Exception>(processMessagesCmd.Execute);
    }

    [Fact]
    public void Impossible_To_Send_EndPoint_Command_To_Thread()
    {
        var threadID = "thread256";
        _getThreadIDByGameID.Setup(
            cmd => cmd.Execute(It.IsAny<string>())
        ).Returns(threadID);

        _sendCmd.Setup(cmd => cmd.Execute()).Throws<Exception>().Verifiable();

        var webApi = new WebApi();

        var processMessagesCmd = new ActionCommand(() =>
            { _messagesList.ForEach(webApi.ProcessMessage); });

        Assert.Throws<Exception>(processMessagesCmd.Execute);
    }

    [Fact]
    public void Impossible_To_Create_Command_From_Message()
    {
        var threadID = "thread512";
        _getThreadIDByGameID.Setup(
            cmd => cmd.Execute(It.IsAny<string>())
        ).Returns(threadID);

        _createFromMesssageCmd.Setup(
            cmd => cmd.Execute(It.IsAny<object[]>())
        ).Throws<Exception>();

        var webApi = new WebApi();

        var processMessagesCmd = new ActionCommand(() =>
            { _messagesList.ForEach(webApi.ProcessMessage); });

        Assert.Throws<Exception>(processMessagesCmd.Execute);
    }
}
