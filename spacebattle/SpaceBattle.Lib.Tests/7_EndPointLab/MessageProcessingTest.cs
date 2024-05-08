namespace SpaceBattle.Lib.Test;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;


public class MessageProcessingTest
{
    public MessageProcessingTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void GetMessageReturnsException()
    {
        Mock<IStrategy> _getMessage = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage",
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        _getMessage.Setup(strategy => strategy.Execute()).Throws<InvalidOperationException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidOperationException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));
    }

    [Fact]
    public void GetMessageReturnsGameIdIncorrest()
    {
        string gameIdIncorrect = "01";
        Mock<IMessage> _message = new();
        Mock<IStrategy> _getMessage = new();
        Mock<IStrategy> _sendCommandInGame = new();
        Mock<IStrategy> _getInterpretateMessageCommand = new();
        Mock<ICommand> _interpretCmd = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage",
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetInterpretateMessageCommand",
            (object[] args) => _getInterpretateMessageCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdIncorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidOperationException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdIncorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void SuccessfulMessageProcessing()
    {
        string gameIdCorrect = "asdfg";
        Mock<IMessage> _message = new();
        Mock<IStrategy> _getMessage = new();
        Mock<IStrategy> _sendCommandInGame = new();
        Mock<ICommand> _sendCmd = new();
        Mock<IStrategy> _getInterpretateMessageCommand = new();
        Mock<ICommand> _interpretCmd = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage",
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetInterpretateMessageCommand",
            (object[] args) => _getInterpretateMessageCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _sendCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_sendCmd.Object).Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        try
        {
            act();
        }
        catch (Exception ex)
        {
            Assert.Fail("Test should be performed without exceptions" + ex.Message);
        }

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdCorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void SendCommandInGameReturnsException()
    {
        string gameIdCorrect = "asdfg";
        Mock<IMessage> _message = new();
        Mock<IStrategy> _getMessage = new();
        Mock<IStrategy> _sendCommandInGame = new();
        Mock<IStrategy> _getInterpretateMessageCommand = new();
        Mock<ICommand> _interpretCmd = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage",
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetInterpretateMessageCommand",
            (object[] args) => _getInterpretateMessageCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidOperationException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdCorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void GetInterpretateMessageCommandReturnsException()
    {
        string gameIdCorrect = "asdfg";
        Mock<IMessage> _message = new();
        Mock<IStrategy> _getMessage = new();
        Mock<IStrategy> _getInterpretateMessageCommand = new();
        Mock<ICommand> _interpretCmd = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage",
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetInterpretateMessageCommand",
            (object[] args) => _getInterpretateMessageCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<NullCommandException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<NullCommandException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Never()); // Never

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void GetMessageReturnsNonTypeIMessage()
    {
        Mock<IStrategy> _getMessage = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage",
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        _getMessage.Setup(strategy => strategy.Execute()).Throws<InvalidCastException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidCastException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));
    }

    [Fact]
    public void GetInterpretateMessageCommandReturnsNonTypeICommand()
    {
        string gameIdCorrect = "asdfg";
        Mock<IMessage> _message = new();
        Mock<IStrategy> _getMessage = new();
        Mock<IStrategy> _getInterpretateMessageCommand = new();
        Mock<ICommand> _interpretCmd = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage",
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetInterpretateMessageCommand",
            (object[] args) => _getInterpretateMessageCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidCastException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidCastException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Never());

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void SendCommandInGameReturnsNonTypeICommand()
    {
        string gameIdCorrect = "asdfg";
        Mock<IMessage> _message = new();
        Mock<IStrategy> _getMessage = new();
        Mock<IStrategy> _sendCommandInGame = new();
        Mock<IStrategy> _getInterpretateMessageCommand = new();
        Mock<ICommand> _interpretCmd = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage",
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetInterpretateMessageCommand",
            (object[] args) => _getInterpretateMessageCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Throws<InvalidCastException>().Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidCastException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdCorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void SendCmdReturnsException()
    {
        string gameIdCorrect = "asdfg";
        Mock<IMessage> _message = new();
        Mock<IStrategy> _getMessage = new();
        Mock<IStrategy> _sendCommandInGame = new();
        Mock<ICommand> _sendCmd = new();
        Mock<IStrategy> _getInterpretateMessageCommand = new();
        Mock<ICommand> _interpretCmd = new();

        IoC.Resolve<ICommand>("IoC.Register", "GetMessage",
            (object[] args) => _getMessage.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommandInGame",
            (object[] args) => _sendCommandInGame.Object.Execute(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetInterpretateMessageCommand",
            (object[] args) => _getInterpretateMessageCommand.Object.Execute(args)
        ).Execute();

        _message.SetupGet(strategy => strategy.gameId).Returns(gameIdCorrect).Verifiable();

        _getMessage.Setup(strategy => strategy.Execute()).Returns(_message.Object).Verifiable();

        _interpretCmd.Setup(cmd => cmd.Execute()).Verifiable();

        _sendCmd.Setup(cmd => cmd.Execute()).Throws<InvalidOperationException>().Verifiable();

        _getInterpretateMessageCommand.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_interpretCmd.Object).Verifiable();

        _sendCommandInGame.Setup(strategy => strategy.Execute(It.IsAny<object[]>())).Returns(_sendCmd.Object).Verifiable();


        MessageProcessing msgProcess = new MessageProcessing();

        var act = () => msgProcess.Execute();


        Assert.Throws<InvalidOperationException>(act);

        _getMessage.Verify(strategy => strategy.Execute(), Times.Exactly(1));

        object[] expectArgs = new object[] { _message.Object };
        _getInterpretateMessageCommand.Verify(strategy => strategy.Execute(It.Is<object[]>(factArg => factArg[0] == expectArgs[0])), Times.Exactly(1));

        _sendCommandInGame.Verify(strategy => strategy.Execute(It.Is<object[]>(
            factArg => (string)factArg[0] == gameIdCorrect && factArg[1] == _interpretCmd.Object)), Times.Exactly(1));

        _message.VerifyGet<string>(strategy => strategy.gameId, Times.Exactly(1));

        _sendCmd.Verify(cmd => cmd.Execute(), Times.Exactly(1));

        _interpretCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }
}
