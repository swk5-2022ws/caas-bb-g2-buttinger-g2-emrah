﻿using CaaS.Core.Domainmodels;
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
        /// Returns a shop by id
        /// </summary>
        Task<Shop?> Get(int id);

        /// <summary>
        /// Returns shops associated with a tenant
        /// </summary>
        /// <param name="id">Tenant id</param>
        /// <returns>Shop associated with the passed tenant id</returns>
        Task<IList<Shop>> GetByTenantId(int id);
        /// <summary>
        /// Creates a shop.
        /// </summary>
        /// <param name="shop">A shop to be created. The id must be empty.</param>
        Task<int> Create(Shop shop);
        /// <summary>
        /// Updates a shop.
        /// </summary>
        /// <param name="shop">A shop to be updated.</param>
        Task<bool> Update(Shop shop);

        /// <summary>
        /// Deletes a shop.
        /// </summary>
        Task<bool> Delete(int id);
    }
}
