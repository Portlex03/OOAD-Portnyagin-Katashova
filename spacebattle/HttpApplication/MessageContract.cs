using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

namespace WebHttp;

[DataContract(Name = "MessageContract", Namespace = "http://spacebattle.com")]
public class MessageContract
{
    [DataMember(Name = "Type", IsRequired = true, Order = 1)]
    [OpenApiProperty(Description = "Тип команды.")]
    public required string Type { get; set; }

    [DataMember(Name = "GameId", IsRequired = true, Order = 2)]
    [OpenApiProperty(Description = "Id потока")]
    public required string GameId { get; set; }

    [DataMember(Name = "GameItemId", IsRequired = true, Order = 3)]
    [OpenApiProperty(Description = "Id объекта")]
    public required uint GameItemId { get; set; }

    [DataMember(Name = "InitialValues", IsRequired = true, Order = 4)]
    [OpenApiProperty(Description = "Начальные значения для команды")]
    public Dictionary<string, object>? InitialValues { get; set; }
}
