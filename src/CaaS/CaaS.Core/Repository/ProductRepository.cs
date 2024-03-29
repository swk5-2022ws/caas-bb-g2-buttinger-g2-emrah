﻿using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;

namespace CaaS.Core.Repository
{
    public class ProductRepository : AdoRepository, IProductRepository
    {
        public ProductRepository(IAdoTemplate adoTemplate) : base(adoTemplate)
        {

        }

        public async Task<int> Create(Product product) => (await template.InsertAsync<Product>(product))?.ElementAt(0) ?? 0;

        public async Task<bool> Delete(int id)
        {
            Product? product = await Get(id);
            if (product == null)
                return false;

            product.Deleted = DateTime.Now;
            return await Update(product);
        }

        public async Task<Product?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(
                ProductMapping.ReadProductOnly,
                whereExpression:
                    new { Id = id }
                );

        public async Task<IList<Product>> Get(IList<int> ids) =>
            (IList<Product>)await template.QueryAsync(
                ProductMapping.ReadProductOnly,
            whereExpression:
                new { Id = ids }
            );

        public async Task<IList<Product>> GetByShopId(int shopId) =>
            (IList<Product>)await template.QueryAsync(
                ProductMapping.ReadProductOnly,
            whereExpression:
                new { ShopId = shopId }
            );

        public async Task<IList<Product>> GetByShopIdWithFilter(int shopId, string filter) =>
            (IList<Product>)await template.QueryAsync(
                ProductMapping.ReadProductOnly,
            whereExpression:
                new { 
                    ShopId = shopId,
                    Label = new AdoSearch(filter)
                }
            );

        public async Task<bool> Update(Product product) => 
            (await template.UpdateAsync<Product>(product, new { Id = product.Id })) > 0;

        
    }
}
