using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    public interface IDiscountLogic
    {
        /// <summary>
        /// Adds the passed discounts to the cart with id cartId.
        /// All previously linked discounts will be removed from the cart.
        /// This method also validates if the passed discountIds are currently valid discounts for the shop associated with the cart.
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="cartId"></param>
        /// <param name="discountIds"></param>
        /// <returns></returns>
        Task AddDiscountsToCart(Guid appKey, int cartId, IList<int> discountIds);
        Task<IEnumerable<Domainmodels.Discount>> GetAvailableDiscountsByCartId(Guid appKey, int id);

    }
}
