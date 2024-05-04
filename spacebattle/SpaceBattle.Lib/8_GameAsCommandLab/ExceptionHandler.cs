using Hwdtech;

namespace SpaceBattle.Lib;

public class ExceptionHandlerCmd : ICommand
{
    readonly Exception _ex;
    readonly ICommand _cmd;
    public ExceptionHandlerCmd(Exception ex, ICommand cmd)
    {
        _ex = ex;
        _cmd = cmd;
    }
    public void Execute()
    {
        var _exDict = IoC.Resolve<Dictionary<object, object>>("Exception.Tree");
        if (_exDict.ContainsKey(_cmd))
            IoC.Resolve<ICommand>("Exception.SuitableStrategy").Execute();
        else
        {
            _ex.Data["Command"] = _cmd;
            throw _ex;
        }
    }
}
