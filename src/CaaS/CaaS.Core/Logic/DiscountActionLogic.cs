using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic.Util;
using CaaS.Core.Repository;
using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic
{
    public class DiscountActionLogic : IDiscountActionLogic
    {
        private readonly IDiscountActionRepository discountActionRepository;
        private readonly IShopRepository shopRepository;

        public DiscountActionLogic(IDiscountActionRepository discountActionRepository, IShopRepository shopRepository)
        {
            this.discountActionRepository = discountActionRepository;
            this.shopRepository = shopRepository;
        }

        public async Task<int> Create(Guid appKey, DiscountAction action)
        {
            if (action is null) throw ExceptionUtil.ParameterNullException(nameof(action));

            await Check.Shop(shopRepository, action!.ShopId, appKey);
            return await discountActionRepository.Create(action);
        }

        public async Task<bool> Delete(Guid appKey, int id)
        {
            await Check.DiscountAction(shopRepository, discountActionRepository, appKey, id);
            return await discountActionRepository.Delete(id);
        }

        public async Task<IEnumerable<DiscountAction>> GetByShopId(Guid appKey, int shopId)
        {
            await Check.Shop(shopRepository, shopId, appKey);
            return await discountActionRepository.GetByShopId(shopId);
        }

        public Task<IEnumerable<DiscountActionBase>> GetRulesets()
        {
            return Task.FromResult(
                DiscountActionBase.BuildSamples()
            );
        }
    }
}
