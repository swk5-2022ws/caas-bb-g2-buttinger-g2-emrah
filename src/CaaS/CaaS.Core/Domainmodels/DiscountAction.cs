using Caas.Core.Common.Attributes;
using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;
using System.Text.Json.Serialization;

namespace CaaS.Core.Domainmodels;
public record DiscountAction
{
    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DiscountAction()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }

    public DiscountAction(int id, int shopId, string name, DiscountActionBase action)
    {
        ActionObject = action;
        Id = id;
        ShopId = shopId;
        Name = name;
    }

    public DiscountAction(int id, int shopId, string name, string action) : this(id, shopId, name, DiscountActionBase.Deserialize(action))
    {
    }

    public int Id { get; set; }
    public int ShopId { get; set; }
    public string Name { get; set; }
    [AdoIgnore]
    public DiscountActionBase ActionObject { get; set; }
    [JsonIgnore]
    public string Action => DiscountActionBase.Serialize(ActionObject);
}
