namespace SpaceBattle.Lib;
using Hwdtech;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _t;

    public HardStopCommand(ServerThread t) => _t = t;

    public void Execute()
    {
        if (_t.Equals(Thread.CurrentThread))
            _t.Stop();
        else
            throw new Exception();
    }
}
