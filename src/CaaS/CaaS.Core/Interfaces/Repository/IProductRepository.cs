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
        /// Returns all products for a given shop id.
        /// </summary>
        /// <param name="id">shop id</param>
        /// <returns>A list of all products for a specific shop</returns>
        IList<Product> Get(Guid id);
        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="product">A existing product</param>
        void Update(Product product);
        /// <summary>
        /// Creates a product.
        /// </summary>
        /// <param name="product">The product to be created. The UUID must be empty.</param>
        void Create(Product product);
        /// <summary>
        /// Soft deletes a product.
        /// </summary>
        /// <param name="id">The product id to be deleted.</param>
        void Delete(Guid id);
    }
}
