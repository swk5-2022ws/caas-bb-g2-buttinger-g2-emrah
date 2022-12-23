using Caas.Core.Common.Attributes;
using CaaS.Core.Domainmodels;

namespace CaaS.Api.Transfers
{
    public record TCart(int? CustomerId, string SessionId, double Price, HashSet<TProductCart> ProductCarts, HashSet<TDiscount> Discounts);
}
