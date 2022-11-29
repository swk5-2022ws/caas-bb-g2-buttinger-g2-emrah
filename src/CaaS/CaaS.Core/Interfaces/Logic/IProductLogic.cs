using CaaS.Core.Domainmodels;

namespace CaaS.Core.Interfaces.Logic
{
    public interface IProductLogic
    {
        /// <summary>
        /// Get Product by its id
        /// </summary>
        /// <param name="id">The id of the product</param>
        /// <returns>the product</returns>
        Task<Product> GetProductById(int id);

        /// <summary>
        /// Retrieves Products for a specific shop 
        /// </summary>
        /// <param name="shopId">the shop where the products are referenced</param>
        /// <param name="appKey">the key of the specified shop</param>
        /// <returns></returns>
        Task<IList<Product>> GetProductsByShopId(int shopId, Guid appKey);

        /// <summary>
        /// Creates a product defined by the product itself
        /// </summary>
        /// <param name="product">product itself</param>
        /// <param name="appKey">the key of the specified shop</param>
        /// <returns>the id of the newly created product</returns>
        Task<int> Create(Product product, Guid appKey);

        /// <summary>
        /// Updates values of a product
        /// </summary>
        /// <param name="product">the product that is updated</param>
        /// <returns></returns>
        Task<bool> Edit(Product product);

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="id">the id of the product that should be deleted</param>
        /// <returns>if the deletion could be performed or not</returns>
        Task<bool> Delete(int id);
    }
}
