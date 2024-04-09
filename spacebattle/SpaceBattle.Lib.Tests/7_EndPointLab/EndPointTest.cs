namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;
using WebHttp;

public class EndPointTest
{
    [Fact]
    public void WebApi_Gets_Messages_And_Sends_It_To_Thread()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Thead.GetByGameId",
            (object[] args) => "fuck"
        ).Execute();

        var createFromMesssageCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Command.CreateFromMessage",
            (object[] args) => createFromMesssageCmd.Object
        ).Execute();

        var sendCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCommand",
            (object[] args) => sendCmd.Object
        ).Execute();

        var messagesList = new MessageContract[4]
        {
            new() { Type = "start movement", GameId = "asdfg", GameItemId = 1488, InitialValues = new() { { "Velocity", 2 } } },
            new() { Type = "start rotatement", GameId = "asdfg", GameItemId = 13, InitialValues = new() { {"AngularVelocity", 135 }, { "N", 8 } } },
            new() { Type = "stop movement", GameId = "asdfg", GameItemId = 666 },
            new() { Type = "stop shooting", GameId = "asdfg", GameItemId = 77 }
        };
        var webApi = new WebApi();
        var length = messagesList.Length;

        Array.ForEach(messagesList, webApi.GetMessage);
        
        createFromMesssageCmd.Verify(cmd => cmd.Execute(), Times.Exactly(length));
        sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(length));
    }
}
