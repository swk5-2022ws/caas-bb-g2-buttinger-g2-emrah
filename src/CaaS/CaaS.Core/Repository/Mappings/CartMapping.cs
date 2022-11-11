using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using System.Data;

namespace CaaS.Core.Repository.Mappings
{
    internal static class CartMapping
    {
        internal static Cart ReadCartOnly(IDataRecord record) =>
                new Cart(record.GetIntByName(nameof(Cart.Id)), record.GetStringByName(nameof(Cart.SessionId)))
                {
                    CustomerId = record.GetNullableIntByName(nameof(Cart.CustomerId))
                };
}
}
