namespace SpaceBattle.Lib;
using Hwdtech;

public class RemoveCommandInObject : ICommand
{
    private readonly IDictionary<string, IUObject> _objects;
    private readonly string _key;
    public RemoveCommandInObject(IDictionary<string, IUObject> objects, string key)
    {
        _objects = objects;
        _key = key;
    }

    public void Execute()
    {
        _objects.Remove(_key);
    }
}
