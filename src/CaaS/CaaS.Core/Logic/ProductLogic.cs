using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic.Util;
using CaaS.Util;

namespace CaaS.Core.Logic
{
    public class ProductLogic : IProductLogic
    {
        private readonly IProductRepository productRepository;
        private readonly IShopRepository shopRepository;

        public ProductLogic(IProductRepository productRepository, IShopRepository shopRepository)
        {
            this.productRepository = productRepository ?? throw ExceptionUtil.ParameterNullException(nameof(productRepository));
            this.shopRepository = shopRepository ?? throw ExceptionUtil.ParameterNullException(nameof(shopRepository));
        }

        public async Task<int> Create(Product product, Guid appKey)
        {
            await Check.ShopAuthorization(shopRepository, product.ShopId, appKey);
            if (product.Price < 0) throw new ArgumentOutOfRangeException("No prices below zero are allowed");

            return await productRepository.Create(product);
        }

        public async Task<bool> Delete(int id, Guid appKey)
        {
            if (id <= 0) throw ExceptionUtil.NoSuchIdException();
            var shopId = (await productRepository.Get(id))?.ShopId ?? -1;
            await Check.ShopAuthorization(shopRepository, shopId, appKey);

            return await productRepository.Delete(id);
        }

        public async Task<bool> Edit(Product product, Guid appKey)
        {
            if (product.Id <= 0) throw ExceptionUtil.NoSuchIdException();
            product.ShopId = (await productRepository.Get(product.Id))?.ShopId ?? throw ExceptionUtil.NoSuchProductException();
            await Check.ShopAuthorization(shopRepository, product.ShopId, appKey);


            return await productRepository.Update(product);
        }

        public async Task<Product> GetProductById(int id, Guid appKey)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("No such id");
            var product = (await productRepository.Get(id)) ?? throw ExceptionUtil.NoSuchProductException();

            await Check.ShopAuthorization(shopRepository, product.ShopId, appKey);

            return product;
        }

        public async Task<IList<Product>> GetProductsByShopId(int shopId, Guid appKey, string? filter)
        {
            await Check.ShopAuthorization(shopRepository, shopId, appKey);

            if (string.IsNullOrEmpty(filter)) return await productRepository.GetByShopId(shopId);
            return await productRepository.GetByShopIdWithFilter(shopId, filter);
        }

    }
}
