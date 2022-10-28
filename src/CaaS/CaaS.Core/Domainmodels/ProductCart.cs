namespace CaaS.Core.Domainmodels;
public record ProductCart
{
    public ProductCart(Product product, double price, int amount)
    {
        Product = product;
        Price = price;
        Amount = amount;
    }

    public double Price { get; set; }
    public int Amount { get; set; }
    public Product Product { get; set; }
}
