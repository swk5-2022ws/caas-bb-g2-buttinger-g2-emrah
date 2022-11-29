using System.Text.Json.Serialization;

namespace CaaS.Api.Transfers
{
    public record TCreateShop([property: JsonRequired] string Label, [property: JsonRequired] int TenantId);
}
