using CaaS.Core.Domainmodels;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Gets a order by id.
        /// </summary>
        /// <param name="id">Order id</param>
        Task<Order?> Get(int id);
        /// <summary>
        /// Gets all orders for a specific shop.
        /// </summary>
        /// <param name="id">Shop id</param>
        /// <returns>All orders for a shop.</returns>
        Task<IList<Order>> GetOrdersByShopId(int id);
        /// <summary>
        /// Gets all orders for a specific customer.
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <returns>All orders for a customer.</returns>
        Task<IList<Order>> GetOrdersByCustomerId(int id);

        /// <summary>
        /// Retrieves all orders for a specific cart.
        /// </summary>
        /// <param name="id">Cart id</param>
        /// <returns>All orders for a specific cart</returns>
        Task<IList<Order>> GetOrdersByCartId(int id);

        /// <summary>
        /// Creates a new order from a cart.
        /// </summary>
        /// <param name="cart">The cart which is ordered.</param>
        Task<int> Create(Cart cart);
    }
}
