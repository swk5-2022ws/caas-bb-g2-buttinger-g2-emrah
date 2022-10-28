namespace CaaS.Core.Domainmodels;
public record Discount
{
    public Discount(int id, DiscountRule discountRule, DiscountAction discountAction)
    {
        Id = id;
        DiscountAction = discountAction;
        DiscountRule = discountRule;
    }

    public int Id { get; set; }
    public DiscountAction DiscountAction { get; set; }
    public DiscountRule DiscountRule { get; set; }
}
