namespace SpaceBattle.Lib;
using Dict = IDictionary<int, object>;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class CollisionDetectionCommandTest
{
    public CollisionDetectionCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    [Fact]
    public void CollisionDetectionCommandCompleted()
    {
        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(x => x.Execute()).Verifiable();

        var mockDict = new Mock<Dict>();
        mockDict.SetupGet(x => x[It.IsAny<int>()]).Returns(mockDict.Object);
        mockDict.SetupGet(x => x.Keys).Returns(new List<int>{ 1 });

        IoC.Resolve<ICommand>("IoC.Register", "Game.UObject.GetProperty", (object[] args) => new List<int> {1, 1}).Execute();
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Game.Command.CheckCollision", 
            (object[] args) => new CollisionDetectionCommand((IUObject)args[0], (IUObject)args[1])
        ).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Game.CollisionTree", (object[] args) => mockDict.Object).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Game.Event.Collision", (object[] args) => mockCommand.Object).Execute(); 

        var mockUObject = new Mock<IUObject>();

        var checkCollisionCommand = IoC.Resolve<ICommand>("Game.Command.CheckCollision", mockUObject.Object, mockUObject.Object);

        checkCollisionCommand.Execute();

        mockCommand.VerifyAll();
    }

    [Fact]
    public void TryGetNewTreeThrowsException()
    {
        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(x => x.Execute());

        var mockDict = new Mock<Dict>();
        mockDict.SetupGet(x => x[It.IsAny<int>()]).Throws(new Exception()).Verifiable(); 

        IoC.Resolve<ICommand>("IoC.Register", "Game.UObject.GetProperty", (object[] args) => new List<int> {1, 1}).Execute();
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Game.Command.CheckCollision", 
            (object[] args) => new CollisionDetectionCommand((IUObject)args[0], (IUObject)args[1])
        ).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Game.CollisionTree", (object[] args) => mockDict.Object).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Game.Event.Collision", (object[] args) => mockCommand.Object).Execute(); 

        var mockUObject = new Mock<IUObject>();

        var checkCollisionCommand = IoC.Resolve<ICommand>("Game.Command.CheckCollision", mockUObject.Object, mockUObject.Object);

        Assert.Throws<Exception>(() => checkCollisionCommand.Execute());
        mockDict.VerifyAll();
    }
}
