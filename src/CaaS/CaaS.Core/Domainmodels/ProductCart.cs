using Caas.Core.Common.Attributes;

namespace CaaS.Core.Domainmodels;
public record ProductCart
{
    public ProductCart(int productId, int cartId, double price, uint amount)
    {
        ProductId = productId;
        CartId = cartId;
        Price = price;
        Amount = amount;
    }

    public double Price { get; set; }
    public uint Amount { get; set; }

    public int ProductId { get; set; }
    public int CartId { get; set; }

    [AdoIgnore]
    public Cart? Cart { get; set; }
    [AdoIgnore]
    public Product? Product { get; set; }
}
