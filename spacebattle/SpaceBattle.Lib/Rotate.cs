namespace SpaceBattle.Lib;

public interface IRotatable
{
    public int Angle { get; set; }
    public int AngularVelocity { get; }
    public float N{ get; }
}

public class RotateCommand : ICommand
{
    private readonly IRotatable rotatable;
    public RotateCommand(IRotatable rotatable)
    {
        this.rotatable = rotatable;
    }

    public void Execute()
    {
        // angle и angularVelocity переводятся из градусов в метки от 0 до n-1z 
        // считается остаток деления на n и переводится опять в градусы
        rotatable.Angle = (int)(rotatable.N / 360 * (rotatable.Angle + rotatable.AngularVelocity) % rotatable.N * 360 / rotatable.N);
    }
}
