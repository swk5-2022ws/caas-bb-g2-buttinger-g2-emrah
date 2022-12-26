using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;
using System.Text.Json.Serialization;

namespace CaaS.Api.Transfers
{
    public record TCreateDiscountRule([property: JsonRequired] int ShopId, [property: JsonRequired] string Name, [property: JsonRequired] DiscountRulesetBase RuleSet)
    {
    }
}
