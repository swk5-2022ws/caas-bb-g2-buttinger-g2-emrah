using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    public class ProductRepositoryStub: IProductRepository
    {
        private readonly IDictionary<int, Product> products;

        public ProductRepositoryStub(IDictionary<int, Product> products)
        {
            this.products = products;
        }

        public Task<int> Create(Product product)
        {
            var id = products.Keys.Max() + 1;
            products.Add(id, product);
            return Task.FromResult(id);
        }

        public Task<bool> Delete(int id)
        {
            if (products.TryGetValue(id, out Product? product))
            {
                products.Remove(id);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<Product?> Get(int id)
        {
            products.TryGetValue(id, out Product? product);
            return Task.FromResult(product);
        }

        public Task<IList<Product>> Get(IList<int> ids) =>
            Task.FromResult((IList<Product>)products.Where(x => ids.Contains(x.Key)).Select(x => x.Value).ToList());

        public Task<IList<Product>> GetByShopId(int id) =>
            Task.FromResult((IList<Product>)products.Values.Where(x => x.ShopId == id).ToList());

        public Task<IList<Product>> GetByShopIdWithFilter(int shopId, string filter) =>
            Task.FromResult((IList<Product>)products.Values.Where(x => x.ShopId == shopId && x.Label.Contains(filter)).ToList());


        public Task<bool> Update(Product product)
        {
            if (products.ContainsKey(product.Id))
            {
                products[product.Id] = product;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
