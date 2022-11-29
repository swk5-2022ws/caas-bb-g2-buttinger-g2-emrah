using System.Text.Json.Serialization;

namespace CaaS.Api.Transfers
{
    public record TEditProduct([property:JsonRequired]int Id, string Description, string ImageUrl, [property: JsonRequired]string label, [property: JsonRequired]double Price);
}
