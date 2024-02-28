namespace SpaceBattle.Lib;
using Hwdtech;


public class SoftStopCommand : ICommand
{
    ServerThread _t;
    Action _action;
    public SoftStopCommand(ServerThread t, Action action)
    {
        _t = t;
        _action = action;
    }

    public void Execute()
    {
        var oldBehaviour = _t.GetBehaviour();
        void newBehaviour()
        {
            if (_t.QueueIsEmpty)
            {
                _t.Stop();
                _action();
            }
            else
            {
                oldBehaviour();
            }
        }
        _t.UpdateBehaviour(newBehaviour);
    }
}
