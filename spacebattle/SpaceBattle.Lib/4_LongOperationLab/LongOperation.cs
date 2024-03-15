namespace SpaceBattle.Lib;
using Hwdtech;

public class LongOperation : IStrategy
{
    private readonly string _cmdName;
    private readonly IUObject _target;

    public LongOperation(string cmdName, IUObject target)
    {
        _cmdName = cmdName;
        _target = target;
    }

    public object RunStrategy(params object[] args)
    {
        var macroCommand = IoC.Resolve<ICommand>(
            "MacroCommand.Create", _cmdName, _target);

        var repeatCommand = IoC.Resolve<ICommand>(
            "Command.Repeat", macroCommand);

        var injectCommand = IoC.Resolve<ICommand>(
            "Command.Inject", repeatCommand);

        return injectCommand;
    }
}
