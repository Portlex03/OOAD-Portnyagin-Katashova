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

        // стратегия установки нового скопа
        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();

        // стратегия создания и запуска сервера
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Start",
            (object[] args) => new StartServerCommand((int)args[0])
        ).Execute();

        // словарь с потоками
        var threadDict = new Dictionary<int, object>();

        // стратегия создания и запуска потока сервера
        _createAndStartServerCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.Create&Start",
            (object[] args) =>
            {
                // получаем id потока
                var threadId = (int)args[0];

                // регистрируем его в словарь
                threadDict.TryAdd(threadId, $"ServerThread_№{threadId}");

                // возвращаем команду, которая создала и зарегистрировала сервер
                return _createAndStartServerCommand.Object;
            }
        ).Execute();

        // стратегия получения словаря с потоками
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.GetThreadDict",
            (object[] args) => threadDict
        ).Execute();

        // стратегия отправки команды по id потока
        _sendToThreadCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.SendCommand",
            (object[] args) => _sendToThreadCommand.Object
        ).Execute();

        // стратегия остановки потока по id
        var softStopCommand = new Mock<ICommand>();
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Thread.SoftStop",
            (object[] args) => softStopCommand.Object
        ).Execute();

        // стратегия остановки всего сервера
        IoC.Resolve<ICommand>(
            "IoC.Register", "Server.Stop",
            (object[] args) => new StopServerCommand()
        ).Execute();

        // стратегия получения имени лог файла
        var logFileName = "Exception.Logging";
        IoC.Resolve<ICommand>(
            "IoC.Register", "Exception.GetLogFileName",
            (object[] args) => logFileName
        ).Execute();
    }

    [Fact]
    public void SuccessfulStartingServerTest()
    {
        // кол-во потоков
        int threadCount = 10;

        // старт сервера с кол-вом потоков threadCount
        IoC.Resolve<ICommand>("Server.Start", threadCount).Execute();

        // проверка на то, что команда запуска потока
        // сервера исполнилась ровно threadCount раз
        _createAndStartServerCommand.Verify(cmd => cmd.Execute(), Times.Exactly(threadCount));

        // получаем словарь с потоками
        var threadDict = IoC.Resolve<Dictionary<int, object>>("Server.GetThreadDict");

        // проверка на то, что в словаре с потоками 
        // находится столько потоков, сколько мы создали
        Assert.True(threadDict.Count == threadCount);

        // очистка словаря с потоками
        threadDict.Clear();
    }

    [Fact]
    public void ServerStartsAsConsoleApplicationTest()
    {
        // кол-во потоков
        int threadCount = 3;

        // объект, который читает из консольного приложения
        var consoleInput = new StringReader("any");

        // объект, который записывает в консольное приложение
        var consoleOutput = new StringWriter();

        // Перенаправление стандартного ввода из консоли в объект
        Console.SetIn(consoleInput);

        // Перенаправление стандартного вывода из консоли в объект
        Console.SetOut(consoleOutput);

        // запуск консольного приложения
        ConsoleApplication.Main(new string[] { "3" });

        // присваиваем вывод консольного приложения объекту
        var output = consoleOutput.ToString();

        // проверка на то, что отобразились сообщения в консольном приложении
        Assert.Contains("Начало запуска сервера...", output);
        Assert.Contains($"Успешно запущен сервер с кол-вом потоков: {threadCount}", output);
        Assert.Contains("Для остановки сервера введите любую клавишу... ", output);
        Assert.Contains("Начало остановки сервера...", output);
        Assert.Contains("Cервер успешно завершил свою работу. Нажмите любую кнопку для выхода... ", output);
    }

    [Fact]
    public void SuccessfulSoftStopThreadsTest()
    {
        // кол-во потоков
        int threadCount = 10;

        // старт сервера с кол-вом потоков threadCount
        IoC.Resolve<ICommand>("Server.Start", threadCount).Execute();

        // получения словаря с потоками
        var threadDict = IoC.Resolve<Dictionary<int, object>>("Server.GetThreadDict");

        // получение команды, которая останавливает сервер
        var stopServerCommand = IoC.Resolve<ICommand>("Server.Stop");

        // остановка всего сервера
        stopServerCommand.Execute();

        // проверка на то, что стратегия отправки команды по id потока
        // отправила команду остановки ко всем потокам
        _sendToThreadCommand.Verify(cmd => cmd.Execute(), Times.Exactly(threadDict.Count));

        // очистка словаря с потоками
        threadDict.Clear();
    }

    [Fact]
    public void SuccessfulLoggingInFile()
    {
        // путь лог файла, куда запишется сообщение об ошибке
        var logFilePath = Path.GetTempFileName();

        // Команда, которая выдала ошибку
        var cmd = Mock.Of<ICommand>();

        // Ошибка команды
        var ex = new Exception();

        // инициализация обработчика
        var exHandler = new ExceptionHandlerCommand(cmd, ex);

        // запись в лог файл
        exHandler.Execute();

        // чтение всех строк лог файла
        var logFileLines = File.ReadAllLines(logFilePath);

        // проверка, что файл существует
        // Assert.True(File.Exists(logFileName));

        // сообщение об ошибке
        var exMessage = $"Exception in command {cmd.GetType().Name}. Message: {ex.Message}";

        // проверка на то, что сообщение об ошибке записалось в лог файл
        Assert.Contains(exMessage, logFileLines);
    }
}
