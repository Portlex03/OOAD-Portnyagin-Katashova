namespace SpaceBattle.Lib.Tests;
using System.Collections.Concurrent;
using Hwdtech;

public class ServerThreadEqualsTest
{
    [Fact]
    public void Object_Is_Null_Test()
    {
        ServerThread? st = null;

        var q = new BlockingCollection<ICommand>(10);

        var st2 = new ServerThread(q);

        Assert.False(st2.Equals(st));
    }

    [Fact]
    public void Object_Is_Not_ServerThread_Type_Test()
    {
        int st = 1;

        var q = new BlockingCollection<ICommand>(10);

        var st2 = new ServerThread(q);

        Assert.False(st2.Equals(st));
    }
}