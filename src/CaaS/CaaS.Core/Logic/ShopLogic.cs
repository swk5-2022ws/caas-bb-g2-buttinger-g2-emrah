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

        public async Task<bool> Delete(int id)
        {
            return await shopRepository.Delete(id);
        }

        public Task<Shop?> Get(int id)
        {
            return shopRepository.Get(id);
        }

        public Task<IList<Shop>> GetByTenantId(int tenantId)
        {
            return shopRepository.GetByTenantId(tenantId);
        }

        public Task<bool> Update(Shop shop)
        {
            if (shop.Id == 0) throw new ArgumentException("Can not update a new shop");
            if (shop.TenantId == 0) throw new ArgumentException("Can not update a shop without a tenant");
            if (string.IsNullOrWhiteSpace(shop.Label)) throw new ArgumentException($"Can not update a shop without a label");
            if (shop.AppKey == Guid.Empty) throw new ArgumentException("Can not update a shop without a AppKey");
            return shopRepository.Update(shop);
        }
    }
}
