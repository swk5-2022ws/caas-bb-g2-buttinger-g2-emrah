namespace CaaS.Core.Domainmodels;
public record Order
{
    public Order(int id, Cart cart, double discount, DateTime orderDate)
    {
        Id = id;
        Cart = cart;
        Discount = discount;
        OrderDate = orderDate;
    }

    public int Id { get; set; }
    public double Discount { get; set; }
    public DateTime OrderDate { get; set; }
    public Cart Cart {get; set;}
}
