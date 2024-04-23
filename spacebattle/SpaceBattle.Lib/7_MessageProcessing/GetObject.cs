namespace SpaceBattle.Lib;
using Hwdtech;

public class GetObject : IStrategy
{
    public object Execute(params object[] args)
    {
        IoC.Resolve<Queue<IUObject>>("SpaceBattle.Queue").TryDequeue(out IUObject? obj); // вопрос исключает возможность null?
        return obj ?? throw new Exception();
    }
}