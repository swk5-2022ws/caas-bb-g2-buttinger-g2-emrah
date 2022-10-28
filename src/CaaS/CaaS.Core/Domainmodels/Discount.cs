namespace CaaS.Core.Domainmodels;
public record Discount
{
    public Discount(int id, int actionId, int ruleId)
    {
        Id = id;
        ActionId = actionId;
        RuleId = ruleId;
    }

    public int Id { get; set; }
    public int ActionId { get; set; }
    public int RuleId { get; set; }
}
