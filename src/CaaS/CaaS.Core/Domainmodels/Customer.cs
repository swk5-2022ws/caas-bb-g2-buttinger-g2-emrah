using Caas.Core.Common.Attributes;
using CaaS.Core.Interfaces.Transferclasses;
namespace CaaS.Core.Domainmodels;

public record Customer : ISoftDeleteable
{
    public Customer(int id, int shopId, string name, string email)
    {
        Id = id;
        ShopId = shopId;
        Name = name;
        Email = email;
        Orders = new HashSet<Order>();
    }

    public int Id { get; set; }
    public int ShopId { get; set; }

    public int? CartId { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime? Deleted { get; set; }

    [AdoIgnore]
    public Cart? Cart { get; set; }
    [AdoIgnore]
    public HashSet<Order> Orders { get; set; }
}
