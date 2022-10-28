namespace CaaS.Core.Domainmodels;
public record DiscountCart
{
    public DiscountCart(int cartId, int discountId)
    {
        CartId = cartId;
        DiscountId = discountId;
    }

    public int CartId { get; set; }
    public int DiscountId { get; set; }
}
