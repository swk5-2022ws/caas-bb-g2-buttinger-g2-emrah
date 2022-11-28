using Caas.Core.Common.Ado;
using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;

namespace CaaS.Core.Repository
{
    public class CartRepository : AdoRepository, ICartRepository
    {
        public CartRepository(IAdoTemplate adoTemplate) : base(adoTemplate)
        {
        }

        public async Task<int> Create(Cart customer) =>
            (await template.InsertAsync<Cart>(customer))?.ElementAt(0) ?? 0;

        public async Task<bool> Delete(int id) =>
            await template.DeleteAsync<Cart>(new
            {
                Id = id
            });

        public async Task<Cart?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(CartMapping.ReadCartOnly, whereExpression: new
                {
                    Id = id
                });

        public async Task<Cart?> GetByCustomer(int id) =>
            await template.QueryFirstOrDefaultAsync(CartMapping.ReadCartOnly, whereExpression: new
            {
                CustomerId = id
            });
    }
}
