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

        var createSingleUObjectsStrategy = new Mock<IStrategy>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "CreateSingleUObject",
            (object[] args) => createSingleUObjectsStrategy.Object.Invoke(args)
        ).Execute();

        var createUObjectsCmd = new CreateUObjectsCommand();
        IoC.Resolve<ICommand>(
            "IoC.Register", "CreateEmptyUObjects",
            (object[] args) => createUObjectsCmd.Invoke((int)args[0])
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "SetFuelCommand",
            (object[] args) =>
            {
                var iFuelBurnableUObject = new SetFuelAdapter((IUObject)args[0]);
                return new SetFuelCommand(iFuelBurnableUObject, (double)args[1]);
            }
        ).Execute();
    }

    [Fact]
    public void Successful_Setting_Fuel_Volume_To_UObjects()
    {
        var setUObjectPropertyCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObject.SetProperty",
            (object[] args) => setUObjectPropertyCmd.Object
        ).Execute();

        var uObjectsCount = 6;

        var uObjectsCollection = IoC.Resolve<Dictionary<Guid, IUObject>>("CreateEmptyUObjects", uObjectsCount);

        var fuelVolumes = new List<double>() { 10, 20, 30, 40, 50, 60 };

        var fuelEnumerator = new FuelEnumerator(fuelVolumes);
        IoC.Resolve<ICommand>(
            "IoC.Register", "FuelEnumerator",
            (object[] args) => fuelEnumerator
        ).Execute();

        var setFuelCmd = new SetUObjectsFuelCommand(uObjectsCollection.Values);
        setFuelCmd.Execute();

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

        var uObjectsCount = 6;

        var uObjectsCollection = IoC.Resolve<Dictionary<Guid, IUObject>>("CreateEmptyUObjects", uObjectsCount);

        var fuelVolumes = new List<double>() { 10, 20, 30, 40, 50, 60 };

        var fuelEnumerator = new FuelEnumerator(fuelVolumes);
        IoC.Resolve<ICommand>(
            "IoC.Register", "FuelEnumerator",
            (object[] args) => fuelEnumerator
        ).Execute();

        var setFuelCmd = new SetUObjectsFuelCommand(uObjectsCollection.Values);
        Assert.Throws<Exception>(setFuelCmd.Execute);
        setUObjectPropertyCmd.Verify();
    }

    [Fact]
    public void Impossible_To_Get_Fuel_Enumerator()
    {
        var setUObjectPropertyCmd = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "UObject.SetProperty",
            (object[] args) => setUObjectPropertyCmd.Object
        ).Execute();

        var uObjectsCount = 6;

        var uObjectsCollection = IoC.Resolve<Dictionary<Guid, IUObject>>("CreateEmptyUObjects", uObjectsCount);

        var fuelVolumes = new List<double>() { 10, 20, 30, 40, 50, 60 };

        var getFuelEnumeratorStrategy = new Mock<IStrategy>();
        getFuelEnumeratorStrategy.Setup(s => s.Invoke(It.IsAny<object[]>())).Throws<Exception>().Verifiable();
        IoC.Resolve<ICommand>(
            "IoC.Register", "FuelEnumerator",
            (object[] args) => getFuelEnumeratorStrategy.Object.Invoke(args)
        ).Execute();

        var setFuelCmd = new SetUObjectsFuelCommand(uObjectsCollection.Values);

        Assert.Throws<Exception>(setFuelCmd.Execute);
        getFuelEnumeratorStrategy.Verify();
    }
}
