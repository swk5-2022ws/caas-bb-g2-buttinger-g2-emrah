using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Discount
{
    internal interface IDiscountAction
    {
        /// <summary>
        /// Calculates the discount for the passed cart.
        /// The calculated value should be subtracted from the cart
        /// during checkout.
        /// </summary>
        /// <param name="cart">The cart which is qualified for a discount.</param>
        /// <returns>The calculated discount.</returns>
        double GetDiscount(Cart cart);
    }
}
