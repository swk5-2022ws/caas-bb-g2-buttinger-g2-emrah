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
        void Get(Guid id);

        /// <summary>
        /// Get all customers by shop id
        /// </summary>
        /// <param name="shopId">The shop id which customer to get</param>
        void GetAllByShopId(Guid shopId);

        /// <summary>
        /// Creates a customer.
        /// </summary>
        /// <param name="customer">The customer to create.</param>
        void Create(Customer customer);

        /// <summary>
        /// Updates a customer.
        /// </summary>
        /// <param name="customer">The customer to update.</param>
        void Update(Customer customer);

        /// <summary>
        /// Soft deletes a customer.
        /// </summary>
        /// <param name="customer">The customer to delete.</param>
        void Delete(Guid id);
    }
}
