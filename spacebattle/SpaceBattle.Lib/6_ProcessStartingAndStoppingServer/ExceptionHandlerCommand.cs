namespace SpaceBattle.Lib;
using Hwdtech;

public class ExceptionHandlerCommand : ICommand
{
    ICommand _cmd;
    Exception _ex;
    public ExceptionHandlerCommand(ICommand cmd, Exception ex)
    {
        _cmd = cmd;
        _ex = ex;
    }
    public void Execute()
    {
        string logFileName = IoC.Resolve<string>("Exception.GetLogFileName");

        var exMessage = $"Exception in command {_cmd.GetType().Name}. Message: {_ex.Message}";

        using StreamWriter writer = new StreamWriter(logFileName, true);

        writer.WriteLine(exMessage);
    }
}
