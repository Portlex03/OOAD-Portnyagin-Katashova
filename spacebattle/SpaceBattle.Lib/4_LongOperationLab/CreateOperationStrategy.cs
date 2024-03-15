namespace SpaceBattle.Lib;
using Hwdtech;
public class CreateOperationStrategy : IStrategy
{
    private readonly string _name;

    public CreateOperationStrategy(string name) => _name = name;

    public object RunStrategy(params object[] args)
    {
        IUObject _obj = (IUObject)args[0];
        ICommand command = IoC.Resolve<ICommand>("Game.Command." + _name, _obj);
        List<ICommand> commandsList = new List<ICommand> { command };
        ICommand macroCommand = IoC.Resolve<ICommand>("Game.Command.Macro", commandsList);
        ICommand injectCommand = IoC.Resolve<ICommand>("Game.Command.Inject", macroCommand);
        ICommand repeatCommand = IoC.Resolve<ICommand>("Game.Command.Repeat", injectCommand);
        commandsList.Add(repeatCommand);
        return injectCommand;
    }
}
