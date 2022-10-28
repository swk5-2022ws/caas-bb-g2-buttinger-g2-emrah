using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IDiscountRepository
    {
        /// <summary>
        /// Get all discount
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<Discount>> GetAllByShopId(int id);

        /// <summary>
        /// Updates a discount.
        /// </summary>
        /// <param name="item">The discount to update.</param>
        Task Update(Discount item);

        /// <summary>
        /// Deletes a discount.
        /// </summary>
        /// <param name="id">The discount id to deleted.</param>
        Task Delete(int id);
    }
}
