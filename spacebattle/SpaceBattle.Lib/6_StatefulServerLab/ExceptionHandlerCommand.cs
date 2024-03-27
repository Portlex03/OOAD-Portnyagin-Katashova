namespace SpaceBattle.Lib;
using Hwdtech;

public class ExceptionHandlerCommand : ICommand
{
    private readonly ICommand _cmd;
    private readonly Exception _ex;
    public ExceptionHandlerCommand(ICommand cmd, Exception ex)
    {
        _cmd = cmd;
        _ex = ex;
    }
    public void Execute()
    {
        var logFilePath = IoC.Resolve<string>("Game.Exception.GetTempFilePath");

        var exMessage = $"Exception in command {_cmd.GetType().Name}. Message: {_ex.Message}";

        using StreamWriter writer = new(logFilePath, true);

        writer.WriteLine(exMessage);
    }
}
