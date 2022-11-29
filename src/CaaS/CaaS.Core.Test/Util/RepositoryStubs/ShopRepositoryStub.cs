using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.MemoryRepositories
{
    internal class ShopRepositoryStub : IShopRepository
    {
        private readonly IDictionary<int, Shop> shops;

        public ShopRepositoryStub(IDictionary<int, Shop> shops)
        {
            this.shops = shops;
        }

        public Task<int> Create(Shop shop)
        {
            var id = shops.Keys.Max() + 1;
            shops.Add(id, shop);

            return Task.FromResult(id);
        }

        public Task<Shop?> Get(int id)
        {
            shops.TryGetValue(id, out Shop? shop);
            return Task.FromResult(shop);
        }

        public Task<Shop?> GetByTenantId(int id)
        {
            shops.TryGetValue(id, out Shop? value);
            return Task.FromResult(value);
        }

        public Task<bool> Update(Shop shop)
        {
            if (shops.ContainsKey(shop.Id))
            {
                shops[shop.Id] = shop;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        Task<IList<Shop>> IShopRepository.GetByTenantId(int id)
        {
            IList<Shop> shopsByTenantId = new List<Shop>();
            foreach (var keyValuePair in shops)
            {
                if (keyValuePair.Value.TenantId == id)
                    shopsByTenantId.Add(keyValuePair.Value);
            }
            return Task.FromResult(shopsByTenantId);
        }
    }
}
