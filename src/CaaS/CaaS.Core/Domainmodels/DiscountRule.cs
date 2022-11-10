using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;
using System.Text.Json;

namespace CaaS.Core.Domainmodels;
public record DiscountRule
{
    public DiscountRule(int id, int shopId, string name, DiscountRulesetBase ruleset)
    {
        Ruleset = ruleset;
        Id = id;
        ShopId = shopId;
        Name = name;
    }


    public DiscountRule(int id, int shopId, string name, string ruleset) : this(id, shopId, name, DiscountRulesetBase.Deserialize(ruleset))
    {
    }

    public int Id { get; set; }
    public int ShopId { get; set; }
    public string Name { get; set; }
    public DiscountRulesetBase Ruleset { get; set; }
}
