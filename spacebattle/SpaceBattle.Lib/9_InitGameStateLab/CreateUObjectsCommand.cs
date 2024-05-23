namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateUObjectsCommand : IStrategy
{
    public object Invoke(params object[] args)
    {
        return Enumerable.Range(0, (int)args[0]).ToDictionary(
            key => Guid.NewGuid(),
            value => IoC.Resolve<IUObject>("CreateSingleUObject")
            );
    }
}
