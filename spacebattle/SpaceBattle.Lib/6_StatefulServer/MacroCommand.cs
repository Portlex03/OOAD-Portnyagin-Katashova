namespace SpaceBattle.Lib;
using Hwdtech;

public class MacroCommand : ICommand
{
    private readonly IEnumerable<ICommand> _cmdList;
    public MacroCommand(IEnumerable<ICommand> cmdList) => _cmdList = cmdList;
    public void Execute() => _cmdList.ToList().ForEach(cmd => cmd.Execute());
}