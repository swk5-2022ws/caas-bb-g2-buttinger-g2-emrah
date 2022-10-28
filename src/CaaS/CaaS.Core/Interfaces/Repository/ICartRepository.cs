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
        Cart Get(Guid id);
        /// <summary>
        /// Deletes a cart. Only carts without orders can be deleted.
        /// </summary>
        /// <param name="id">cart id</param>
        void Delete(Guid id);
    }
}
