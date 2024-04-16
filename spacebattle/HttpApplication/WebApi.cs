using CoreWCF;
using Hwdtech;

namespace WebHttp;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class WebApi : IWebApi
{
    public void GetMessage(MessageContract message)
    {
        var threadId = message.GameId;

        var cmd = IoC.Resolve<ICommand>("Command.CreateFromMessage", message);

        IoC.Resolve<ICommand>("Thread.SendCmd", threadId, cmd).Execute();
    }
}
