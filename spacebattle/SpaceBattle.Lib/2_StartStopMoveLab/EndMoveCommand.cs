namespace SpaceBattle.Lib;
using Hwdtech;

public class EndMoveCommand : ICommand
{
    private readonly IMoveCommandEndable _endable;
    public EndMoveCommand(IMoveCommandEndable endable) => _endable = endable;

    public void Execute()
    {
        _endable.RequiredValues.ToList().ForEach(key =>
            IoC.Resolve<ICommand>(
                "Game.UObject.DeleteProperty",
                _endable.Target,
                key.Key
            ).Execute()
        );

        IoC.Resolve<IInjectable>(
            "Game.UObject.GetProperty",
            _endable.Target,
            "Operations.Movement"
        ).Inject(IoC.Resolve<ICommand>("Game.Command.EmptyCommand"));
    }
}
