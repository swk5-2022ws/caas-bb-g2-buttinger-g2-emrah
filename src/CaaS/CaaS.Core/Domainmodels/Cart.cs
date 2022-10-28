using System;

namespace CaaS.Core.Domainmodels;

public record Cart
{
    public Cart(int id, string sessionId)
    {
        Id = id;
        SessionId = sessionId;
        ProductCarts = new HashSet<ProductCart>();
    }

    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public string SessionId { get; set; }
    public Customer? Customer { get; set; }
    /// <summary>
    /// Contains the products with amount and price. 
    /// The ProductCart objects contain a reference to the product.
    /// </summary>
    public HashSet<ProductCart> ProductCarts { get; set; }
    public Coupon? Coupon { get; set; }
    public Discount? Discount { get; set; }
}
