using CaaS.Core.Domainmodels.DiscountActions;
using System.Text.Json;

namespace CaaS.Core.Domainmodels;
public record DiscountRule
{
    public DiscountRule(int id, int shopId, string name, string ruleset)
    {
        Id = id;
        ShopId = shopId;
        Name = name;
        Ruleset = ruleset;
    }

    public int Id { get; set; }
    public int ShopId { get; set; }
    public string Name { get; set; }
    public string Ruleset { get; set; }
}
