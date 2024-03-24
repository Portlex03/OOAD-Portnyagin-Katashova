namespace SpaceBattle.Lib;
using Hwdtech;

public class StopServerCommand : ICommand
{
    public void Execute()
    {
        var threadDict = IoC.Resolve<Dictionary<int, object>>("Server.GetThreadDict");
        Barrier barrier = new Barrier(threadDict.Count + 1); // Создаем Barrier с количеством потоков

        // каждому элементу словаря посылаем с помощью
        // отправителя команду остановки
        threadDict.ToList().ForEach(idAndThreadPair =>
            IoC.Resolve<ICommand>(
                "Server.Thread.SendCommand",
                idAndThreadPair.Key,
                IoC.Resolve<ICommand>(
                    "Server.Thread.SoftStop", idAndThreadPair.Key,
                    () => { barrier.SignalAndWait(); })
            ).Execute()
        );

        barrier.SignalAndWait(); // Ждем завершения всех потоков
    }
}
