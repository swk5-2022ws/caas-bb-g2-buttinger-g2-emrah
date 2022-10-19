using CaaS.Core.Transferclasses;
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
        IList<TProduct> Get(Guid id);
        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="product">A existing product</param>
        void Update(TProduct product);
        /// <summary>
        /// Creates a product.
        /// </summary>
        /// <param name="product">The product to be created. The uuid must be empty.</param>
        void Create(TProduct product);
        /// <summary>
        /// Soft deletes a product.
        /// </summary>
        /// <param name="id">The product id to be deleted.</param>
        void Delete(Guid id);
    }
}
