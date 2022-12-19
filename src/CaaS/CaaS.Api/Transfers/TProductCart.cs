using Caas.Core.Common.Attributes;
using CaaS.Core.Domainmodels;

namespace CaaS.Api.Transfers
{
    public record TProductCart(double Price, uint Amount, TProduct? Product);
}
