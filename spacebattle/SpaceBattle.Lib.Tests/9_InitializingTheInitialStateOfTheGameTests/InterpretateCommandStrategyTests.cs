namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class InterpretateCommandStrategyTests
{
    public InterpretateCommandStrategyTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void EmptyArgsMessageThrowsExceptionTest()
    {
        InterpretateCommandStrategy interpretateCommandStrategy = new();

        Assert.Throws<IndexOutOfRangeException>(() => interpretateCommandStrategy.Invoke());
    }

    [Fact]
    public void TypeDoesNotExistInSpaceBattleThrowsExceptionTest()
    {
        Mock<IMessage> mockMessage = new();

        mockMessage.SetupGet(x => x.type).Returns("Jump").Verifiable();
        mockMessage.Setup(x => x.gameId).Returns("1").Verifiable();
        mockMessage.Setup(x => x.gameItemId).Returns("1").Verifiable();
        mockMessage.Setup(x => x.properties).Returns(new Dictionary<string, object>()).Verifiable();

        Mock<IStrategy> mockGetObjectStrategy = new();
        Mock<IUObject> mockTarget = new();
        Mock<ICommand> mockCommand = new();

        mockGetObjectStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockTarget.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetObject",
            (object[] args) => mockGetObjectStrategy.Object.Invoke(args)
        ).Execute();


        InterpretateCommandStrategy interpretateCommandStrategy = new();

        Assert.Throws<ArgumentException>(() => interpretateCommandStrategy.Invoke(mockMessage.Object));

        mockMessage.Verify(s => s.type, Times.Exactly(1));
        mockMessage.Verify(s => s.gameItemId, Times.Exactly(1));
        mockMessage.Verify(s => s.gameItemId, Times.Exactly(1));
        mockMessage.Verify(s => s.properties, Times.Exactly(1));
    }

    [Fact]
    public void SuccessfulInterpretateCommandStrategyTest()
    {
        Mock<IMessage> mockMessage = new();
        string key = "StartMove";

        mockMessage.SetupGet(x => x.type).Returns(key).Verifiable();
        mockMessage.Setup(x => x.gameId).Returns("1").Verifiable();
        mockMessage.Setup(x => x.gameItemId).Returns("1").Verifiable();
        mockMessage.Setup(x => x.properties).Returns(new Dictionary<string, object>()).Verifiable();

        Mock<IStrategy> mockGetObjectStrategy = new();
        Mock<IUObject> mockTarget = new();
        Mock<IStrategy> mockStrategy = new();
        Mock<ICommand> mockCommand = new();

        mockGetObjectStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockTarget.Object).Verifiable();

        mockStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockCommand.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetObject",
            (object[] args) => mockGetObjectStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle." + key,
            (object[] args) => mockStrategy.Object.Invoke(args)
        ).Execute();


        InterpretateCommandStrategy interpretateCommandStrategy = new();

        try
        {
            ICommand result = (ICommand)interpretateCommandStrategy.Invoke(mockMessage.Object);
        }
        catch (Exception ex)
        {
            Assert.Fail("Test should be performed without exceptions: " + ex.Message);
        }

        mockMessage.Verify(s => s.type, Times.Exactly(1));
        mockMessage.Verify(s => s.gameItemId, Times.Exactly(1));
        mockMessage.Verify(s => s.gameItemId, Times.Exactly(1));
        mockMessage.Verify(s => s.properties, Times.Exactly(1));
    }
}
