namespace SpaceBattle.Lib;
using Hwdtech;

public class SetUObjectsFuelCommand : ICommand
{
    readonly IEnumerable<IUObject> _uObjectsCollection;
    public SetUObjectsFuelCommand(IEnumerable<IUObject> uObjectsCollection) => _uObjectsCollection = uObjectsCollection;

    public void Execute()
    {
        using IEnumerator<object> fuelEnumerator = IoC.Resolve<IEnumerator<object>>("FuelEnumerator");
        fuelEnumerator.Reset();
        _uObjectsCollection.ToList().ForEach(uObject =>
            {
                IoC.Resolve<ICommand>("SetFuelCommand", uObject, fuelEnumerator.Current).Execute();
                fuelEnumerator.MoveNext();
            }
        );
    }
}
