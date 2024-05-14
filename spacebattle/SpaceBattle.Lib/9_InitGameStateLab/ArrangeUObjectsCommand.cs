namespace SpaceBattle.Lib;
using Hwdtech;

public class ArrangeUObjectsCommand : ICommand
{
    readonly IEnumerator<object> _positionsEnumerator;

    public ArrangeUObjectsCommand(IEnumerator<object> positionsEnumerator) => _positionsEnumerator = positionsEnumerator;

    public void Execute()
    {
        var uObjectsDict = IoC.Resolve<Dictionary<Guid, IUObject>>("UObjectsDict");
        _positionsEnumerator.Reset();

        uObjectsDict.ToList().ForEach(
            idAndUObject =>
            {
                IoC.Resolve<ICommand>("UObject.SetProperty", idAndUObject.Value, "Position", _positionsEnumerator.Current).Execute();
                _positionsEnumerator.MoveNext();
            }
        );
    }
}
