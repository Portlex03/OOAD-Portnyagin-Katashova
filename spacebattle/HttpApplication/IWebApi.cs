using System.Net;
using CoreWCF;
using CoreWCF.OpenApi.Attributes;
using CoreWCF.Web;

namespace WebHttp;

[ServiceContract]
[OpenApiBasePath("/api")]
internal interface IWebApi
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "/command")]
    [OpenApiTag("Message")]
    [OpenApiResponse(ContentTypes = new[] { "application/json", "text" }, Description = "Accepted", StatusCode = HttpStatusCode.Accepted, Type = typeof(MessageContract))]
    void ProcessMessage(
        [OpenApiParameter(ContentTypes = new[] { "application/json", "text" }, Description = "Get message from http endpoint")] MessageContract param);
}
