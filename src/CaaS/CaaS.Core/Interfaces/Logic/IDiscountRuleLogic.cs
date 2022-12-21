using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    public interface IDiscountRuleLogic
    {
        /// <summary>
        /// Gets all rule sets.
        /// </summary>
        Task<IEnumerable<DiscountRulesetBase>> GetRulesets();

        /// <summary>
        /// Gets all discount rules for a shop.
        /// </summary>
        Task<IEnumerable<DiscountRule>> GetByShopId(Guid appKey, int shopId);

        /// <summary>
        /// Creates a new discount rule.
        /// </summary>
        Task<int> Create(Guid appKey, DiscountRule rule);

        /// <summary>
        /// Deletes a discount rule
        /// </summary
        Task<bool> Delete(Guid appKey, int id);
    }
}
