namespace SpaceBattle.Lib;
using Hwdtech;

public class QueueEnqueueCommand : ICommand
{
    private readonly Queue<ICommand> _queue;
    private readonly ICommand _command;
    public QueueEnqueueCommand(Queue<ICommand> queue, ICommand command)
    {
        _queue = queue;
        _command = command;
    }

    public void Execute()
    {
        _queue.Enqueue(_command);
    }
}
