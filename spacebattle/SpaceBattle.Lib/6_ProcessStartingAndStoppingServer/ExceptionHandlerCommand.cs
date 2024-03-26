namespace SpaceBattle.Lib;
using Hwdtech;

public class ExceptionHandlerCommand : ICommand
{
    private readonly string _logFilePath;
    private readonly ICommand _cmd;
    private readonly Exception _ex;
    public ExceptionHandlerCommand(string logFilePath, ICommand cmd, Exception ex)
    {
        _logFilePath = logFilePath;
        _cmd = cmd;
        _ex = ex;
    }
    public void Execute()
    {
        var exMessage = $"Exception in command {_cmd.GetType().Name}. Message: {_ex.Message}";

        File.WriteAllText(_logFilePath, exMessage);
    }
}
