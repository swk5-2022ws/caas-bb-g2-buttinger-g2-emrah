using CaaS.Core.Transferclasses;
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
        TDiscountAction Get(Guid id);

        /// <summary>
        /// Get all discount actions for a specific shop.
        /// </summary>
        /// <param name="id">The shop id to get the discount actions for.</param>
        IList<TDiscountAction> GetAllByShopId(Guid id);

        /// <summary>
        /// Creates a discount action.
        /// </summary>
        /// <param name="action">The discount aciton to create.</param>
        void Create(TDiscountAction action);

        /// <summary>
        /// Updates a discount action.
        /// </summary>
        /// <param name="action">The discount action to update.</param>
        void Update(TDiscountAction action);

        /// <summary>
        /// Delete a discount action.
        /// </summary>
        /// <param name="id">The discount aciton id to delete.</param>
        void Delete(Guid id);
    }
}
