using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IProductCartRepository
    {
        /// <summary>
        /// Creates a new product cart relationship.
        /// </summary>
        /// <param name="productCart">The new product in a cart</param>
        Task Create(ProductCart productCart);
        /// <summary>
        /// Updates the amount of a product cart.
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="cartId">cart id</param>
        /// <param name="amount">The new amount for a product in a cart</param>
        Task Update(int productId, int cartId, uint amount);
        /// <summary>
        /// Deletes a product in a cart.
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="cartId">cart id</param>
        Task Delete(int productId, int cartId);
    }
}
