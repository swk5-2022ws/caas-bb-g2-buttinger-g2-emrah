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
        Task<DiscountRule> Get(int id);

        /// <summary>
        /// Get all discount rules for a specific shop.
        /// </summary>
        /// <param name="id">The shop id to get the discount rules for.</param>
        Task<IList<DiscountRule>> GetAllByShopId(int id);

        /// <summary>
        /// Creates a discount rule.
        /// </summary>
        /// <param name="action">The discount rule to create.</param>
        Task Create(DiscountRule action);

        /// <summary>
        /// Updates a discount rule.
        /// </summary>
        /// <param name="action">The discount rule to update.</param>
        Task Update(DiscountRule action);

        /// <summary>
        /// Delete a discount rule.
        /// </summary>
        /// <param name="id">The discount rule id to delete.</param>
        Task Delete(Guid id);
    }
}
