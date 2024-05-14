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
    }

    [Fact]
    public void Successful_Placing_UObjects_On_Game_Map()
    {
        var uObjectsCount = 10;

        var uObjectsDict = Enumerable.Repeat(new Mock<IUObject>().Object, uObjectsCount).ToDictionary(id => Guid.NewGuid());

        var getUObjectsDictStrategy = new Mock<IStrategy>();
        getUObjectsDictStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Returns(uObjectsDict);
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsDict",
            (object[] args) => getUObjectsDictStrategy.Object.Invoke(args)
        ).Execute();

        var setUObjectPropertyCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObject.SetProperty",
            (object[] args) => setUObjectPropertyCmd.Object
        ).Execute();

        var getUObjectsPositionsStrategy = new Mock<IStrategy>();
        getUObjectsPositionsStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Returns(Enumerable.Range(0, uObjectsCount).Select(index => new Vector(0, index)).ToList());
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsPositions",
            (object[] args) => getUObjectsPositionsStrategy.Object.Invoke(args)
        ).Execute();

        var arrangeUObjectsCommand = new ArrangeUObjectsCommand(new PositionsEnumerator());
        arrangeUObjectsCommand.Execute();

        getUObjectsPositionsStrategy.Verify(s => s.Invoke(), Times.Once);
        setUObjectPropertyCmd.Verify(c => c.Execute(), Times.Exactly(uObjectsCount));
    }

    [Fact]
    public void Impossible_To_Get_UObjects_Map()
    {
        var uObjectsCount = 10;

        var uObjectsDict = Enumerable.Repeat(new Mock<IUObject>().Object, uObjectsCount).ToDictionary(id => Guid.NewGuid());

        var getUObjectsDictStrategy = new Mock<IStrategy>();
        getUObjectsDictStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Throws<Exception>().Verifiable();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsDict",
            (object[] args) => getUObjectsDictStrategy.Object.Invoke(args)
        ).Execute();

        var setUObjectPropertyCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObject.SetProperty",
            (object[] args) => setUObjectPropertyCmd.Object
        ).Execute();

        var getUObjectsPositionsStrategy = new Mock<IStrategy>();
        getUObjectsPositionsStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Returns(Enumerable.Range(0, uObjectsCount).Select(index => new Vector(0, index)).ToList());
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsPositions",
            (object[] args) => getUObjectsPositionsStrategy.Object.Invoke(args)
        ).Execute();

        var arrangeUObjectsCommand = new ArrangeUObjectsCommand(new PositionsEnumerator());

        Assert.Throws<Exception>(arrangeUObjectsCommand.Execute);
        getUObjectsPositionsStrategy.Verify(s => s.Invoke(), Times.Once);
        setUObjectPropertyCmd.Verify(c => c.Execute(), Times.Never);
    }

    [Fact]
    public void Impossible_To_Set_UObject_Property()
    {
        var uObjectsCount = 10;

        var uObjectsDict = Enumerable.Repeat(new Mock<IUObject>().Object, uObjectsCount).ToDictionary(id => Guid.NewGuid());

        var getUObjectsDictStrategy = new Mock<IStrategy>();
        getUObjectsDictStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Returns(uObjectsDict);
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsDict",
            (object[] args) => getUObjectsDictStrategy.Object.Invoke(args)
        ).Execute();

        var setUObjectPropertyCmd = new Mock<ICommand>();
        setUObjectPropertyCmd.Setup(c => c.Execute()).Throws<Exception>().Verifiable();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObject.SetProperty",
            (object[] args) => setUObjectPropertyCmd.Object
        ).Execute();

        var getUObjectsPositionsStrategy = new Mock<IStrategy>();
        getUObjectsPositionsStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Returns(Enumerable.Range(0, uObjectsCount).Select(index => new Vector(0, index)).ToList());
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsPositions",
            (object[] args) => getUObjectsPositionsStrategy.Object.Invoke(args)
        ).Execute();

        var positionsEnumerator = new PositionsEnumerator();

        var arrangeUObjectsCommand = new ArrangeUObjectsCommand(positionsEnumerator);

        Assert.Throws<Exception>(arrangeUObjectsCommand.Execute);
        Assert.Throws<NotImplementedException>(positionsEnumerator.Dispose);
        getUObjectsPositionsStrategy.Verify(s => s.Invoke(), Times.Once);
        setUObjectPropertyCmd.Verify();
    }

    [Fact]
    public void Impossible_To_Get_UObjects_Positions()
    {
        var uObjectsCount = 10;

        var uObjectsDict = Enumerable.Repeat(new Mock<IUObject>().Object, uObjectsCount).ToDictionary(id => Guid.NewGuid());

        var getUObjectsDictStrategy = new Mock<IStrategy>();
        getUObjectsDictStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Returns(uObjectsDict);
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsDict",
            (object[] args) => getUObjectsDictStrategy.Object.Invoke(args)
        ).Execute();

        var setUObjectPropertyCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObject.SetProperty",
            (object[] args) => setUObjectPropertyCmd.Object
        ).Execute();

        var getUObjectsPositionsStrategy = new Mock<IStrategy>();
        getUObjectsPositionsStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Throws<Exception>().Verifiable();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObjectsPositions",
            (object[] args) => getUObjectsPositionsStrategy.Object.Invoke(args)
        ).Execute();

        var arrangeUObjectsCommand = new ActionCommand(() =>
            {
                var positionsEnumerator = new PositionsEnumerator();
                new ArrangeUObjectsCommand(positionsEnumerator).Execute();
            }
        );

        Assert.Throws<Exception>(arrangeUObjectsCommand.Execute);
        getUObjectsPositionsStrategy.Verify();
    }
}
