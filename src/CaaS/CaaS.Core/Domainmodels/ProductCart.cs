namespace CaaS.Core.Domainmodels;
public record ProductCart
{
    public ProductCart(int productId, int cartId, double price, int amount)
    {
        ProductId = productId;
        CartId = cartId;
        Price = price;
        Amount = amount;
    }

    public int ProductId { get; set; }
    public int CartId { get; set; }
    public double Price { get; set; }
    public int Amount { get; set; }
}
