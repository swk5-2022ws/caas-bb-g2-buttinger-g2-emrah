using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Discount
{
    internal interface IDiscountRule
    {
        /// <summary>
        /// Checks if the passed cart is qualified for a discount.
        /// </summary>
        /// <param name="cart">The cart to check for a discount.</param>
        /// <returns>True if the cart is qualified for a discount.</returns>
        bool IsQualifiedForDiscount(Cart cart);
    }
}
