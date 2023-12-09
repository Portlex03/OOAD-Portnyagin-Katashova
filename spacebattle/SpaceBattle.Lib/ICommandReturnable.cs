namespace SpaceBattle.Lib;

public interface ICommandReturnable
{
    object Execute(params object[] args);
}
