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
    /// <summary>
    /// Discounts are added by a instance of IDiscountEngine. The discount engines ensures that the priority order is right.
    /// </summary>
    public IList<Discount>? Discounts { get; set; }

    /// <summary>
    /// Get the total price for this cart.
    /// </summary>
    public double Price
    {
        get
        {
            double price = 0;
            foreach (var productCart in ProductCarts)
            {
                price += productCart.Price * productCart.Amount;
            }
            if (price < 0.0) throw new ArgumentException($"Price can not be negative. Price: {price}.");

            return price;
        }
    }

    
}
