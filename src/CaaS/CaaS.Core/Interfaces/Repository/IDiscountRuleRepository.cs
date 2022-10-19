using CaaS.Core.Transferclasses;
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
        TDiscountRule Get(Guid id);

        /// <summary>
        /// Get all discount rules for a specific shop.
        /// </summary>
        /// <param name="id">The shop id to get the discount rules for.</param>
        IList<TDiscountRule> GetAllByShopId(Guid id);

        /// <summary>
        /// Creates a discount rule.
        /// </summary>
        /// <param name="action">The discount rule to create.</param>
        void Create(TDiscountRule action);

        /// <summary>
        /// Updates a discount rule.
        /// </summary>
        /// <param name="action">The discount rule to update.</param>
        void Update(TDiscountRule action);

        /// <summary>
        /// Delete a discount rule.
        /// </summary>
        /// <param name="id">The discount rule id to delete.</param>
        void Delete(Guid id);
    }
}
