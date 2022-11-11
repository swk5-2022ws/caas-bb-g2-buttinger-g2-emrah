using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IDiscountActionRepository
    {
        /// <summary>
        /// Get a discount action.
        /// </summary>
        /// <param name="id">The discount action id to get.</param>
        Task<DiscountAction?> Get(int id);
        
        /// <summary>
        /// Get all discount actions for a specific shop.
        /// </summary>
        /// <param name="id">The shop id to get the discount actions for.</param>
        Task<IList<DiscountAction>> GetByShopId(int id);

        /// <summary>
        /// Creates a discount action.
        /// </summary>
        /// <param name="action">The discount action to create.</param>
        Task<int> Create(DiscountAction action);

        /// <summary>
        /// Updates a discount action.
        /// </summary>
        /// <param name="action">The discount action to update.</param>
        Task<bool> Update(DiscountAction action);

        /// <summary>
        /// Delete a discount action.
        /// </summary>
        /// <param name="id">The discount action id to delete.</param>
        Task<bool> Delete(int id);
    }
}
