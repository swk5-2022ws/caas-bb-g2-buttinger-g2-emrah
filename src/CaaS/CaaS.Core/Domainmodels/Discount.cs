using Caas.Core.Common.Attributes;

namespace CaaS.Core.Domainmodels;
public record Discount
{
    public Discount(int id, int ruleId, int actionId)
    {
        Id = id;
        RuleId = ruleId;
        ActionId = actionId;
    }

    public Discount(int id, DiscountRule discountRule, DiscountAction discountAction) : this(id, discountRule.Id, discountAction.Id)
    {
        Id = id;
        DiscountAction = discountAction;
        DiscountRule = discountRule;
    }

    public int Id { get; set; }
    public int RuleId { get; set; }
    public int ActionId { get; set; }
    [AdoIgnore]
    public DiscountAction? DiscountAction { get; set; }
    [AdoIgnore]
    public DiscountRule? DiscountRule { get; set; }
}
