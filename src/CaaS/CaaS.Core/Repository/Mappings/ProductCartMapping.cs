using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using System.Data;

namespace CaaS.Core.Repository.Mappings
{
    internal static class ProductCartMapping
    {
        internal static ProductCart ReadProductCartOnly(IDataRecord record) =>
            new(record.GetIntByName(nameof(ProductCart.ProductId)),
                record.GetIntByName(nameof(ProductCart.CartId)),
                record.GetDoubleByName(nameof(ProductCart.Price)),
                record.GetUIntByName(nameof(ProductCart.Amount)));
    }
}
