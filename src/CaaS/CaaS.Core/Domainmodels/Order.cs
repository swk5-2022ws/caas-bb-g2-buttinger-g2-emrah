using Caas.Core.Common.Attributes;

namespace CaaS.Core.Domainmodels;
public record Order
{
    public Order(int id, int cartId, double discount, DateTime orderDate)
    {
        Id = id;
        CartId = cartId;
        Discount = discount;
        OrderDate = orderDate;
    }

    public int Id { get; set; }

    public int CartId { get; set; }
    public double Discount { get; set; }
    public DateTime OrderDate { get; set; }

    [AdoIgnore]
    public Cart? Cart {get; set;}
}
