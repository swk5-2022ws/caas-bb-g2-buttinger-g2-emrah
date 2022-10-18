using CaaS.Core.Transferclasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces
{
    public interface IDiscountRepository
    {
        /// <summary>
        /// Applys a discount action with a specific discount rule on a specific cart.
        /// </summary>
        /// <param name="cartId">cart id</param>
        /// <param name="ruleId">rule id</param>
        /// <param name="actionId">action id</param>
        void ApplyDiscount(Guid cartId, Guid ruleId, Guid actionId);
        /// <summary>
        /// Gets all discounts which are applied to a cart.
        /// </summary>
        /// <param name="cartId">cart id</param>
        /// <returns>All discounts applied to a cart.</returns>
        IList<TDiscount> Get(Guid cartId);
    }
}
