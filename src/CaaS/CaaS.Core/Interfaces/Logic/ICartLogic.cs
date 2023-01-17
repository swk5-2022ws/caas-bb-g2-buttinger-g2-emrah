using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    public interface ICartLogic
    {
        /// <summary>
        /// Adds a reference from a customer to a already given cart.
        /// </summary>
        /// <param name="customerId">the customer id</param>
        /// <param name="sessionId">the session id of the cart</param>
        /// <returns></returns>
        Task<bool> ReferenceCustomerToCart(int customerId, string sessionId, Guid appKey);

        /// <summary>
        /// Creates a new cart without a customer reference
        /// </summary>
        /// <returns>Session id of the created Cart</returns>
        Task<string> Create();

        /// <summary>
        /// Creates a new cart with a customer reference
        /// </summary>
        /// <param name="customerId">the id of the customer</param>
        /// <returns></returns>
        Task<string> CreateCartForCustomer(int customerId, Guid appKey);

        /// <summary>
        /// Adds a reference from a product and a cart into productCart
        /// </summary>
        /// <param name="sessionId">The sessionId of the cart</param>
        /// <param name="productId">The product id of the product</param>
        /// <param name="amount">Adds a specific amount if nothing is set an amount of 1 is considered</param>
        /// <returns></returns>
        Task<bool> ReferenceProductToCart(string sessionId, int productId, Guid appKey, uint? amount);

        /// <summary>
        /// Removes a productCart entry
        /// </summary>
        /// <param name="sessionId">The sessionid of the cart</param>
        /// <param name="productId">The product id</param>
        /// <param name="amount">Removes specific amounts of a product. If nothing is set the removal of the whole product is done.</param>
        /// <returns></returns>
        Task<bool> DeleteProductFromCart(string sessionId, int productId, Guid appKey, uint? amount);

        /// <summary>
        /// Retrieves a cart by its sessionId
        /// </summary>
        /// <param name="sessionId">the sessionId of the cart</param>
        /// <param name="appKey">the appKey</param>
        /// <returns></returns>
        Task<Cart> Get(string sessionId, Guid appKey);

        Task<Cart> GetByCustomerId(int customerId, Guid appKey);
    }
}
