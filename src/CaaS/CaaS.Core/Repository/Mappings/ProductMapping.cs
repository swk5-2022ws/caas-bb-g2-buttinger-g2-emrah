using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using System.Data;

namespace CaaS.Core.Repository.Mappings
{
    internal static class ProductMapping
    {
        internal static Product ReadProductOnly(IDataRecord reader) => new(
                reader.GetIntByName(nameof(Product.Id)),
                reader.GetIntByName(nameof(Product.ShopId)),
                reader.GetStringByName(nameof(Product.Description)),
                reader.GetStringByName(nameof(Product.ImageUrl)),
                reader.GetStringByName(nameof(Product.Label)),
                reader.GetDoubleByName(nameof(Product.Price))
                );
    }
}
