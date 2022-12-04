using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic
{
    public class ShopLogic : IShopLogic
    {
        private readonly IShopRepository shopRepository;
        private readonly ITenantRepository tenantRepository;

        public ShopLogic(IShopRepository shopRepository, ITenantRepository tenantRepository) 
        {
            this.shopRepository = shopRepository;
            this.tenantRepository = tenantRepository;
        }

        public async Task<int> Create(Shop shop)
        {
            if (shop.Id != 0) throw new ArgumentException($"Can not create a shop with {nameof(shop.Id)} > 0. Most likely this is not a new entity.");
            if (string.IsNullOrWhiteSpace(shop.Label)) throw new ArgumentException($"Can not create a shop without a {nameof(shop.Label)}");
            if(await tenantRepository.Get(shop.TenantId) == null) throw new ArgumentException($"Tenant ({shop.TenantId}) does not exist.");

            return await shopRepository.Create(shop);
        }

        public async Task<bool> Delete(Guid appKey, int id)
        {
            await AuthorizationCheck(id, appKey);

            return await shopRepository.Delete(id);
        }

        public async Task<Shop?> Get(Guid appKey, int id)
        {
            await AuthorizationCheck(id, appKey);

            return await shopRepository.Get(id);
        }

        public async Task<IList<Shop>> GetByTenantId(int tenantId)
        {
            return await shopRepository.GetByTenantId(tenantId);
        }

        public async Task<bool> Update(Guid appKey, Shop shop)
        {
            await AuthorizationCheck(shop.Id, appKey);

            if (shop.Id == 0) throw new ArgumentException("Can not update a new shop");
            if (shop.TenantId == 0) throw new ArgumentException("Can not update a shop without a tenant");
            if (string.IsNullOrWhiteSpace(shop.Label)) throw new ArgumentException($"Can not update a shop without a label");
            if (shop.AppKey == Guid.Empty) throw new ArgumentException("Can not update a shop without a AppKey");
            return await shopRepository.Update(shop);
        }

        private async Task AuthorizationCheck(int shopId, Guid appKey)
        {
            var availableShop = await shopRepository.Get(shopId);

            if (availableShop is null) throw new KeyNotFoundException($"The shop with id={shopId} is currently not available.");
            if (availableShop.AppKey != appKey) throw new UnauthorizedAccessException($"You have not the right privileges.");
        }
    }
}
