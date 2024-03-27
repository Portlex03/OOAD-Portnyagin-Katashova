namespace SpaceBattle.Lib;
using Hwdtech;

public class StopServerCommand : ICommand
{
    public void Execute()
    {
        var threadDict = IoC.Resolve<Dictionary<int, object>>("Server.GetThreadDict");
        Barrier barrier = new Barrier(threadDict.Count + 1);

        threadDict.ToList().ForEach(idAndThreadPair =>
            IoC.Resolve<ICommand>(
                "Server.Thread.SendCommand",
                idAndThreadPair.Key,
                IoC.Resolve<ICommand>(
                    "Server.Thread.SoftStop", idAndThreadPair.Key,
                    () => { barrier.RemoveParticipant(); })
            ).Execute()
        );

        barrier.SignalAndWait();
    }
}
