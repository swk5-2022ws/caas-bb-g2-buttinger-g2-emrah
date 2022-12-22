using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaaS.Core.Domainmodels.DiscountActions;

namespace CaaS.Core.Interfaces.Logic
{
    internal interface IDiscountActionLogic
    {
        /// <summary>
        /// Gets all actions.
        /// </summary>
        Task<IEnumerable<DiscountActionBase>> GetRulesets();

        /// <summary>
        /// Gets all discount actions for a shop.
        /// </summary>
        Task<IEnumerable<DiscountAction>> GetByShopId(Guid appKey, int shopId);

        /// <summary>
        /// Creates a new discount action.
        /// </summary>
        Task<int> Create(Guid appKey, DiscountAction action);

        /// <summary>
        /// Deletes a discount action
        /// </summary
        Task<bool> Delete(Guid appKey, int id);
    }
}
