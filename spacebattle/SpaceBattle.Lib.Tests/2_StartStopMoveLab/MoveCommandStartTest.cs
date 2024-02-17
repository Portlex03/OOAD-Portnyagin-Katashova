namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class MoveCommandStartTest
{
    private readonly Mock<IQueue> q;
    public MoveCommandStartTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>(
            "Scopes.Current.Set", 
            IoC.Resolve<object>("Scopes.New", 
            IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var movement = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register","Operations.Movement",
            (object[] args) => movement.Object
        ).Execute();

        var setInitialValuesCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register","InitialValues.Set",
            (object[] args) => setInitialValuesCommand.Object 
        ).Execute();

        q = new Mock<IQueue>();
        IoC.Resolve<ICommand>(
            "IoC.Register","Game.Queue", 
            (object[] args) => q.Object
        ).Execute();
    }

    [Fact]
    public void MoveCommandStartRegistersInitialValuesAndPutsMoveCommandInQueue()
    {
        var uobject = new Mock<IUObject>();

        var startable = new Mock<IMoveCommandStartable>();
        startable.Setup(s => s.Target).Returns(uobject.Object);
        startable.Setup(s => s.InitialValues).Returns(
            new Dictionary<string, object>() {{"Velocity",new Vector(1,1)}}
        );

        var moveCommandStart = new MoveCommandStart(startable.Object);

        moveCommandStart.Execute();

        startable.Verify(s => s.InitialValues, Times.Once());
        q.Verify(q => q.Put(It.IsAny<ICommand>()), Times.Once());
    }

    [Fact]
    public void ImpossibleToDetermineUObject()
    {
        var uobject = new Mock<IUObject>();

        var startable = new Mock<IMoveCommandStartable>();
        startable.Setup(s => s.Target).Throws(() => new Exception()).Verifiable();
        startable.Setup(s => s.InitialValues).Returns(
            new Dictionary<string, object>() {{"Velocity",new Vector(1,1)}}
        );

        var moveCommandStart = new MoveCommandStart(startable.Object);

        Assert.Throws<Exception>(moveCommandStart.Execute);
    }

    [Fact]
    public void ImpossibleToDetermineInitialValuesOfUObject()
    {
        var uobject = new Mock<IUObject>();

        var startable = new Mock<IMoveCommandStartable>();
        startable.Setup(s => s.Target).Returns(uobject.Object);
        startable.Setup(s => s.InitialValues).Throws(() => new Exception()).Verifiable();

        var moveCommandStart = new MoveCommandStart(startable.Object);

        Assert.Throws<Exception>(moveCommandStart.Execute);
    }
}
