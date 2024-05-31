namespace SpaceBattle.Lib.Test;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class ArrangeUObjectsTests
{
    public ArrangeUObjectsTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        var createSingleUObjectStrategy = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "CreateSingleUObject",
            (object[] args) => createSingleUObjectStrategy.Object.Invoke(args)
        ).Execute();

        var createUObjectsCmd = new CreateUObjectsCommand();
        IoC.Resolve<ICommand>(
            "IoC.Register", "CreateEmptyUObjects",
            (object[] args) => createUObjectsCmd.Invoke((int)args[0])
        ).Execute();
    }

    [Fact]
    public void Successful_Placing_UObjects_On_Game_Map()
    {
        var setUObjectPropertyCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObject.SetProperty",
            (object[] args) => setUObjectPropertyCmd.Object
        ).Execute();

        var uObjectsCount = 10;

        var uObjectsCollection = IoC.Resolve<Dictionary<Guid, IUObject>>("CreateEmptyUObjects", uObjectsCount);

        var uObjectPositions = Enumerable.Range(0, uObjectsCount).Select(index => new Vector(0, index)).ToList();

        var positionsEnumerator = new PositionsEnumerator(uObjectPositions);
        IoC.Resolve<ICommand>(
            "IoC.Register", "PositionsEnumerator",
            (object[] args) => positionsEnumerator
        ).Execute();

        var arrangeUObjectsCommand = new ArrangeUObjectsCommand(uObjectsCollection.Values);
        arrangeUObjectsCommand.Execute();

        setUObjectPropertyCmd.Verify(c => c.Execute(), Times.Exactly(uObjectsCount));
    }

    [Fact]
    public void Impossible_To_Set_Property_To_UObject()
    {
        var setUObjectPropertyCmd = new Mock<ICommand>();
        setUObjectPropertyCmd.Setup(c => c.Execute()).Throws<Exception>().Verifiable();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObject.SetProperty",
            (object[] args) => setUObjectPropertyCmd.Object
        ).Execute();

        var uObjectsCount = 10;

        var uObjectsCollection = IoC.Resolve<Dictionary<Guid, IUObject>>("CreateEmptyUObjects", uObjectsCount);

        var uObjectPositions = Enumerable.Range(0, uObjectsCount).Select(index => new Vector(0, index)).ToList();

        var positionsEnumerator = new PositionsEnumerator(uObjectPositions);
        IoC.Resolve<ICommand>(
            "IoC.Register", "PositionsEnumerator",
            (object[] args) => positionsEnumerator
        ).Execute();

        var arrangeUObjectsCommand = new ArrangeUObjectsCommand(uObjectsCollection.Values);

        Assert.Throws<Exception>(arrangeUObjectsCommand.Execute);
        setUObjectPropertyCmd.Verify();
    }

    [Fact]
    public void Impossible_To_Get_Position_Enumerator()
    {
        var setUObjectPropertyCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObject.SetProperty",
            (object[] args) => setUObjectPropertyCmd.Object
        ).Execute();

        var uObjectsCount = 10;

        var uObjectsCollection = IoC.Resolve<Dictionary<Guid, IUObject>>("CreateEmptyUObjects", uObjectsCount);

        var uObjectPositions = Enumerable.Range(0, uObjectsCount).Select(index => new Vector(0, index)).ToList();

        var getPositionsEnumeratorStrategy = new Mock<IStrategy>();
        getPositionsEnumeratorStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Throws<Exception>().Verifiable();
        IoC.Resolve<ICommand>(
            "IoC.Register", "PositionsEnumerator",
            (object[] args) => getPositionsEnumeratorStrategy.Object.Invoke(args)
        ).Execute();

        var arrangeUObjectsCommand = new ArrangeUObjectsCommand(uObjectsCollection.Values);

        Assert.Throws<Exception>(arrangeUObjectsCommand.Execute);
        getPositionsEnumeratorStrategy.Verify();
    }
}
