namespace SpaceBattle.Lib;
using Hwdtech;

public class LongOperation : ICommand
{
    private readonly string _cmdName;
    private readonly IUObject _target;

    public LongOperation(string cmdName, IUObject target)
    {
        _cmdName = cmdName;
        _target = target;
    }

    public void Execute()
    {
        var stringsList = IoC.Resolve<IEnumerable<string>>(
            "SetupStringOperation." + _cmdName
        );

        var commandsList = stringsList.Select(
            str => IoC.Resolve<ICommand>(str,_target)
        );

        var macroCommand = IoC.Resolve<ICommand>(
            "MacroCommand.Create", commandsList
        );
    }
}
