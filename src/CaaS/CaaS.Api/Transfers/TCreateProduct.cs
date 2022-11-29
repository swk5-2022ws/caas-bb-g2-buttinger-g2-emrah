using System.Text.Json.Serialization;

namespace CaaS.Api.Transfers
{
    public record TCreateProduct([property:JsonRequired]int ShopId, string Description, string ImageUrl, [property: JsonRequired]string label, [property: JsonRequired]double Price);
}
