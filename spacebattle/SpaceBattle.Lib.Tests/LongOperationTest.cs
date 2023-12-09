namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;

public class LongOperationTest
{
    public LongOperationTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>(
            "Scopes.Current.Set", 
            IoC.Resolve<object>("Scopes.New", 
            IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }
    
    [Fact]
    public void LongOperationTest1()
    {

    }
}