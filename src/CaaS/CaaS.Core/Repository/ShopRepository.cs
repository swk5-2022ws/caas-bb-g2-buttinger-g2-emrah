using Caas.Core.Common.Ado;
using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;

namespace CaaS.Core.Repository
{
    public class ShopRepository : AdoRepository, IShopRepository
    {
        public ShopRepository(AdoTemplate adoTemplate) : base(adoTemplate)
        {

        }

        public async Task<int> Create(Shop shop) => (await template.InsertAsync<Shop>(shop))?.ElementAt(0) ?? 0;

        public async Task<Shop?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(reader =>
             new Shop(
                    reader.GetIntByName(nameof(Shop.Id)),
                    reader.GetIntByName(nameof(Shop.TenantId)),
                    reader.GetGuidByName(nameof(Shop.AppKey)),
                    reader.GetStringByName(nameof(Shop.Label))
                    ),
                whereExpression:
                    new { Id = id }
                );

        public async Task<bool> Update(Shop shop) => (await template.UpdateAsync<Shop>(shop, new { Id = shop.Id } )) > 0;
    }
}
