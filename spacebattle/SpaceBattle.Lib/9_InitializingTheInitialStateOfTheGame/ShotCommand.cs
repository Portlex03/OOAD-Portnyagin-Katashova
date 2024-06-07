namespace SpaceBattle.Lib;

using Hwdtech;

public class ShotCommand : ICommand
{
    private readonly IShotable shotable;
    public ShotCommand(IShotable shotable)
    {
        this.shotable = shotable;
    }

    public void Execute()
    {
        IUObject torpedo = IoC.Resolve<IUObject>("Games.Create.Object");
        Dictionary<string, object> InitialValues = IoC.Resolve<Dictionary<string, object>>("Games.Create.Torpedo", shotable);
        IoC.Resolve<object>("SpaceBattle.StartMove", torpedo, InitialValues);
        string id = IoC.Resolve<string>("Games.Id.GetNew");
        IoC.Resolve<Dictionary<string, object>>("SpaceBattle.GetObjects")[id] = torpedo;
    }
}
