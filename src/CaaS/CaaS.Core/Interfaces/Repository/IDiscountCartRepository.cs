using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IDiscountCartRepository
    {
        /// <summary>
        /// Apply a discount action with a specific discount rule on a specific cart.
        /// </summary>
        /// <param name="cartId">cart id</param>
        /// <param name="ruleId">rule id</param>
        /// <param name="actionId">action id</param>
        Task ApplyDiscount(int cartId, int ruleId, int actionId);

        /// <summary>
        /// Gets all discounts which are valid for a cart.
        /// </summary>
        /// <param name="cartId">cart id</param>
        /// <returns>All discounts applied to a cart.</returns>
        Task<IList<Discount>> GetByCartId(int cartId);

    }
}
