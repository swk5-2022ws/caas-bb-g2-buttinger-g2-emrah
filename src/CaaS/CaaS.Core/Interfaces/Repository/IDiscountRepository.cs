﻿using CaaS.Core.Transferclasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IDiscountRepository
    {
        /// <summary>
        /// Get all discount
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IList<TDiscount> GetAllByShopId(Guid id);

        /// <summary>
        /// Updates a discount.
        /// </summary>
        /// <param name="item">The discount to update.</param>
        void Update(TDiscount item);

        /// <summary>
        /// Deletes a discount.
        /// </summary>
        /// <param name="id">The discount id to deleted.</param>
        void Delete(Guid id);
    }
}
