using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    public interface ICouponLogic
    {

        /// <summary>
        /// Retrieves coupons for a specific shop 
        /// </summary>
        /// <param name="shopId">the shop where the coupons are referenced</param>
        /// <param name="appKey">the key of the specified shop</param>
        /// <returns></returns>
        Task<IList<Coupon>> GetCoupons(int shopId, Guid appKey);

        /// <summary>
        /// Creates a coupon defined by the product itself
        /// </summary>
        /// <param name="coupon">coupon itself</param>
        /// <param name="appKey">the key of the specified shop</param>
        /// <param name="shopId">the shopid of the specified shop</param>
        /// <returns>the id of the newly created coupon</returns>
        Task<int> Create(Coupon coupon, Guid appKey);

        /// <summary>
        /// Deletes a coupon if the coupon is not referenced to a cart
        /// </summary>
        /// <param name="couponKey">the key of the coupon that should be deleted</param>
        /// <param name="appkey">the appKey of the shop</param>
        /// <returns>if the deletion could be performed or not</returns>
        Task<bool> Delete(string couponKey, Guid appkey);

        /// <summary>
        /// Applies a coupon on a specific cart. 
        /// A coupon is considered like a one time discount in our case.
        /// The price reduction does not come into play until the order is created. 
        /// If the order is created a price reduction will take place.
        /// </summary>
        /// <param name="cartId">The cart id where the coupon should be applied to</param>
        /// <param name="couponKey">The coupon key of the coupon that should be applied</param>
        /// <param name="appKey">The appKey that is used to authorize the coupon and the cart to a specfic shop.</param>
        /// <returns>Wether or not a coupon could be applied. If the appKey is considered invalid an exception will be thrown.</returns>
        Task<bool> Apply(string couponKey, int cartId, Guid appKey);
    }
}
