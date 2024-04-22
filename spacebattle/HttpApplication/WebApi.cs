using CoreWCF;
using Hwdtech;

namespace WebHttp;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class WebApi : IWebApi
{
    public void ProcessMessage(MessageContract message)
    {
        var threadID = IoC.Resolve<string>("Game.GetThreadIDByGameID", message.GameID);

        var cmd = IoC.Resolve<ICommand>("Command.CreateFromMessage", message);

        IoC.Resolve<ICommand>("Thread.SendCmd", threadID, cmd).Execute();
    }
}
