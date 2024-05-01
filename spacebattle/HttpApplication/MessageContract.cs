using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

namespace WebHttp;

[DataContract(Name = "MessageContract", Namespace = "http://spacebattle.com")]
public class MessageContract
{
    [DataMember(Name = "Type", IsRequired = true, Order = 1)]
    [OpenApiProperty(Description = "Тип команды")]
    public required string Type { get; set; }

    [DataMember(Name = "GameID", IsRequired = true, Order = 2)]
    [OpenApiProperty(Description = "ID игры")]
    public required string GameID { get; set; }

    [DataMember(Name = "GameItemID", IsRequired = true, Order = 3)]
    [OpenApiProperty(Description = "ID игрового объекта")]
    public required uint GameItemID { get; set; }

    [DataMember(Name = "InitialValues", IsRequired = true, Order = 4)]
    [OpenApiProperty(Description = "Начальные значения для команды")]
    public Dictionary<string, object>? InitialValues { get; set; }
}
