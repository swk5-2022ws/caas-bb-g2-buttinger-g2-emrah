using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;

namespace CaaS.Core.Domainmodels;
public record DiscountAction
{
    public DiscountAction(int id, int shopId, string name, DiscountActionBase action)
    {
        Action = action;
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
    public DiscountActionBase Action { get; set; }
}
