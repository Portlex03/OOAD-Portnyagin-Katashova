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
        var uObjectsDict = new Dictionary<Guid, IUObject>();

        var getUObjectsDictStrategy = new Mock<IStrategy>();
        getUObjectsDictStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Returns(uObjectsDict);
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsDict",
            (object[] args) => getUObjectsDictStrategy.Object.Invoke(args)
        ).Execute();

        var createUObjectsStrategy = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "CreateUObject",
            (object[] args) => createUObjectsStrategy.Object.Invoke(args)
        ).Execute();

        var uObjectsCount = 10;

        var createUObjectsCmd = new CreateUObjectsCommand(uObjectsCount);
        createUObjectsCmd.Execute();

        createUObjectsStrategy.Verify(s => s.Invoke(), Times.Exactly(uObjectsCount));
        Assert.True(uObjectsDict.Count == uObjectsCount);
    }

    [Fact]
    public void Impossible_To_Get_UObjects_Map()
    {
        var uObjectsDict = new Dictionary<Guid, IUObject>();

        var getUObjectsDictStrategy = new Mock<IStrategy>();
        getUObjectsDictStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Throws<Exception>().Verifiable();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsDict",
            (object[] args) => getUObjectsDictStrategy.Object.Invoke(args)
        ).Execute();

        var createUObjectsStrategy = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "CreateUObject",
            (object[] args) => createUObjectsStrategy.Object.Invoke(args)
        ).Execute();

        var createUObjectsCmd = new CreateUObjectsCommand(10);

        Assert.Throws<Exception>(createUObjectsCmd.Execute);
        getUObjectsDictStrategy.Verify();
        createUObjectsStrategy.Verify(s => s.Invoke(), Times.Never);
    }

    [Fact]
    public void Impossible_To_Create_UObject()
    {
        var uObjectsDict = new Dictionary<Guid, IUObject>();

        var getUObjectsDictStrategy = new Mock<IStrategy>();
        getUObjectsDictStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Returns(uObjectsDict);
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsDict",
            (object[] args) => getUObjectsDictStrategy.Object.Invoke(args)
        ).Execute();

        var createUObjectsStrategy = new Mock<IStrategy>();
        createUObjectsStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Throws<Exception>().Verifiable();
        IoC.Resolve<ICommand>(
            "IoC.Register", "CreateUObject",
            (object[] args) => createUObjectsStrategy.Object.Invoke(args)
        ).Execute();

        var createUObjectsCmd = new CreateUObjectsCommand(10);

        Assert.Throws<Exception>(createUObjectsCmd.Execute);
        createUObjectsStrategy.Verify();
    }
}
