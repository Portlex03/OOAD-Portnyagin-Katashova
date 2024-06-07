namespace SpaceBattle.Lib;

public interface IShotable
{
    bool isShootable { get; }

    int projectileSpeed { get; }

    Vector projectileStartPoint { get; }

    Vector direction { get; }
}
