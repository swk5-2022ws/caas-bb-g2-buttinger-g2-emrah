using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    internal class DiscountRepositoryStub : IDiscountRepository
    {
        private readonly IDictionary<int, Discount> discounts;

        public DiscountRepositoryStub(IDictionary<int, Discount> discounts)
        {
            this.discounts = discounts;
        }

        public Task<int> Create(Discount discount)
        {
            var id = discounts.Keys.Max() + 1;
            discounts.Add(id, discount);

            return Task.FromResult(id);
        }

        public Task<bool> Delete(int id)
        {
            if (discounts.TryGetValue(id, out Discount? discount))
            {
                discounts.Remove(id);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<Discount?> Get(int id)
        {
            discounts.TryGetValue(id, out Discount? discount);
            return Task.FromResult(discount);
        }

        public Task<IList<Discount>> GetByShopId(int id)
        {
            IList<Discount> discountsByShopId = new List<Discount>();
            foreach (var keyValuePair in discounts)
            {
                if (keyValuePair.Value!.DiscountAction!.ShopId == id && keyValuePair.Value!.DiscountRule!.ShopId == id)
                    discountsByShopId.Add(keyValuePair.Value);
            }
            return Task.FromResult(discountsByShopId);
        }

        public Task<bool> Update(Discount item)
        {
            if (discounts.ContainsKey(item.Id))
            {
                discounts[item.Id] = item;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
