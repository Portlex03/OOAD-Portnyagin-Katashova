namespace SpaceBattle.Lib;
using Hwdtech;

public class SetFuelCommand : ICommand
{
    private readonly IFuelBurnable _uObject;
    private readonly double _fuelVolume;

    public SetFuelCommand(IFuelBurnable uObject, double fuelVolume)
    {
        _uObject = uObject;
        _fuelVolume = fuelVolume;
    }

    public void Execute() => _uObject.FuelVolume = _fuelVolume;
}
