namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using Moq;

using ThreadDict = Dictionary<int, ServerThread>;
using QueueDict = Dictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;

public class RegisterIoCScope : IStrategy
{
    public object Execute(params object[] args)
    {
        return IoC.Resolve<ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>(
                "Scopes.New", IoC.Resolve<object>("Scopes.Root")
            )
        );
    }
}

public class RegisterGetThreadSenderDictCommand : IStrategy
{
    public object Execute(params object[] args)
    {
        var senderDict = new QueueDict();
        return IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.GetSenderDict",
            (object[] args) => senderDict
        );
    }
}

public class RegisterGetThreadDictCommand : IStrategy
{
    public object Execute(params object[] args)
    {
        var threadDict = new ThreadDict();
        return IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.GetDict",
            (object[] args) => threadDict
        );
    }
}

public class RegisterSendCommand : IStrategy
{
    public object Execute(params object[] args)
    {
        return IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SendCommand",
            (object[] args) =>
            {
                var threadId = (int)args[0];
                var cmd = (ICommand)args[1];

                var q = IoC.Resolve<QueueDict>("Thread.GetSenderDict")[threadId];

                return new SendCommand(q, cmd);
            }
        );
    }
}

public class RegisterServerThreadCreateAndStartCommand : IStrategy
{
    public object Execute(params object[] args)
    {
        return IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.Create&Start",
            (object[] args) =>
            {
                return new ActionCommand(() =>
                {
                    var threadId = (int)args[0];

                    Action action = () => { };
                    if (args.Length > 1)
                        action = (Action)args[1];
                    var actionCommand = new ActionCommand(action);

                    var q = new BlockingCollection<ICommand>(100) { actionCommand };
                    var serverThread = new ServerThread(q);

                    IoC.Resolve<ThreadDict>("Thread.GetDict").TryAdd(threadId, serverThread);
                    IoC.Resolve<QueueDict>("Thread.GetSenderDict").TryAdd(threadId, q);

                    serverThread.Start();
                });
            }
        );
    }
}

public class RegisterHardStopCommand : IStrategy
{
    public object Execute(params object[] args)
    {
        return IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.HardStop",
            (object[] args) =>
            {
                var serverThread = IoC.Resolve<ThreadDict>("Thread.GetDict")[(int)args[0]];
                var cmdList = new List<ICommand> { new HardStopCommand(serverThread) };

                if (args.Length > 1)
                    cmdList.Add(new ActionCommand((Action)args[1]));

                return new MacroCommand(cmdList);
            }
        );
    }
}

public class RegisterSoftStopCommand : IStrategy
{
    public object Execute(params object[] args)
    {
        return IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.SoftStop",
            (object[] args) =>
            {
                var serverThread = IoC.Resolve<ThreadDict>("Thread.GetDict")[(int)args[0]];

                Action? action = null;
                if (args.Length > 1)
                    action = (Action)args[1];

                return new SoftStopCommand(serverThread, action);
            }
        );
    }
}

public class RegisterExceptionHandlerCommand : IStrategy
{
    public object Execute(params object[] args)
    {
        var exceptionHandler = new Mock<ICommand>();
        return IoC.Resolve<ICommand>(
            "IoC.Register",
            "Exception.Handler",
            (object[] args) => exceptionHandler.Object
        );
    }
}
