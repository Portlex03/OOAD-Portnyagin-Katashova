namespace SpaceBattle.Lib;
using Hwdtech;

public class SetFuelAdapter : IFuelBurnable
{
    private readonly IUObject _uobject;
    public SetFuelAdapter(IUObject uobject) => _uobject = uobject;

    public double FuelVolume
    {
        get => IoC.Resolve<double>("UObject.GetProperty", "FuelVolume", _uobject);

        set => IoC.Resolve<ICommand>("UObject.SetProperty", "FuelVolume", _uobject, value).Execute();
    }
}
