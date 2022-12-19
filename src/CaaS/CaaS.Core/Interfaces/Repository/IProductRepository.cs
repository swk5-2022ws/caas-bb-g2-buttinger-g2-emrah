using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IProductRepository
    {
        /// <summary>
        /// Return a product by id
        /// </summary>
        /// <param name="id">The product id to fetch</param>
        /// <returns>The product or null if not found</returns>
        Task<Product?> Get(int id);

        /// <summary>
        /// Return products by ids
        /// </summary>
        /// <param name="id">The product ids to fetch</param>
        /// <returns>The product or null if not found</returns>
        Task<IList<Product>> Get(IList<int> ids);

        /// <summary>
        /// Returns all products for a given shop id.
        /// </summary>
        /// <param name="id">shop id</param>
        /// <returns>A list of all products for a specific shop</returns>
        Task<IList<Product>> GetByShopId(int id);

        /// <summary>
        /// Returns all products for a given shop that contain a specific words as a filter
        /// </summary>
        /// <param name="shopId">the shop id</param>
        /// <param name="filter">the filter used to check search for some names</param>
        /// <returns></returns>
        Task<IList<Product>> GetByShopIdWithFilter(int shopId, string filter);

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="product">A existing product</param>
        Task<bool> Update(Product product);
        /// <summary>
        /// Creates a product.
        /// </summary>
        /// <param name="product">The product to be created. The UUID must be empty.</param>
        Task<int> Create(Product product);
        /// <summary>
        /// Soft deletes a product.
        /// </summary>
        /// <param name="id">The product id to be deleted.</param>
        Task<bool> Delete(int id);
    }
}
