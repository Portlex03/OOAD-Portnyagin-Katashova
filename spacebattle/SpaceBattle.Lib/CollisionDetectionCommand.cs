namespace SpaceBattle.Lib;
using Dict = IDictionary<int, object>;
using Hwdtech;

public class CollisionDetectionCommand : ICommand
{
    IUObject obj1, obj2;

    public CollisionDetectionCommand(IUObject objFirst, IUObject objSecond)
    {
        obj1 = objFirst;
        obj2 = objSecond;
    }

    public void Execute()
    {
        var positionFirst = IoC.Resolve<List<int>>("Game.UObject.GetProperty", obj1, "Position");
        var positionSecond = IoC.Resolve<List<int>>("Game.UObject.GetProperty", obj2, "Position");
        var velocityFirst = IoC.Resolve<List<int>>("Game.UObject.GetProperty", obj1, "Velocity");
        var velocitySecond = IoC.Resolve<List<int>>("Game.UObject.GetProperty", obj2, "Velocity");

        var attribute_pos = positionFirst.Zip(positionSecond, (first, second) => first - second).ToList();
        var attribute_vel = velocityFirst.Zip(velocitySecond, (first, second) => first - second).ToList();

        var attributes = attribute_pos.Concat(attribute_vel).ToList();

        var collisionTree = IoC.Resolve<Dict>("Game.CollisionTree");

        attributes.ForEach(attribute => collisionTree = (Dict)collisionTree[attribute]);

        if (collisionTree.Keys.First() != 0)
        {
            IoC.Resolve<ICommand>("Game.Event.Collision", obj1, obj2).Execute();
        }
    }
}
