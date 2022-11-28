using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;

namespace CaaS.Core.Repository
{
    public class ProductCartRepository : AdoRepository, IProductCartRepository
    {
        public ProductCartRepository(IAdoTemplate template) : base(template) { }

        public async Task<int> Create(ProductCart productCart) =>
            (await template.InsertAsync<ProductCart>(productCart, false))?.ElementAt(0) ?? 0;


        public async Task<bool> Delete(int productId, int cartId) => await template.DeleteAsync<ProductCart>(new
        {
            ProductId = productId,
            CartId = cartId
        });

        public async Task<ProductCart?> Get(int productId, int cartId) =>
            await template.QueryFirstOrDefaultAsync(ProductCartMapping.ReadProductCartOnly, whereExpression: new
            {
                ProductId = productId,
                CartId = cartId
            });

        public async Task<IList<ProductCart>> GetByCartId(int id) =>
            (IList<ProductCart>)await template.QueryAsync(ProductCartMapping.ReadProductCartOnly, whereExpression: new
            {
                CartId = id
            });

        public async Task<bool> Update(int productId, int cartId, uint amount) =>
            await template.UpdateAsync<ProductCart>(new { Amount = amount}, whereExpression: new {
                ProductId = productId,
                CartId = cartId
            }) == 1;
    }
}
