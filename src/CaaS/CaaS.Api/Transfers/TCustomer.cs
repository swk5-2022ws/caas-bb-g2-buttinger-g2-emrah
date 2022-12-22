namespace CaaS.Api.Transfers
{
    public record TCustomer(int id, int shopId, string name, string email, int? cartId = null);
    public record TCreateCustomer(int shopId, string name, string email, int? cartId = null);
}
