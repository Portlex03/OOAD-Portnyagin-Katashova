namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class LongOperationTest
{
    public LongOperationTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
            IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        var repeatCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Command.Repeat",
            (object[] args) => repeatCommand.Object
        ).Execute();

        var injectCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Command.Inject",
            (object[] args) => injectCommand.Object
        ).Execute();

        var macroCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "MacroCommand.Create",
            (object[] args) => macroCommand.Object
        ).Execute();
    }

    [Fact]
    public void LongOperationCreatesWithoutErrors()
    {
        string cmdName = "NameOfDependence";
        var target = new Mock<IUObject>();

        var longOperation = new LongOperation(cmdName, target.Object);
        Assert.NotNull(longOperation.Invoke());
    }
}
