namespace SpaceBattle.Lib;
using Hwdtech;

/* Удалить */
// public class MacroCommand : ICommand
// {
//     public List<ICommand> _cmds = new();

//     public MacroCommand(string nameOfDependency_returnsAtomaricCmdNames)
//     {
//         var cmdNames = IoC.Resolve<string[]>(nameOfDependency_returnsAtomaricCmdNames);
//         cmdNames.ToList().ForEach(cmd_name => {
//             _cmds.Add(IoC.Resolve<ICommand>(cmd_name));
//         });
//     }

//     public void Execute()
//     {
//         _cmds.ForEach(cmd => cmd.Execute());
//     }
// }

public interface IStrategy
{
    public object Execute(params object[] args);
}

public class LongOperation : IStrategy
{
    private readonly string _cmdName;
    private readonly IUObject _target;

    public LongOperation(string cmdName, IUObject target)
    {
        _cmdName = cmdName;
        _target = target;
    }

    public object Execute(params object[] args)
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
