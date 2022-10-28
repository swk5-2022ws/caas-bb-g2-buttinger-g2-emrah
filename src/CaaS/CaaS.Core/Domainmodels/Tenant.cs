namespace CaaS.Core.Domainmodels;
public class Tenant
{
    public Tenant(int id, string email, string name)
    {
        Id = id;
        Email = email;
        Name = name;
    }

    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}
