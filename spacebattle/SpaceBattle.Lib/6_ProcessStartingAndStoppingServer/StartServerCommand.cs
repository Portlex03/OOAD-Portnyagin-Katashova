namespace SpaceBattle.Lib;
using Hwdtech;

public class StartServerCommand : ICommand
{
    int _threadCount;
    public StartServerCommand(int threadCount) => _threadCount = threadCount;
    public void Execute()
    {
        for (int threadId = 0; threadId < _threadCount; threadId++)
        {
            IoC.Resolve<ICommand>("Server.Thread.Create&Start", threadId).Execute();
        }
    }
}
