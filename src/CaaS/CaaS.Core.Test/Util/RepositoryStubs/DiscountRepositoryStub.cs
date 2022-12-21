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
        private readonly IDiscountActionRepository discountActionRepository;
        private readonly IDiscountRuleRepository discountRuleRepository;

        public DiscountRepositoryStub(IDictionary<int, Discount> discounts,
            IDiscountActionRepository discountActionRepository,
            IDiscountRuleRepository discountRuleRepository)
        {
            this.discounts = discounts;
            this.discountActionRepository = discountActionRepository;
            this.discountRuleRepository = discountRuleRepository;
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

        public async Task<Discount?> Get(int id)
        {
            discounts.TryGetValue(id, out Discount? discount);
            if (discount == null)
                return null;

            var rule = await discountRuleRepository.Get(discount.RuleId);
            var action = await discountActionRepository.Get(discount.ActionId);

            Discount fullDiscount = new(id, rule!, action!);

            return fullDiscount;
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
