using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    public interface IShopLogic
    {
        /// <summary>
        /// Get all shops for a tenant
        /// </summary>
        Task<IList<Shop>> GetByTenantId(int tenantId);
        /// <summary>
        /// Updates a shop
        /// </summary>
        Task<bool> Update(Guid appKey, Shop shop);
        /// <summary>
        /// Creates a new shop. A default AppKey will be generated.
        /// </summary>
        /// <param name="shop">Shop to create.</param>
        /// <returns>The Id of the created shop.</returns>
        Task<int> Create(Shop shop);
        /// <summary>
        /// Returns a shop by id
        /// </summary>
        Task<Shop?> Get(Guid appKey, int id);

        /// <summary>
        /// Deletes a shop by id
        /// </summary>
        Task<bool> Delete(Guid appKey, int id);
    }
}
