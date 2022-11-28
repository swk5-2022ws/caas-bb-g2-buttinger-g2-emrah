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
        Task<IList<Shop>> GetByTenantId(string tenantId);
        Task<bool> Update(Shop shop);
        /// <summary>
        /// Creates a new shop. A default AppKey will be generated.
        /// </summary>
        /// <param name="shop">Shop to create.</param>
        /// <returns>The Id of the created shop.</returns>
        Task<int> Create(Shop shop); 
    }
}
