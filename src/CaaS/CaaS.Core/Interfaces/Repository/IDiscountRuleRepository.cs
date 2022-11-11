using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IDiscountRuleRepository
    {
        /// <summary>
        /// Get a discount rule.
        /// </summary>
        /// <param name="id">The discount rule id to get.</param>
        Task<DiscountRule?> Get(int id);

        /// <summary>
        /// Get all discount rules for a specific shop.
        /// </summary>
        /// <param name="id">The shop id to get the discount rules for.</param>
        Task<IList<DiscountRule>> GetByShopId(int id);

        /// <summary>
        /// Creates a discount rule.
        /// </summary>
        /// <param name="action">The discount rule to create.</param>
        Task<int> Create(DiscountRule action);

        /// <summary>
        /// Updates a discount rule.
        /// </summary>
        /// <param name="action">The discount rule to update.</param>
        Task<bool> Update(DiscountRule action);

        /// <summary>
        /// Delete a discount rule.
        /// </summary>
        /// <param name="id">The discount rule id to delete.</param>
        Task<bool> Delete(int id);
    }
}
