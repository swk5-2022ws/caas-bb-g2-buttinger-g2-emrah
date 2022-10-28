using CaaS.Core.Interfaces.Transferclasses;
namespace CaaS.Core.Domainmodels;

public record Coupon : ISoftDeleteable
{
    public Coupon(int id, int shopId, double value)
    {
        Id = id;
        ShopId = shopId;
        Value = value;
    }

    public int Id { get; set; }
    public int? CartId { get; set; }
    public int ShopId { get; set; }
    public double Value { get; set; }
    public DateTime? Deleted { get; set; }
}
