namespace SpaceBattle.Lib;
using Hwdtech;

public interface IQueue
{
    void Put(ICommand cmd);
    ICommand Take();
}
