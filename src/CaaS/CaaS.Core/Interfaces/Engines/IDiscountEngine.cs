using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Engines
{
    public interface IDiscountEngine
    {
        /// <summary>
        /// Apply discounts to a cart. Previous discounts will be removed.
        /// </summary>
        void ApplyValidDiscounts(Cart cart);


        /// <summary>
        /// Calculates the discount amount for a cart.
        /// </summary>
        /// <returns>Returns the new price of a cart. (Price - discounts)</returns>
        double CalculateDiscountPrice(Cart cart);

    }
}
