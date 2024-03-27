namespace SpaceBattle.Lib.Tests;
using Hwdtech.Ioc;
using Hwdtech;
using Moq;

public class ProcessStartingAndStoppingServerTest
{
    Mock<ICommand> _sendToThreadCommand;
    Mock<ICommand> _createAndStartServerCommand;
    public ProcessStartingAndStoppingServerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Start",
            (object[] args) => new StartServerCommand((int)args[0])
        ).Execute();

        var threadDict = new Dictionary<int, object>();

        _createAndStartServerCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.Create&Start",
            (object[] args) =>
            {
                var threadId = (int)args[0];

                threadDict.TryAdd(threadId, $"ServerThread_№{threadId}");

                return _createAndStartServerCommand.Object;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.GetThreadDict",
            (object[] args) => threadDict
        ).Execute();

        _sendToThreadCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.SendCommand",
            (object[] args) => _sendToThreadCommand.Object
        ).Execute();

        var softStopCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.SoftStop",
            (object[] args) =>
            {
                var actionCmd = new ActionCommand((Action)args[1]);
                actionCmd.Execute();
                return softStopCommand.Object;
            }
        ).Execute();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Stop",
            (object[] args) => new StopServerCommand()
        ).Execute();
    }

    [Fact]
    public void SuccessfulStartingServerTest()
    {
        int threadCount = 10;

        IoC.Resolve<ICommand>("Server.Start", threadCount).Execute();

        _createAndStartServerCommand.Verify(cmd => cmd.Execute(), Times.Exactly(threadCount));

        var threadDict = IoC.Resolve<Dictionary<int, object>>("Server.GetThreadDict");

        Assert.True(threadDict.Count == threadCount);

        threadDict.Clear();
    }

    [Fact]
    public void ServerStartsAsConsoleApplicationTest()
    {
        int threadCount = 3;

        var consoleInput = new StringReader("any");

        var consoleOutput = new StringWriter();

        Console.SetIn(consoleInput);

        Console.SetOut(consoleOutput);

        ConsoleApplication.Main(new string[] { "3" });

        var output = consoleOutput.ToString();

        Assert.Contains("Начало запуска сервера...", output);
        Assert.Contains($"Успешно запущен сервер с кол-вом потоков: {threadCount}", output);
        Assert.Contains("Для остановки сервера введите любую клавишу... ", output);
        Assert.Contains("Начало остановки сервера...", output);
        Assert.Contains("Cервер успешно завершил свою работу. Нажмите любую кнопку для выхода... ", output);
    }

    [Fact]
    public void SuccessfulSoftStopThreadsTest()
    {
        int threadCount = 10;

        IoC.Resolve<ICommand>("Server.Start", threadCount).Execute();

        var threadDict = IoC.Resolve<Dictionary<int, object>>("Server.GetThreadDict");

        var stopServerCommand = IoC.Resolve<ICommand>("Server.Stop");

        stopServerCommand.Execute();

        _sendToThreadCommand.Verify(cmd => cmd.Execute(), Times.Exactly(threadDict.Count));

        threadDict.Clear();
    }

    [Fact]
    public void SuccessfulLoggingInFile()
    {
        var logFilePath = Path.GetTempFileName();

        IoC.Resolve<ICommand>(
            "IoC.Register", "Game.Exception.GetTempFilePath",
            (object[] args) => logFilePath
        ).Execute();

        var cmd = Mock.Of<ICommand>();

        var ex = new Exception();

        var exHandler = new ExceptionHandlerCommand(cmd, ex);

        exHandler.Execute();

        var logFileLines = File.ReadAllLines(logFilePath);

        var exMessage = $"Exception in command {cmd.GetType().Name}. Message: {ex.Message}";

        Assert.Contains(exMessage, logFileLines);
    }
}
