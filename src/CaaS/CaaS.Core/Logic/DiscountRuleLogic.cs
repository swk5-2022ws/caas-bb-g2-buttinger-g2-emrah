using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic.Util;
using CaaS.Core.Logic.Util.Discounts;
using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic
{
    public class DiscountRuleLogic : IDiscountRuleLogic
    {
        private readonly IShopRepository shopRepository;
        public readonly IDiscountRuleRepository discountRuleRepository;

        public DiscountRuleLogic(IDiscountRuleRepository discountRuleRepository, IShopRepository shopRepository)
        {
            this.discountRuleRepository = discountRuleRepository;
            this.shopRepository = shopRepository;
        }

        public async Task<int> Create(Guid appKey, DiscountRule rule)
        {
            if(rule is null) throw ExceptionUtil.ParameterNullException(nameof(rule));

            await Check.Shop(shopRepository, rule!.ShopId, appKey);
            return await discountRuleRepository.Create(rule);
        }

        public async Task<bool> Delete(Guid appKey, int id)
        {  
            await Check.DiscountRule(shopRepository, discountRuleRepository, appKey, id);
            return await discountRuleRepository.Delete(id);
        }

        public async Task<IEnumerable<DiscountRule>> GetByShopId(Guid appKey, int shopId)
        {
            await Check.Shop(shopRepository, shopId, appKey);
            return await discountRuleRepository.GetByShopId(shopId);
        }

        public Task<IEnumerable<DiscountRulesetBase>> GetRulesets()
        {
            return Task.FromResult(
                DiscountRulesetBase.BuildSamples()
            );
        }
    }
}
