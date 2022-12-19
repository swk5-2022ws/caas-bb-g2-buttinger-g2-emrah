using System.Text.Json.Serialization;

namespace CaaS.Api.Transfers
{
    public record TCreateProduct([property:JsonRequired]int ShopId, string Description, string ImageUrl, [property: JsonRequired]string Label, [property: JsonRequired]double Price);
}
