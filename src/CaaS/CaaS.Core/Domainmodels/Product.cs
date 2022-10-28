using CaaS.Core.Interfaces.Transferclasses;

namespace CaaS.Core.Domainmodels;

public record Product : ISoftDeleteable
{
    public Product(int id, int shopId, string description, string imageUrl, string label, double price)
    {
        Id = id;
        ShopId = shopId;
        Description = description;
        ImageUrl = imageUrl;
        Label = label;
        Price = price;
    }

    public int Id { get; set; }
    public int ShopId { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Label { get; set; }
    public double Price { get; set; }
    public DateTime? Deleted { get; set; }
}
