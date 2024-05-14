namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateUObjectsCommand : ICommand
{
    readonly int _uObjectsCount;
    public CreateUObjectsCommand(int uObjectsCount) => _uObjectsCount = uObjectsCount;

    public void Execute()
    {
        var uObjectsDict = IoC.Resolve<Dictionary<Guid, IUObject>>("UObjectsDict");

        Enumerable.Range(0, _uObjectsCount).ToList().ForEach(uObject => uObjectsDict.Add(Guid.NewGuid(), IoC.Resolve<IUObject>("CreateUObject")));
    }
}
