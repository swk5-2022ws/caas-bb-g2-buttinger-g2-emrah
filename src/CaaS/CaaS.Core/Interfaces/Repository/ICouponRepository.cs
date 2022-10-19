using CaaS.Core.Transferclasses;
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
        void Create(TCoupon coupon);
        /// <summary>
        /// Applys a coupon to a cart.
        /// </summary>
        /// <param name="couponId">coupon id to be applied.</param>
        /// <param name="cardId">card id</param>
        void Apply(Guid couponId, Guid cardId);
        /// <summary>
        /// Soft deletes a coupon. Only coupons which are not applied to a cart can be deleted.
        /// </summary>
        /// <param name="id">The coupon id to be deleted.</param>
        void Delete(Guid id);
    }
}
