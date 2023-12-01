namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class MoveCommandStartTest
{
    public MoveCommandStartTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();        
    }

    [Fact]
    public void MoveCommandStartRegisterInitialValuesAndPutMoveCommandInQueue()
    {
        var uobject = new Mock<IUObject>();

        var startable = new Mock<IMoveCommandStartable>();
        startable.Setup(m => m.Target).Returns(uobject.Object);
        startable.Setup(m => m.InitialValues).Returns(
            new Dictionary<string, object>() {{"Velocity",new Vector(1,1)}}
        );
        var moveCommandStart = new MoveCommandStart(startable.Object);

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

        var q = new Mock<IQueue>();
        IoC.Resolve<ICommand>(
            "IoC.Register","Game.Queue", 
            (object[] args) => q.Object
        ).Execute();

        moveCommandStart.Execute();

        startable.Verify(s => s.InitialValues, Times.Once());
        q.Verify(q => q.Put(It.IsAny<ICommand>()), Times.Once());
    }
}