using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface ICouponRepository
    {
        /// <summary>
        /// Creates a coupon.
        /// </summary>
        /// <param name="coupon">The coupon to be created.</param>
        Task Create(Coupon coupon);
        /// <summary>
        /// Apply a coupon to a cart.
        /// </summary>
        /// <param name="couponId">coupon id to be applied.</param>
        /// <param name="cardId">card id</param>
        Task Apply(int couponId, int cardId);
        /// <summary>
        /// Soft deletes a coupon. Only coupons which are not applied to a cart can be deleted.
        /// </summary>
        /// <param name="id">The coupon id to be deleted.</param>
        Task Delete(int id);
    }
}
