namespace SpaceBattle.Lib.Tests;
using System.Collections.Concurrent;
using Hwdtech;

public class ServerThreadEqualsTest
{
    [Fact]
    public void Object_Is_Null_Test()
    {
        ServerThread? st1 = null;

        var q = new BlockingCollection<ICommand>(10);

        var st2 = new ServerThread(q);

        Assert.False(st2.Equals(st1));
    }

    [Fact]
    public void Object_Is_Not_ServerThread_Type_Test()
    {
        int st1 = 1;

        var q = new BlockingCollection<ICommand>(10);

        var st2 = new ServerThread(q);

        Assert.False(st2.Equals(st1));
    }

    [Fact]
    public void Two_different_objects_have_different_GetHashCodes()
    {
        var q = new BlockingCollection<ICommand>(10);

        var st1 = new ServerThread(q);

        var st2 = new ServerThread(q);

        Assert.False(st1.GetHashCode() == st2.GetHashCode());
    }
}