namespace SpaceBattle.Lib;
using Hwdtech;

public class StartServerCommand : ICommand
{
    int _threadCount;
    public StartServerCommand(int threadCount) => _threadCount = threadCount;
    public void Execute()
    {
        Enumerable.Range(0, _threadCount).ToList().ForEach(threadId =>
            IoC.Resolve<ICommand>("Server.Thread.Create&Start", threadId
        ).Execute());
    }
}
