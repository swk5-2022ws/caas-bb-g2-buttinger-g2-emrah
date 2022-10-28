namespace CaaS.Core.Domainmodels;
public record DiscountAction
{
    public DiscountAction(int id, int shopId, int actionType, string name, double value)
    {
        Id = id;
        ShopId = shopId;
        ActionType = actionType;
        Name = name;
        Value = value;
    }

    public int Id { get; set; }
    public int ShopId { get; set; }
    public int ActionType { get; set; }
    public String Name { get; set; }
    public double Value { get; set; }
}
