namespace SpaceBattle.Lib;

using Hwdtech;

public class GetMessage : IStrategy
{
    private readonly IUObject message;
    public GetMessage(IUObject obj)
    {
        message = obj;
    }
    public object Execute(params object[] args)
    {
        IoC.Resolve<Dictionary<string, IUObject>>("SpaceBattle.GetMessage").TryGetValue((string)args[0], out IUObject? message); 

        IoC.Resolve<Queue<IUObject>>("SpaceBattle.Queue").TryDequeue(out message);

        return message ?? throw new Exception();
    }
}
