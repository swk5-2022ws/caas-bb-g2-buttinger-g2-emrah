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

        // TODO is that right? do we want a list of carts or do we want the last open cart?
        public async Task<Cart?> GetByCustomerId(int id) =>
            await template.QueryFirstOrDefaultAsync(CartMapping.ReadCartOnly, whereExpression: new
            {
                CustomerId = id
            });

        public async Task<Cart?> GetBySession(string sessionId) =>
            await template.QueryFirstOrDefaultAsync(CartMapping.ReadCartOnly, whereExpression: new
            {
                SessionId = sessionId
            });

        public async Task<bool> Update(Cart cart) =>
                    (await template.UpdateAsync<Cart>(cart, whereExpression: new { Id = cart.Id })) == 1;

    }
}
