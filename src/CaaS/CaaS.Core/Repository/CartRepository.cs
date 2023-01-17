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
        public async Task<IList<Cart>> Get(IList<int> ids) =>
            (IList<Cart>)await template.QueryAsync(CartMapping.ReadCartOnly, whereExpression: new
            {
                Id = ids
            });

        public async Task<IList<Cart>> GetAll() =>
            (IList<Cart>)await template.QueryAsync(CartMapping.ReadCartOnly);

        // TODO is that right? do we want a list of carts or do we want the last open cart?
        /**
         * SELECT * FROM `Cart` c
LEFT JOIN `Order` o on o.CartId = c.Id AND c.CustomerId = 1
WHERE c.CustomerId = 1 AND o.Id IS NULL;



SELECT * FROM `Cart` c
LEFT JOIN `Order` o on o.CartId = c.Id AND c.CustomerId = 1
LEFT OUTER JOIN `Order` o2 ON (c.id = o2.CartId AND o.Id < o2.id)
WHERE c.CustomerId = 1 AND o.Id IS NULL;



SELECT * FROM `Cart` c
LEFT JOIN `Order` o on o.CartId = c.Id
LEFT OUTER JOIN `Order` o2 ON (c.id = o2.CartId AND o.Id < o2.id)
WHERE c.CustomerId = 1 AND o2.Id IS NULL AND o.id IS NULL;



SELECT * FROM `Cart` c
LEFT JOIN `Cart` c2 ON c2.CustomerId = c.CustomerId AND c.Id < c2.Id
WHERE c.customerid = 1 AND c2.Id IS NULL;



SELECT * FROM `Cart` c
LEFT JOIN `Cart` c2 ON c2.CustomerId = c.CustomerId AND c.Id < c2.Id
LEFT JOIN `Order` o ON o.CartId = c.Id
WHERE c.customerid = 1 AND c2.Id IS NULL AND o.Id IS NULL;
         */

        // TODO filter with SQL and not in memory
        public async Task<Cart?> GetByCustomerId(int id) =>
            (await template.QueryAsync(CartMapping.ReadCartOnly, whereExpression: new
            {
                CustomerId = id,
                o = new
                {
                    Id = (int?) null
                }
            },
            joins: $"LEFT JOIN `Order` o on o.CartId = t.id")).OrderBy(x => x.Id).LastOrDefault();

        public async Task<Cart?> GetBySession(string sessionId) =>
            await template.QueryFirstOrDefaultAsync(CartMapping.ReadCartOnly, whereExpression: new
            {
                SessionId = sessionId
            });

        public async Task<bool> Update(Cart cart) =>
                    (await template.UpdateAsync<Cart>(cart, whereExpression: new { Id = cart.Id })) == 1;

    }
}
