using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    public interface IOrderLogic
    {
        /// <summary>
        /// Creates an order from a cart.
        /// </summary>
        /// <param name="id">the id of the cart</param>
        /// <param name="appKey">the appKey of the shop</param>
        /// <returns></returns>
        Task<int> Create(int id, IEnumerable<CaaS.Core.Domainmodels.Discount> discounts, Guid appKey);

        /// <summary>
        /// Retrieves all orders for a shop
        /// </summary>
        /// <param name="shopId">the shop id</param>
        /// <param name="appKey">the appKey of the shop</param>
        /// <returns></returns>
        Task<IList<Order>> GetByShopId(int shopId, Guid appKey);

        /// <summary>
        /// Retrieves all orders for a customer
        /// </summary>
        /// <param name="customerId">the customer Id</param>
        /// <param name="appKey">the appKey of the shop</param>
        /// <returns></returns>
        Task<IList<Order>> GetByCustomerId(int customerId, Guid appKey);

        /// <summary>
        /// Retrieves an order by its id
        /// </summary>
        /// <param name="id">the order id</param>
        /// <param name="appKey">the appKey of the shop</param>
        /// <returns></returns>
        Task<Order> Get(int id, Guid appKey);

        /// <summary>
        /// Returns true if the order could be payed. Else false is returned
        /// </summary>
        /// <param name="id">the id of the order</param>
        /// <param name="appKey">the appKey of the shop</param>
        /// <returns></returns>
        Task<bool> Pay(int id, Guid appKey);
    }
}
