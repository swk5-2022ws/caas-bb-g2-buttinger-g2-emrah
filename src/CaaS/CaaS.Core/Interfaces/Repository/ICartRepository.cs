using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface ICartRepository
    {
        /// <summary>
        /// Returns a product cart.
        /// </summary>
        /// <param name="id">Cart id</param>
        /// <returns>A product card</returns>
        Task<Cart?> Get(int id);

        /// <summary>
        /// Returns a cart from a customer
        /// </summary>
        /// <param name="id">customer id</param>
        /// <returns>A cart</returns>
        Task<Cart?> GetByCustomerId(int id);

        /// <summary>
        /// Deletes a cart. Only carts without orders can be deleted.
        /// </summary>
        /// <param name="id">cart id</param>
        Task<bool> Delete(int id);

        /// <summary>
        /// Creates a new cart
        /// </summary>
        /// <param name="cart">The cart that should be created</param>
        /// <returns>the id of the newly created cart</returns>
        Task<int> Create(Cart cart);
    }
}
