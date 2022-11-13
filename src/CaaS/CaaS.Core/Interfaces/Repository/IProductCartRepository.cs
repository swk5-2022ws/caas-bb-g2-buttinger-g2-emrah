using CaaS.Core.Domainmodels;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IProductCartRepository
    {
        /// <summary>
        /// Creates a new product cart relationship.
        /// </summary>
        /// <param name="productCart">The new product in a cart</param>
        Task<int> Create(ProductCart productCart);

        /// <summary>
        /// Retrieves a product from cart
        /// </summary>
        /// <param name="productId">The product id</param>
        /// <param name="cartId">The cart id</param>
        Task<ProductCart?> Get(int productId, int cartId);

        /// <summary>
        /// Retrieves a product list from cart by the cartId
        /// </summary>
        /// <param name="id">The cart id</param>
        Task<IList<ProductCart>> GetByCartId(int id);

        /// <summary>
        /// Updates the amount of a product cart.
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="cartId">cart id</param>
        /// <param name="amount">The new amount for a product in a cart</param>
        Task<bool> Update(int productId, int cartId, uint amount);
        /// <summary>
        /// Deletes a product in a cart.
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="cartId">cart id</param>
        Task<bool> Delete(int productId, int cartId);
    }
}
