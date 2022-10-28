using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Gets a order by id.
        /// </summary>
        /// <param name="id">Order id</param>
        Order Get(Guid id);
        /// <summary>
        /// Gets all orders for a specific shop.
        /// </summary>
        /// <param name="id">Shop id</param>
        /// <returns>All orders for a shop.</returns>
        IList<Order> GetOrdersByShopId(Guid id);
        /// <summary>
        /// Gets all orders for a specific customer.
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <returns>All orders for a customer.</returns>
        IList<Order> GetOrdersByCustomerId(Guid id);
        /// <summary>
        /// Creates a new order from a cart.
        /// </summary>
        /// <param name="cart">The cart which is ordered.</param>
        void Create(Cart cart);
    }
}
