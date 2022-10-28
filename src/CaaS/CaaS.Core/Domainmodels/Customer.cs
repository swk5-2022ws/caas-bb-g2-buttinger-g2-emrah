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
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime? Deleted { get; set; }
    public Cart? Cart { get; set; }
    public HashSet<Order> Orders { get; set; }
}
