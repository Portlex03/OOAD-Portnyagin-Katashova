namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;
using WebHttp;
using System.Collections.Concurrent;

public class EndPointTest
{
    public EndPointTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        var startServerCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.Create&Start",
            (object[] args) =>
            {
                return new ActionCommand(() =>
                {
                    var threadID = (string)args[0];

                    startServerCmd.Object.Execute();

                    var q = new BlockingCollection<ICommand>(10);

                    IoC.Resolve<ICommand>(
                        "IoC.Register", $"Queue.{threadID}",
                        (object[] args) => q
                    ).Execute();
                });
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCmd",
            (object[] args) =>
            {
                var threadID = (string)args[0];
                var cmd = (ICommand)args[1];

                var q = IoC.Resolve<BlockingCollection<ICommand>>($"Queue.{threadID}");

                return new SendCommand(q, cmd);
            }
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
        var threadID = "thread64";

        var getThreadIDByGameID = new Mock<IStrategy>();
        getThreadIDByGameID.Setup(
            cmd => cmd.Execute(It.IsAny<object[]>())
        ).Returns(threadID);

        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.GetThreadIDByGameID",
            (object[] args) => getThreadIDByGameID.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("Thread.Create&Start", threadID).Execute();

        var messagesList = new List<MessageContract>()
        {
            new() { Type = "start movement", GameID = "asdfg", GameItemID = 1488, InitialValues = new() { { "Velocity", 2 } } },
            new() { Type = "start rotatement", GameID = "asdfg", GameItemID = 13, InitialValues = new() { {"AngularVelocity", 135 }, { "N", 8 } } },
            new() { Type = "stop movement", GameID = "asdfg", GameItemID = 666 },
            new() { Type = "stop shooting", GameID = "asdfg", GameItemID = 77 }
        };
        var webApi = new WebApi();
        var length = messagesList.Count;

        messagesList.ForEach(webApi.GetMessage);

        var q = IoC.Resolve<BlockingCollection<ICommand>>($"Queue.{threadID}");

        Assert.True(q.Count == 4);
    }
}
/// hwdtech ыыыыыыыыыыыыыыыыыыыыыы