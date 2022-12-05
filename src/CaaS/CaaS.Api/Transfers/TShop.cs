using CaaS.Core.Domainmodels;
using System.Text.Json.Serialization;

namespace CaaS.Api.Transfers
{
    public record TShop([property: JsonRequired] int Id, [property: JsonRequired] string Label, [property: JsonRequired] int TenantId, [property: JsonRequired] Guid appKey)
    {
    }
}
