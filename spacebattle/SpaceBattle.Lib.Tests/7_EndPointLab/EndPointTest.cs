namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;
using WebHttp;

public class EndPointTest
{
    readonly Mock<ICommand> _sendCmd;
    public EndPointTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _sendCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCmd",
            (object[] args) => _sendCmd.Object
        ).Execute();

        var createFromMesssageCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Command.CreateFromMessage",
            (object[] args) => createFromMesssageCmd.Object
        ).Execute();
    }

    [Fact]
    public void WebApi_Gets_Messages_And_Sends_It_To_Thread()
    {     
        var messagesList = new List<MessageContract>()
        {
            new() { Type = "start movement", GameId = "asdfg", GameItemId = 1488, InitialValues = new() { { "Velocity", 2 } } },
            new() { Type = "start rotatement", GameId = "asdfg", GameItemId = 13, InitialValues = new() { {"AngularVelocity", 135 }, { "N", 8 } } },
            new() { Type = "stop movement", GameId = "asdfg", GameItemId = 666 },
            new() { Type = "stop shooting", GameId = "asdfg", GameItemId = 77 }
        };
        var webApi = new WebApi();
        var length = messagesList.Count;

        messagesList.ForEach(webApi.GetMessage);
        
        _sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(length));
    }
}
