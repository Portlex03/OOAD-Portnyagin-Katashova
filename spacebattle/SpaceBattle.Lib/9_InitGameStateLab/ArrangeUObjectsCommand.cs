namespace SpaceBattle.Lib;
using Hwdtech;

public class ArrangeUObjectsCommand : ICommand
{
    readonly IEnumerable<IUObject> _uObjectsCollection;

    public ArrangeUObjectsCommand(IEnumerable<IUObject> uObjectsCollection) => _uObjectsCollection = uObjectsCollection;

    public void Execute()
    {
        using IEnumerator<object> positionsEnumerator = IoC.Resolve<IEnumerator<object>>("PositionsEnumerator");
        positionsEnumerator.Reset();
        _uObjectsCollection.ToList().ForEach(
            uObject =>
            {
                IoC.Resolve<ICommand>("UObject.SetProperty", uObject, "Position", positionsEnumerator.Current).Execute();
                positionsEnumerator.MoveNext();
            }
        );
    }
}
