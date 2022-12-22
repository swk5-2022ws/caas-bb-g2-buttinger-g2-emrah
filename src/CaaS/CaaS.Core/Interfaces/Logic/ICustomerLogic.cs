using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    public interface ICustomerLogic
    {
        /// <summary>
        /// Get all customers for a shop.
        /// </summary>
        Task<IEnumerable<Customer>> GetByShopId(Guid appKey, int shopId);

        /// <summary>
        /// Gets a customer.
        /// </summary
        Task<Customer> Get(Guid appKey, int id);

        /// <summary>
        /// Creates a customer.
        /// </summary>
        Task<int> Create(Guid appKey, Customer customer);

        /// <summary>
        /// Updates a customer.
        /// </summary>
        Task<bool> Update(Guid appKey, Customer customer);

        /// <summary>
        /// Deletes a customer.
        /// </summary>        
        Task<bool> Delete(Guid appKey, int id);
    }
}
