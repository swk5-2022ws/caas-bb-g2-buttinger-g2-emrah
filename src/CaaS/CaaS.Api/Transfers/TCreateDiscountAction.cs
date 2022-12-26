using CaaS.Core.Domainmodels.DiscountActions;
using System.Text.Json.Serialization;

namespace CaaS.Api.Transfers
{
    public record TCreateDiscountAction([property: JsonRequired] int ShopId, [property: JsonRequired] string Name, [property: JsonRequired] DiscountActionBase ActionObject)
    {
    }
}
