using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Transferrecordes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository
{
    public class ProductRepository : AdoRepository, IProductRepository
    {
        public ProductRepository(AdoTemplate adoTemplate) : base(adoTemplate)
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

        public async Task<Product?> Get(int id)
        {
            return await template.QueryFirstOrDefaultAsync(
                ReadProduct,
                whereExpression:
                    new { Id = id }
                );
        }

        public async Task<IList<Product>> GetByShopId(int shopId)
        {
            return (IList<Product>)await template.QueryAsync(
                ReadProduct,
            whereExpression:
                new { ShopId = shopId }
            );
        }

        public async Task<bool> Update(Product product) => 
            (await template.UpdateAsync<Product>(product, new { Id = product.Id })) > 0;

        private Product ReadProduct(IDataRecord reader)
        {
            return new(
                (int)reader["Id"],
                (int)reader["ShopId"],
                (string)reader["Description"],
                (string)reader["ImageUrl"],
                (string)reader["Label"],
                (double)reader["Price"]
                  );
        }
    }
}
