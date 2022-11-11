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
    public int Amount { get; set; }
    public Product Product { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Price, Amount, Product);
    }
}
