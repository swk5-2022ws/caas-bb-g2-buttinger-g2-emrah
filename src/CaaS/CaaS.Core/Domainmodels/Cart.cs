using Caas.Core.Common.Attributes;
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

    [AdoIgnore]
    public Customer? Customer { get; set; }
    /// <summary>
    /// Contains the products with amount and price. 
    /// The ProductCart objects contain a reference to the product.
    /// </summary>
    [AdoIgnore]
    public HashSet<ProductCart> ProductCarts { get; set; }
    [AdoIgnore]
    public Coupon? Coupon { get; set; }    
    [AdoIgnore]
    public Discount? Discount { get; set; }
}
