using CaaS.Core.Transferclasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces
{
    public interface IShopRepository
    {
        /// <summary>
        /// Returns a shop associated with a tenant
        /// </summary>
        /// <param name="id">Tenant id</param>
        /// <returns>Shop associated with the passed tenant id</returns>
        TShop Get(Guid id);
        /// <summary>
        /// Creates a shop.
        /// </summary>
        /// <param name="shop">A shop to be created. The id must be empty.</param>
        void Create(TShop shop);
        /// <summary>
        /// Updates a shop.
        /// </summary>
        /// <param name="shop">A shop to be updated.</param>
        void Update(TShop shop);
    }
}
