namespace SpaceBattle.Lib;

using System.Collections.Concurrent;
using Hwdtech;

public class SendCommand : ICommand
{
    private readonly BlockingCollection<ICommand> _q;
    private readonly ICommand _cmd;
    
    public SendCommand(BlockingCollection<ICommand> q, ICommand cmd)
    {
        _q = q;
        _cmd = cmd;
    }

    public void Execute() => _q.Add(_cmd);
}
