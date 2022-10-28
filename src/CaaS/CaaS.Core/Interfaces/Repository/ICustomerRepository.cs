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
        Task Get(int id);

        /// <summary>
        /// Get all customers by shop id
        /// </summary>
        /// <param name="shopId">The shop id which customer to get</param>
        Task GetAllByShopId(int shopId);

        /// <summary>
        /// Creates a customer.
        /// </summary>
        /// <param name="customer">The customer to create.</param>
        Task Create(Customer customer);

        /// <summary>
        /// Updates a customer.
        /// </summary>
        /// <param name="customer">The customer to update.</param>
        Task Update(Customer customer);

        /// <summary>
        /// Soft deletes a customer.
        /// </summary>
        /// <param name="customer">The customer to delete.</param>
        Task Delete(int id);
    }
}
