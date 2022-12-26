using Caas.Core.Common.Attributes;
using CaaS.Core.Interfaces.Transferclasses;
namespace CaaS.Core.Domainmodels;

public record Customer : ISoftDeleteable
{
    public Customer(int id, int shopId, string name, string email, int? cartId = null, string? creditcartNumber = null, string? cvv = null, string? expiration = null)
    {
        Id = id;
        ShopId = shopId;
        Name = name;
        Email = email;
        CartId = cartId;
        CreditCardNumber = creditcartNumber;
        CVV = cvv;
        Expiration = expiration;
        Orders = new HashSet<Order>();
    }

    public int Id { get; set; }
    public int ShopId { get; set; }

    public int? CartId { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }

    public string? CreditCardNumber { get; set; }
    public string? CVV { get; set; }
    public string? Expiration { get; set; }

    public DateTime? Deleted { get; set; }

    [AdoIgnore]
    public Cart? Cart { get; set; }
    [AdoIgnore]
    public HashSet<Order> Orders { get; set; }
}
