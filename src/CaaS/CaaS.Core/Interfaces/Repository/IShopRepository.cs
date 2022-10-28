using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IShopRepository
    {
        /// <summary>
        /// Returns a shop associated with a tenant
        /// </summary>
        /// <param name="id">Tenant id</param>
        /// <returns>Shop associated with the passed tenant id</returns>
        Task<Shop> Get(int id);
        /// <summary>
        /// Creates a shop.
        /// </summary>
        /// <param name="shop">A shop to be created. The id must be empty.</param>
        Task Create(Shop shop);
        /// <summary>
        /// Updates a shop.
        /// </summary>
        /// <param name="shop">A shop to be updated.</param>
        Task Update(Shop shop);
    }
}
