using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
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
            await AuthorizationCheck(product.ShopId, appKey);
            if (product.Price < 0) throw new ArgumentOutOfRangeException("No prices below zero are allowed");

            return await productRepository.Create(product);
        }

        public async Task<bool> Delete(int id, Guid appKey)
        {
            if (id <= 0) throw ExceptionUtil.NoSuchIdException();
            var shopId = (await productRepository.Get(id))?.ShopId ?? -1;
            await AuthorizationCheck(shopId, appKey);
            return await productRepository.Delete(id);
        }

        public async Task<bool> Edit(Product product, Guid appKey)
        {
            if (product.Id <= 0) throw ExceptionUtil.NoSuchIdException();
            product.ShopId = (await productRepository.Get(product.Id))?.ShopId ?? throw ExceptionUtil.NoSuchProductException();
            await AuthorizationCheck(product.ShopId, appKey);

            return await productRepository.Update(product);
        }

        public async Task<Product> GetProductById(int id, Guid appKey)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException("No such id");
            var product = (await productRepository.Get(id)) ?? throw ExceptionUtil.NoSuchProductException();

            await AuthorizationCheck(product.ShopId, appKey);
            return product;
        }

        public async Task<IList<Product>> GetProductsByShopId(int shopId, Guid appKey, string? filter)
        {
            await AuthorizationCheck(shopId, appKey);

            if (string.IsNullOrEmpty(filter)) return await productRepository.GetByShopId(shopId);
            return await productRepository.GetByShopIdWithFilter(shopId, filter);
        }

        private async Task AuthorizationCheck(int shopId, Guid appKey)
        {
            var availableShop = await shopRepository.Get(shopId);

            if (availableShop is null) throw new ArgumentException($"The shop with id={shopId} is currently not available.");
            if (availableShop.AppKey != appKey) throw new ArgumentException($"You have not the right privileges.");
        }
    }
}
