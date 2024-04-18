namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;
using Xunit.Sdk;

public class ExceptionHandlerTest
{
    readonly object _scope;
    readonly Mock<IStrategy> _getQuantumCmd = new();
    readonly Dictionary<object, object> _exHandlerDict = new();
    readonly Mock<ICommand> _exHandler = new();
    readonly Mock<ICommand> _defaultExHandler = new();
    
    public ExceptionHandlerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", _scope).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.ExecuteCommands",
            (object[] args) => new ExecuteCommandsInGame((Queue<ICommand>)args[0])
        ).Execute();
       
        _getQuantumCmd.Setup(cmd => cmd.Execute()).Returns(50);
        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Quantum",
            (object[] args) => _getQuantumCmd.Object.Execute()
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Queue.Dequeue",
            (object[] args) =>
            {
                var q = (Queue<ICommand>)args[0];
                if(q.TryDequeue(out ICommand? cmd))
                    return cmd;
                throw new InvalidOperationException();
            }
        ).Execute();
        
        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.Handler", 
            (object[] args) =>
            {
                var ex = (Exception)args[0];
                var cmd = (ICommand)args[1];
                
                if (_exHandlerDict.ContainsKey(cmd))
                    return _exHandler.Object;
                
                ex.Data["Command"] = cmd;
                // _defaultExHandler.Setup(x => x.Execute()).Throws(ex).Verifiable();
                return _defaultExHandler.Object;
            }
        ).Execute();
    }

    // [Fact]
    // public void Exception_Handle_Test()
    // {
    //     var cmd = new Mock<ICommand>();

    //     var exCmd = new Mock<ICommand>();
    //     exCmd.Setup(cmd => cmd.Execute()).Throws<Exception>();

    //     _exHandlerDict.Add(exCmd.Object, new Exception());

    //     var q = new Queue<ICommand>();
    //     q.Enqueue(cmd.Object);
    //     q.Enqueue(exCmd.Object);
    //     q.Enqueue(cmd.Object);

    //     var gameInCommand = new GameInCommand(IoC.Resolve<object>("Scopes.New", _scope), q);

    //     Assert.Throws<Exception>(gameInCommand.Execute);
    //     _exHandler.Verify(cmd => cmd.Execute(), Times.Once());
    // }

    [Fact]
    public void Default_Handler_Test()
    {
        var cmd = new Mock<ICommand>();

        var exCmd = new Mock<ICommand>();
        exCmd.Setup(cmd => cmd.Execute()).Throws<Exception>();

        var q = new Queue<ICommand>();
        q.Enqueue(cmd.Object);
        q.Enqueue(exCmd.Object);
        q.Enqueue(cmd.Object);

        var gameInCommand = new GameInCommand(IoC.Resolve<object>("Scopes.New", _scope), q);

        Assert.Throws<Exception>(gameInCommand.Execute);
        _exHandler.Verify(cmd => cmd.Execute(), Times.Once);
    }

    // [Fact]
    // public void CatchRegisteredException()
    // {
    //     RegisterDependencies();
    //     var mockRegisterExceptionHandler = new Mock<Lib.ICommand>();
    //     var mockCMD = new Mock<Lib.ICommand>();
    //     mockCMD.Setup(x => x.Execute()).Callback(() => { }).Verifiable();
    //     var mockDefault = new Mock<Lib.ICommand>();
    //     mockDefault.Setup(x => x.Execute()).Callback(() => { });
    //     var exceptionHandlerDict = new Dictionary<object, object>();
    //     mockRegisterExceptionHandler.Setup(x => x.Execute()).Callback(() =>
    //     {
    //         IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception Handler", (object[] param) =>
    //         {
    //             var command = (Lib.ICommand)param[1];
    //             if (exceptionHandlerDict.ContainsKey(command))
    //             {
    //                 return mockCMD.Object;
    //             }
    //             var exception = (Exception)param[0];
    //             exception.Data["command"] = command;
    //             mockDefault.Setup(x => x.Execute()).Throws(exception).Verifiable();
    //             return mockDefault.Object;
    //         }).Execute();
    //     });
    //     var mockCommandThrowException = new Mock<Lib.ICommand>();
    //     exceptionHandlerDict.Add((Lib.ICommand)mockCommandThrowException.Object, new Exception());
    //     mockCommandThrowException.Setup(command => command.Execute()).Throws<System.IO.IOException>();
    //     Queue<Lib.ICommand> queue = new Queue<Lib.ICommand>();
    //     queue.Enqueue(mockRegisterExceptionHandler.Object);
    //     queue.Enqueue(mockCommandThrowException.Object);
    //     var reciever = new Mock<IReciever>();
    //     reciever.Setup(reciever => reciever.Recieve()).Returns(() =>
    //     {
    //         return queue.Dequeue();
    //     });
    //     reciever.Setup(reciever => reciever.IsEmpty()).Returns(() => queue.Count == 0);

    //     double quant = 40;
    //     var mockStrategy = new Mock<IStrategy>();
    //     mockStrategy.Setup(s => s.RunStrategy()).Returns(quant);
    //     var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
    //     IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Quant", (object[] par) => mockStrategy.Object.RunStrategy(par)).Execute();
    //     var game = new GameCommand(reciever.Object, gameScope);
    //     game.Execute();
    //     mockCommandThrowException.Verify();
    //     mockDefault.VerifyNoOtherCalls();

    // }

    // [Fact]
    // public void CatchNonRegisteredException()
    // {
    //     RegisterDependencies();
    //     var mockRegisterExceptionHandler = new Mock<Lib.ICommand>();
    //     var mockCMD = new Mock<Lib.ICommand>();
    //     mockCMD.Setup(x => x.Execute()).Callback(() => { });
    //     var mockDefault = new Mock<Lib.ICommand>();

    //     var exceptionHandlerDict = new Dictionary<object, object>();
    //     mockRegisterExceptionHandler.Setup(x => x.Execute()).Callback(() =>
    //     {
    //         IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception Handler", (object[] param) =>
    //         {
    //             var command = (Lib.ICommand)param[1];
    //             if (exceptionHandlerDict.ContainsKey(command))
    //             {
    //                 return mockCMD.Object;
    //             }
    //             var exception = (Exception)param[0];
    //             exception.Data["command"] = command;
    //             mockDefault.Setup(x => x.Execute()).Throws(exception).Verifiable();
    //             return mockDefault.Object;
    //         }).Execute();
    //     });
    //     var mockCommandThrowException = new Mock<Lib.ICommand>();
    //     mockCommandThrowException.Setup(command => command.Execute()).Throws<System.IO.IOException>();
    //     Queue<Lib.ICommand> queue = new Queue<Lib.ICommand>();
    //     queue.Enqueue(mockRegisterExceptionHandler.Object);
    //     queue.Enqueue(mockCommandThrowException.Object);
    //     var reciever = new Mock<IReciever>();
    //     reciever.Setup(reciever => reciever.Recieve()).Returns(() =>
    //     {
    //         return queue.Dequeue();
    //     });
    //     reciever.Setup(reciever => reciever.IsEmpty()).Returns(() => queue.Count == 0);

    //     double quant = 40;
    //     var mockStrategy = new Mock<IStrategy>();
    //     mockStrategy.Setup(s => s.RunStrategy()).Returns(quant);
    //     var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
    //     IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Quant", (object[] par) => mockStrategy.Object.RunStrategy(par)).Execute();
    //     var game = new GameCommand(reciever.Object, gameScope);
    //     game.Execute();
    //     mockCommandThrowException.Verify();
    //     mockCMD.VerifyNoOtherCalls();
    // }
}