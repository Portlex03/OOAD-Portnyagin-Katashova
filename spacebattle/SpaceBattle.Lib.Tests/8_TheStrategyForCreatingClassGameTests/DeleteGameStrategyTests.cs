namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class DeleteGameStrategyTests
{
    public DeleteGameStrategyTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void EmptyParamsDeleteGameStrategyThrowsExceptionTest()
    {
        Mock<IStrategy> deleteGameStrategyMock = new();
        Mock<ICommand> returnGameCommandMock = new();

        deleteGameStrategyMock.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(returnGameCommandMock.Object).Verifiable();
        returnGameCommandMock.Setup(cmd => cmd.Execute()).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.DeleteGame",
            (object[] args) => deleteGameStrategyMock.Object.Invoke(args)
        ).Execute();

        DeleteGameStrategy deleteGameStrategy = new();

        Assert.Throws<IndexOutOfRangeException>(() => deleteGameStrategy.Invoke());

        deleteGameStrategyMock.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 1)), Times.Never());
        returnGameCommandMock.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void DeleteGameStrategyThrowsInternalExceptionTest()
    {
        Mock<IStrategy> deleteGameStrategyMock = new();
        Mock<ICommand> returnGameCommandMock = new();

        deleteGameStrategyMock.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Throws<InvalidOperationException>().Verifiable();
        returnGameCommandMock.Setup(cmd => cmd.Execute()).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.DeleteGame",
            (object[] args) => deleteGameStrategyMock.Object.Invoke(args)
        ).Execute();

        DeleteGameStrategy deleteGameStrategy = new();

        Mock<object> mockId = new();
        Assert.Throws<InvalidOperationException>(() => deleteGameStrategy.Invoke(mockId.Object));

        deleteGameStrategyMock.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 1)), Times.Exactly(1));
        returnGameCommandMock.Verify(cmd => cmd.Execute(), Times.Never());
    }

    [Fact]
    public void SuccessfulDeleteGameStrategyTest()
    {
        Mock<IStrategy> deleteGameStrategyMock = new();
        Mock<ICommand> returnGameCommandMock = new();

        deleteGameStrategyMock.Setup(strategy => strategy.Invoke(It.IsAny<object[]>())).Returns(returnGameCommandMock.Object).Verifiable();
        returnGameCommandMock.Setup(cmd => cmd.Execute()).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.DeleteGame",
            (object[] args) => deleteGameStrategyMock.Object.Invoke(args)
        ).Execute();

        DeleteGameStrategy deleteGameStrategy = new();

        Mock<object> mockId = new();
        ICommand factCommand = (ICommand)deleteGameStrategy.Invoke(mockId.Object);

        Assert.True(factCommand.Equals(returnGameCommandMock.Object));

        deleteGameStrategyMock.Verify(strategy => strategy.Invoke(It.Is<object[]>(factArgs => factArgs.Count() == 1)), Times.Exactly(1));
        returnGameCommandMock.Verify(cmd => cmd.Execute(), Times.Never());
    }
}
