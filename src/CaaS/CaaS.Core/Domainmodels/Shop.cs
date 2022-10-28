namespace CaaS.Core.Domainmodels;
public record Shop
{
    public Shop(int id, int tenantId, Guid appKey, string label)
    {
        Id = id;
        TenantId = tenantId;
        AppKey = appKey;
        Label = label;
    }

    public int Id { get; set; }
    public int TenantId { get; set; }
    public Guid AppKey { get; set; }
    public string Label { get; set; }
}
