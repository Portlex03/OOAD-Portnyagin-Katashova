namespace SpaceBattle.Lib;
using Moq;
using Hwdtech;
using Hwdtech.Ioc;
public class CreateOperationStrategyTest
{
    public CreateOperationStrategyTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }
    [Fact]
    public void SuccessfullCreateOperationStrategy()
    {
        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(x => x.Execute()).Verifiable();
        string operationName = "Movement";
        var mockUObject = new Mock<IUObject>();
        IoC.Resolve<ICommand>("IoC.Register", "Game.Command." + operationName, (object[] args) => mockCommand.Object).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Game.Command.Macro", (object[] args) => mockCommand.Object).Execute();
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Game.Command.Inject",
            (object[] args) => new InjectCommand((ICommand)args[0])
        ).Execute();
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Game.Command.Repeat",
            (object[] args) => new RepeatCommand((ICommand)args[0])
        ).Execute();
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Game.Operation." + operationName,
            (object[] args) => new CreateOperationStrategy(operationName).Execute(args)
        ).Execute();
        IoC.Resolve<ICommand>("Game.Operation." + operationName, mockUObject.Object).Execute();
        mockCommand.VerifyAll();
    }
    [Fact]
    public void RepeatCommandTest()
    {
        var mockCommand = new Mock<ICommand>();
        var queue = new Queue<ICommand>();
        var repeatCommand = new RepeatCommand(mockCommand.Object);
        IoC.Resolve<ICommand>(
            "IoC.Register",
            "Game.Queue",
            (object[] args) => queue
        ).Execute();
        repeatCommand.Execute();
        Assert.Equal(mockCommand.Object, queue.Dequeue());
    }
}
