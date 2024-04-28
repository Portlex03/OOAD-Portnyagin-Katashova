namespace SpaceBattle.Lib.Tests;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class TestStrategy
{
    private readonly GetInterpretateMessageCommand _getInterpretateMessageCommand = new();
    private readonly Mock<IMessage> _messageMock = new();
    public TestStrategy()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetInterpretateMessageCommand",
            (object[] args) => _getInterpretateMessageCommand.Execute(args)
        ).Execute();
    }

    [Fact]
    public void ExplicitEmptyArray()
    {
        object[] nullArgs = new object[0];

        var act = () => _getInterpretateMessageCommand.Execute(nullArgs);

        Assert.Throws<IndexOutOfRangeException>(act);
    }

    [Fact]
    public void ArrayTypeIsNotIMessage()
    {
        object[] args = new object[1] { 1 };

        var act = () => _getInterpretateMessageCommand.Execute(args);

        Assert.Throws<InvalidCastException>(act);
    }

    [Fact]
    public void SuccessfulExecutionOfStrategy()
    {
        object[] args = new object[1] { _messageMock.Object };

        var act = () => _getInterpretateMessageCommand.Execute(args);

        Assert.IsType<InterpretateCommand>(act());
    }

    [Fact]
    public void ImplicitEmptyArray()
    {
        Assert.Throws<IndexOutOfRangeException>(() => IoC.Resolve<ICommand>("GetInterpretateMessageCommand"));
    }
}
