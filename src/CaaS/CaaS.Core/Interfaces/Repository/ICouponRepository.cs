﻿using CaaS.Core.Domainmodels;
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
        Task<int> Create(Coupon coupon);

        /// <summary>
        /// Retrieves a coupon by its key
        /// </summary>
        /// <param name="key">The key of the coupon</param>
        /// <returns>The coupon</returns>
        Task<Coupon?> GetByKey(string key);

        /// <summary>
        /// Retrieves all coupons for a shop
        /// </summary>
        /// <param name="id">The shopId</param>
        /// <returns>The coupons for a shop</returns>
        Task<IList<Coupon>> GetByShopId(int id);

        /// <summary>
        /// Retrieves an available Coupon key. If
        /// there are no coupon keys left. A new Coupon is generated.
        /// </summary>
        /// <param name="id">The shop id where the coupon is checked</param>
        /// <param name="value">The value the coupon should have</param>
        /// <returns>An available coupon Key</returns>
        Task<string?> GetAvailableCouponKey(int id, double value);

        /// <summary>
        /// Apply a coupon to a cart.
        /// </summary>
        /// <param name="couponKey">coupon key to be applied.</param>
        /// <param name="cartId">card id</param>
        /// <returns>Wether or not the couponKey could be applied to the cart</returns>
        Task<bool> Apply(string couponKey, int cartId);
        /// <summary>
        /// Soft deletes a coupon. Only coupons which are not applied to a cart can be deleted.
        /// </summary>
        /// <param name="id">The coupon id to be deleted.</param>
        Task<bool> Delete(int id);
    }
}
