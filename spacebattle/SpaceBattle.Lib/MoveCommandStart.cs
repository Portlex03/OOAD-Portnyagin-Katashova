namespace SpaceBattle.Lib;
using Hwdtech;

public class MoveCommandStart : ICommand
{
    private readonly IMoveCommandStartable _order;

    public MoveCommandStart(IMoveCommandStartable order) => _order = order;

    public void Execute()
    {
        _order.InitialValues.ToList().ForEach(a => IoC.Resolve<ICommand>("InitialValues.Set",_order.Target,a.Key,a.Value).Execute());

        var cmd = IoC.Resolve<ICommand>("Operations.Movement", _order.Target);

        IoC.Resolve<ICommand>("InitialValues.Set", _order.Target, "Operations.Movement", cmd).Execute();

        IoC.Resolve<IQueue>("Game.Queue").Put(cmd);
    }
}
