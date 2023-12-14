namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class EndMoveCommandTest
{
    public EndMoveCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Game.Command.EmptyCommand", (object[] args) => new EmptyCommand()).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Game.Command.EndMovement", (object[] args) => new EndMoveCommand((IMoveCommandEndable)args[0])).Execute();

        var mockCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>("IoC.Register", "Game.UObject.DeleteProperty", (object[] args) => mockCommand.Object).Execute();

        var mockInjectable = new Mock<IInjectable>();
        IoC.Resolve<ICommand>("IoC.Register", "Game.UObject.GetProperty", (object[] args) => mockInjectable.Object).Execute();
    }

    [Fact]
    public void EndMovementCommandCompleted()
    {
        var mockUObject = new Mock<IUObject>();

        var mockEndable = new Mock<IMoveCommandEndable>();

        mockEndable.Setup(end => end.Target).Returns(mockUObject.Object).Verifiable();
        mockEndable.Setup(end => end.RequiredValues).Returns(new Dictionary <string, object>() {{ "Velocity", new Vector(1,1) }}).Verifiable();

        var endMoveCommand = IoC.Resolve<ICommand>("Game.Command.EndMovement", mockEndable.Object);

        endMoveCommand.Execute();
        mockEndable.VerifyAll();
    }

    [Fact]
    public void InjectCommandTest()
    {
        var mockCommandFirst = new Mock<ICommand>();
        mockCommandFirst.Setup(end => end.Execute()).Verifiable();

        var mockCommandSecond = new Mock<ICommand>();
        mockCommandSecond.Setup(end => end.Execute()).Verifiable();

        var InjectCommand = new InjectCommand(mockCommandFirst.Object);

        InjectCommand.Execute();

        mockCommandFirst.Verify(end => end.Execute(), Times.Once);
        mockCommandSecond.Verify(end => end.Execute(), Times.Never);

        InjectCommand.Inject(mockCommandSecond.Object);

        InjectCommand.Execute();

        mockCommandFirst.Verify(end => end.Execute(), Times.Once);
        mockCommandSecond.Verify(end => end.Execute(), Times.Once);
    }

    [Fact]
    public void EmptyCommandTest()
    {
        var EmptyCommand = IoC.Resolve<ICommand>("Game.Command.EmptyCommand");

        EmptyCommand.Execute();

        Assert.NotNull(EmptyCommand);
    }

    [Fact]
    public void GettingExceptionTarget()
    {
        var mockUObject = new Mock<IUObject>();

        var mockEndable = new Mock<IMoveCommandEndable>();

        mockEndable.Setup(end => end.Target).Throws(new Exception()).Verifiable();
        mockEndable.Setup(end => end.RequiredValues).Returns(new Dictionary<string, object> {{ "Velocity", new Vector(1,1) }}).Verifiable();

        var endMoveCommand = IoC.Resolve<ICommand>("Game.Command.EndMovement", mockEndable.Object);

        Assert.Throws<Exception>(() => endMoveCommand.Execute());
        mockEndable.VerifyAll();
    }

    [Fact]
    public void GettingExceptionRequiredValues()
    {
        var mockUObject = new Mock<IUObject>();

        var mockEndable = new Mock<IMoveCommandEndable>();

        mockEndable.Setup(end => end.Target).Returns(mockUObject.Object);
        mockEndable.Setup(end => end.RequiredValues).Throws(new Exception());

        var endMoveCommand = IoC.Resolve<ICommand>("Game.Command.EndMovement", mockEndable.Object);

        Assert.Throws<Exception>(() => endMoveCommand.Execute());
        mockEndable.VerifyGet(end => end.Target, Times.Never());
        mockEndable.VerifyGet(end => end.RequiredValues, Times.Once());
    }
}
