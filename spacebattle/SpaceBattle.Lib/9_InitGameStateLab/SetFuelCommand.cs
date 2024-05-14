namespace SpaceBattle.Lib;
using Hwdtech;

public class SetFuelVolumeCommand : ICommand
{
    readonly double _fuelVolume;
    public SetFuelVolumeCommand(double fuelVolume) => _fuelVolume = fuelVolume;

    public void Execute()
    {
        var uObjectsDict = IoC.Resolve<Dictionary<Guid, IUObject>>("UObjectsDict");

        uObjectsDict.ToList().ForEach(idAndUObject => IoC.Resolve<ICommand>("UObject.SetProperty", idAndUObject.Value, "Fuel", _fuelVolume).Execute());
    }
}
