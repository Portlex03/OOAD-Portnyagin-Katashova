namespace SpaceBattle.Lib;

public interface IStrategy
{
    public object Invoke(params object[] args);
}
