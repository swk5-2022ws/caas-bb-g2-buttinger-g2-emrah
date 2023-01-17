using Caas.Core.Common.Attributes;
using CaaS.Core.Domainmodels;

namespace CaaS.Api.Transfers
{
    public record TCart(int Id, int? CustomerId, string SessionId, double Price, double DiscountedPrice, HashSet<TProductCart> ProductCarts, HashSet<TDiscount> Discounts, Coupon? Coupon);
}
