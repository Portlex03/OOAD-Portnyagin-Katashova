namespace SpaceBattle.Lib;
using Hwdtech;

public class HardStopCommand : ICommand
{
    ServerThread _t;

    public HardStopCommand(ServerThread t) => _t = t;

    public void Execute() => _t.Stop();
}
