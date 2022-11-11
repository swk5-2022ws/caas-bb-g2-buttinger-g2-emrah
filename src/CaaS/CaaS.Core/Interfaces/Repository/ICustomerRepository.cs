using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface ICustomerRepository
    {
        /// <summary>
        /// Get a customer.
        /// </summary>
        /// <param name="id">The customer id to get</param>
        Task<Customer?> Get(int id);

        /// <summary>
        /// Get all customers by shop id
        /// </summary>
        /// <param name="shopId">The shop id which customer to get</param>
        Task<IList<Customer>> GetAllByShopId(int shopId);

        /// <summary>
        /// Creates a customer.
        /// </summary>
        /// <param name="customer">The customer to create.</param>
        Task<int> Create(Customer customer);

        /// <summary>
        /// Updates a customer.
        /// </summary>
        /// <param name="customer">The customer to update.</param>
        Task<bool> Update(Customer customer);

        /// <summary>
        /// Soft deletes a customer.
        /// </summary>
        /// <param name="customer">The customer to delete.</param>
        Task<bool> Delete(int id);
    }
}
