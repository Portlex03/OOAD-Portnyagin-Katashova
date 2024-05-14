namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class SetFuelVolumeTests
{
    public SetFuelVolumeTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
    }

    [Fact]
    public void Successful_Setting_Fuel_Volume_To_UObjects()
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

        var setFuelVolumeCommand = new SetFuelVolumeCommand(100.0);
        setFuelVolumeCommand.Execute();

        setUObjectPropertyCmd.Verify(c => c.Execute(), Times.Exactly(uObjectsCount));
    }

    [Fact]
    public void Impossible_To_Get_UObjects_Dictionary()
    {
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

        var setFuelVolumeCommand = new SetFuelVolumeCommand(100.0);

        Assert.Throws<Exception>(setFuelVolumeCommand.Execute);
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

        var setFuelVolumeCommand = new SetFuelVolumeCommand(100.0);

        Assert.Throws<Exception>(setFuelVolumeCommand.Execute);
        setUObjectPropertyCmd.Verify();
    }
}
