namespace SpaceBattle.Lib;
using Dict = Dictionary<int, object>;
using Hwdtech;

public class DecisionTreeCommand : ICommand
{
    private readonly string _path;
    public DecisionTreeCommand(string path) => _path = path;

    public void Execute()
    {
        var vectors = File.ReadAllLines(_path).Select(
            line => line.Split().Select(int.Parse).ToList()
        ).ToList();

        var tree = IoC.Resolve<Dict>("Game.DecisionTree");

        vectors.ForEach(vector =>
        {
            var layer = tree;
            vector.ForEach(num =>
            {
                layer.TryAdd(num, new Dict());
                layer = (Dict)layer[num];
            });
        });
    }
}
