namespace SpaceBattle.Lib;

public interface IMovable
{
    public Vector Position { get; set; }
    public Vector Velocity { get; }
}

public class MoveCommand : ICommand
{
    private readonly IMovable movable;
    public MoveCommand(IMovable movable)
    {
        this.movable = movable;
    }
    public void Execute()
    {
        movable.Position += movable.Velocity;
    }
}
