namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using Xunit.Sdk;

public class ShotCommandTests
{
    public ShotCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void CreateObjectThrowsExceptionTest()
    {
        Mock<IShotable> shotable = new();

        Mock<IStrategy> mockGamesCreateObjectStrategy = new();

        mockGamesCreateObjectStrategy.Setup(
            strategy => strategy.Invoke()
        ).Throws<InvalidOperationException>().Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Object",
            (object[] args) => mockGamesCreateObjectStrategy.Object.Invoke(args)
        ).Execute();


        ShotCommand shotCommand = new(shotable.Object);


        Assert.Throws<InvalidOperationException>(() => shotCommand.Execute());


        mockGamesCreateObjectStrategy.Verify(strategy => strategy.Invoke(), Times.Exactly(1));
    }

    [Fact]
    public void CreateTorpedoThrowsExceptionTest()
    {
        Mock<IShotable> shotable = new();

        Mock<IStrategy> mockGamesCreateObjectStrategy = new();
        Mock<IUObject> mockTorpedo = new();

        Mock<IStrategy> mockGamesCreateTorpedoStrategy = new();

        mockGamesCreateObjectStrategy.Setup(
            strategy => strategy.Invoke()
        ).Returns(mockTorpedo.Object).Verifiable();

        mockGamesCreateTorpedoStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Throws<InvalidOperationException>().Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Object",
            (object[] args) => mockGamesCreateObjectStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Torpedo",
            (object[] args) => mockGamesCreateTorpedoStrategy.Object.Invoke(args)
        ).Execute();


        ShotCommand shotCommand = new(shotable.Object);


        Assert.Throws<InvalidOperationException>(() => shotCommand.Execute());


        mockGamesCreateObjectStrategy.Verify(strategy => strategy.Invoke(), Times.Exactly(1));

        mockGamesCreateTorpedoStrategy.Verify(strategy => strategy.Invoke(It.Is<IShotable>(
            factArgs => factArgs == shotable.Object)), Times.Exactly(1));
    }

    [Fact]
    public void StartMoveThrowsExceptionTest()
    {
        Mock<IShotable> shotable = new();

        Mock<IStrategy> mockGamesCreateObjectStrategy = new();
        Mock<IUObject> mockTorpedo = new();

        Mock<IStrategy> mockGamesCreateTorpedoStrategy = new();
        Mock<Dictionary<string, object>> mockInitialValues = new();


        mockGamesCreateObjectStrategy.Setup(
            strategy => strategy.Invoke()
        ).Returns(mockTorpedo.Object).Verifiable();

        mockGamesCreateTorpedoStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockInitialValues.Object).Verifiable();


        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Object",
            (object[] args) => mockGamesCreateObjectStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Torpedo",
            (object[] args) => mockGamesCreateTorpedoStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.StartMove",
            new Func<object[], object>((object[] args) => { throw new InvalidOperationException(); })
        ).Execute();


        ShotCommand shotCommand = new(shotable.Object);


        Assert.Throws<InvalidOperationException>(() => shotCommand.Execute());


        mockGamesCreateObjectStrategy.Verify(strategy => strategy.Invoke(), Times.Exactly(1));

        mockGamesCreateTorpedoStrategy.Verify(strategy => strategy.Invoke(It.Is<IShotable>(
            factArgs => factArgs == shotable.Object)), Times.Exactly(1));
    }

    [Fact]
    public void IdGetNewThrowsExceptionTest()
    {
        Mock<IShotable> shotable = new();

        Mock<IStrategy> mockGamesCreateObjectStrategy = new();
        Mock<IUObject> mockTorpedo = new();

        Mock<IStrategy> mockGamesCreateTorpedoStrategy = new();
        Mock<Dictionary<string, object>> mockInitialValues = new();

        Mock<IStrategy> mockSpaceBattleStartMoveStrategy = new();

        Mock<IStrategy> mockGamesIdGetNewStrategy = new();

        mockGamesCreateObjectStrategy.Setup(
            strategy => strategy.Invoke()
        ).Returns(mockTorpedo.Object).Verifiable();

        mockGamesCreateTorpedoStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockInitialValues.Object).Verifiable();

        mockSpaceBattleStartMoveStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(new object()).Verifiable();

        mockGamesIdGetNewStrategy.Setup(
            strategy => strategy.Invoke()
        ).Throws<InvalidOperationException>().Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Object",
            (object[] args) => mockGamesCreateObjectStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Torpedo",
            (object[] args) => mockGamesCreateTorpedoStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.StartMove",
            (object[] args) => mockSpaceBattleStartMoveStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGamesIdGetNewStrategy.Object.Invoke(args)
        ).Execute();


        ShotCommand shotCommand = new(shotable.Object);


        Assert.Throws<InvalidOperationException>(() => shotCommand.Execute());


        mockGamesCreateObjectStrategy.Verify(strategy => strategy.Invoke(), Times.Exactly(1));

        mockGamesCreateTorpedoStrategy.Verify(strategy => strategy.Invoke(It.Is<IShotable>(
            factArgs => factArgs == shotable.Object)), Times.Exactly(1));

        mockSpaceBattleStartMoveStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockTorpedo.Object && factArgs[1] == mockInitialValues.Object)
        ), Times.Exactly(1));

        mockGamesIdGetNewStrategy.Verify(strategy => strategy.Invoke(), Times.Exactly(1));
    }

    [Fact]
    public void SuccessfulShotCommandTest()
    {
        Mock<IShotable> shotable = new();

        Mock<IStrategy> mockGamesCreateObjectStrategy = new();
        Mock<IUObject> mockTorpedo = new();

        Mock<IStrategy> mockGamesCreateTorpedoStrategy = new();
        Mock<Dictionary<string, object>> mockInitialValues = new();

        Mock<IStrategy> mockSpaceBattleStartMoveStrategy = new();

        Mock<IStrategy> mockGamesIdGetNewStrategy = new();
        string mockId = "id1";

        Mock<IStrategy> mockSpaceBattleGetObjects = new();
        Mock<Dictionary<string, object>> mockDictionary = new();

        mockGamesCreateObjectStrategy.Setup(
            strategy => strategy.Invoke()
        ).Returns(mockTorpedo.Object).Verifiable();

        mockGamesCreateTorpedoStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(mockInitialValues.Object).Verifiable();

        mockSpaceBattleStartMoveStrategy.Setup(
            strategy => strategy.Invoke(It.IsAny<object>())
        ).Returns(new object()).Verifiable();

        mockGamesIdGetNewStrategy.Setup(
            strategy => strategy.Invoke()
        ).Returns(mockId).Verifiable();

        mockSpaceBattleGetObjects.Setup(
            strategy => strategy.Invoke()
        ).Returns(mockDictionary.Object).Verifiable();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Object",
            (object[] args) => mockGamesCreateObjectStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Create.Torpedo",
            (object[] args) => mockGamesCreateTorpedoStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.StartMove",
            (object[] args) => mockSpaceBattleStartMoveStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Games.Id.GetNew",
            (object[] args) => mockGamesIdGetNewStrategy.Object.Invoke(args)
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SpaceBattle.GetObjects",
            (object[] args) => mockSpaceBattleGetObjects.Object.Invoke(args)
        ).Execute();


        ShotCommand shotCommand = new(shotable.Object);


        try
        {
            shotCommand.Execute();
            Assert.True(mockDictionary.Object[mockId] == mockTorpedo.Object);
        }
        catch (Exception ex)
        {
            Assert.Fail("Test should be performed without exceptions: " + ex.Message);
        }


        mockGamesCreateObjectStrategy.Verify(strategy => strategy.Invoke(), Times.Exactly(1));

        mockGamesCreateTorpedoStrategy.Verify(strategy => strategy.Invoke(It.Is<IShotable>(
            factArgs => factArgs == shotable.Object)), Times.Exactly(1));

        mockSpaceBattleStartMoveStrategy.Verify(strategy => strategy.Invoke(It.Is<object[]>(
            factArgs => factArgs[0] == mockTorpedo.Object && factArgs[1] == mockInitialValues.Object)
        ), Times.Exactly(1));

        mockGamesIdGetNewStrategy.Verify(strategy => strategy.Invoke(), Times.Exactly(1));

        mockSpaceBattleGetObjects.Verify(strategy => strategy.Invoke(), Times.Exactly(1));
    }
}
