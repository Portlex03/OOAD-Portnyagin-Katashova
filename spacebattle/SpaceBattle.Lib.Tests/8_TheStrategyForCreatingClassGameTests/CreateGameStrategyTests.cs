namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class CreateGameStrategyTests
{
    public CreateGameStrategyTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void ScopeThrowsExceptionTest()
    {
        Mock<IStrategy> mockGetGameIdStrategy = new();
        Mock<object> quantumTime = new();

        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        CreateGameStrategy createGameStrategy = new();

        Assert.Throws<InvalidOperationException>(() => createGameStrategy.Invoke(quantumTime.Object));

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
    }

    [Fact]
    public void ArgsEmptyScopeThrowsExceptionTest()
    {
        String gameId = "id1";
        Mock<IStrategy> mockGetGameIdStrategy = new();

        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        CreateGameStrategy createGameStrategy = new();

        Assert.Throws<IndexOutOfRangeException>(() => createGameStrategy.Invoke());

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Never());
    }

    [Fact]
    public void GameAsCommandStrategyThrowsInternalErrorTest()
    {
        String gameId = "id1";
        Mock<object> quantumTime = new();
        Mock<IStrategy> mockGetGameIdStrategy = new();
        Mock<IStrategy> mockGetGameAsCommandStrategy = new();

        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();
        mockGetGameAsCommandStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Commands.GameAsCommand",
            (object[] args) => mockGetGameAsCommandStrategy.Object.Invoke(args)
        ).Execute();

        CreateGameStrategy createGameStrategy = new();

        Assert.Throws<InvalidOperationException>(() => createGameStrategy.Invoke(quantumTime.Object));

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 0)), Times.Exactly(1));
        object typeScope = new InitGameScopeStrategy().Invoke(quantumTime);
        mockGetGameAsCommandStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs =>
            factArgs[0].GetType() == typeScope.GetType() && factArgs[1] is Queue<ICommand>)), Times.Exactly(1));
    }

    [Fact]
    public void SuccessfulCreateGameStrategyTest()
    {
        String gameId = "id1";
        Mock<object> quantumTime = new();
        Mock<IStrategy> mockGetGameIdStrategy = new();
        Mock<IStrategy> mockGetGameAsCommandStrategy = new();
        Mock<ICommand> returnCommand = new();

        mockGetGameIdStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(gameId).Verifiable();
        mockGetGameAsCommandStrategy.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(returnCommand.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGetGameIdStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Commands.GameAsCommand",
            (object[] args) => mockGetGameAsCommandStrategy.Object.Invoke(args)
        ).Execute();

        CreateGameStrategy createGameStrategy = new();

        ICommand actualCommand = (ICommand)createGameStrategy.Invoke(quantumTime.Object);

        Assert.True(actualCommand.Equals(returnCommand.Object));

        mockGetGameIdStrategy.Verify(strategy => strategy.Invoke(It.IsAny<object[]>()), Times.Exactly(1));
        object typeScope = new InitGameScopeStrategy().Invoke(quantumTime);
        mockGetGameAsCommandStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs =>
            factArgs[0].GetType() == typeScope.GetType() && factArgs[1] is Queue<ICommand>)), Times.Exactly(1));
    }
}
