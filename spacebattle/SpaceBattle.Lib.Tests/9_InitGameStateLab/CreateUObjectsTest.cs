namespace SpaceBattle.Lib.Test;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;


public class CreateUObjectsTest
{
    public CreateUObjectsTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void Successful_Creating_Empty_UObjects()
    {
        var createUObjectsStrategy = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "CreateSingleUObject",
            (object[] args) => createUObjectsStrategy.Object.Invoke(args)
        ).Execute();

        var uObjectsCount = 6;

        var createUObjectsCmd = new CreateUObjectsCommand();

        var uObjectsCollection = (Dictionary<Guid, IUObject>)createUObjectsCmd.Invoke(uObjectsCount);

        createUObjectsStrategy.Verify(s => s.Invoke(), Times.Exactly(uObjectsCount));
    }

    [Fact]
    public void Impossible_To_Create_Single_UObject()
    {
        var createSingleUObjectStrategy = new Mock<IStrategy>();
        createSingleUObjectStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Throws<Exception>().Verifiable();
        IoC.Resolve<ICommand>(
            "IoC.Register", "CreateSingleUObject",
            (object[] args) => createSingleUObjectStrategy.Object.Invoke(args)
        ).Execute();

        var uObjectsCount = 6;

        var createUObjectsCmd = new ActionCommand(() => new CreateUObjectsCommand().Invoke(uObjectsCount));

        Assert.Throws<Exception>(createUObjectsCmd.Execute);
        createSingleUObjectStrategy.Verify();
    }
}
