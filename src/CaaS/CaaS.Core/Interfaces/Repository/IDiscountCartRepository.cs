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
        /// Create a DiscountCart
        /// </summary>
        Task<bool> Create(DiscountCart discountCart);

        /// <summary>
        /// Gets all discounts which are valid for a cart.
        /// </summary>
        /// <param name="cartId">cart id</param>
        /// <returns>All discounts applied to a cart.</returns>
        Task<IList<CaaS.Core.Domainmodels.DiscountCart>> GetByCartId(int cartId);

        /// <summary>
        /// Deletes a discount cart
        /// </summary>
        Task<bool> Delete(DiscountCart discountCart);

    }
}
