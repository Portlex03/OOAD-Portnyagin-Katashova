namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class ServerThreadTest
{
    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Thread.HardStop",
            (object[] args) => {
                return new ActionCommand(() => {
                    ((ICommand)args[0]).Execute();
                    new ActionCommand((Action)args[1]).Execute();
                });
            }
        ).Execute();

        var exceptionHandler = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.Handle",
            (object[] args) => exceptionHandler.Object
        ).Execute();
    }

    [Fact]
    public void HardStopMustStopServerThread()
    {
        var q = new BlockingCollection<ICommand>(100);
        var st = new ServerThread(q);

        var mre = new ManualResetEvent(false);
        var hs = new HardStopCommand(st);

        q.Add(new EmptyCommand());

        q.Add(new ActionCommand(() => { Thread.Sleep(1000); } ));

        q.Add(IoC.Resolve<ICommand>("Thread.HardStop", hs, () => { mre.Set(); }));

        q.Add(new EmptyCommand());

        st.Start();
        mre.WaitOne();

        Assert.Single(q);
        Assert.False(st.IsAlive);
    }

    [Fact]
    public void SoftStopMustStopServerThread()
    {
        var q = new BlockingCollection<ICommand>(100);
        var st = new ServerThread(q);

        var mre = new ManualResetEvent(false);
        var ss = new SoftStopCommand(st, () => { mre.Set(); });

        q.Add(new EmptyCommand());

        q.Add(new ActionCommand(() => { Thread.Sleep(1000); } ));

        q.Add(ss);

        q.Add(new ActionCommand(() => { Thread.Sleep(1000); } ));

        q.Add(new EmptyCommand());

        st.Start();
        mre.WaitOne();

        Assert.True(st.QueueIsEmpty);
        Assert.False(st.IsAlive);
    }

    [Fact]
    public void ServerThreadCanWorkWithExceptionCommands()
    {
        var q = new BlockingCollection<ICommand>(100);
        var st = new ServerThread(q);

        var mre = new ManualResetEvent(false);
        var hs = new HardStopCommand(st);

        q.Add(new ActionCommand(() => { Thread.Sleep(1000); } ));

        q.Add(new ActionCommand(() => { throw new Exception(); }));

        q.Add(IoC.Resolve<ICommand>("Thread.HardStop", hs, () => { mre.Set(); }));

        q.Add(new ActionCommand(() => { Thread.Sleep(1000); } ));

        q.Add(new EmptyCommand());

        st.Start();
        mre.WaitOne();

        Assert.True(q.Count == 2);
        Assert.False(st.IsAlive);
    }
}