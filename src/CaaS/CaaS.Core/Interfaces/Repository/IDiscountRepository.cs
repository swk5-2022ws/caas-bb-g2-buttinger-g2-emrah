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
        /// Returns a discount.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Domainmodels.Discount?> Get(int id);
        /// <summary>
        /// Get all discount
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<Domainmodels.Discount>> GetByShopId(int id);

        /// <summary>
        /// Creates a discount.
        /// </summary>
        /// <param name="discount"></param>
        /// <returns></returns>
        Task<int> Create(Domainmodels.Discount discount);

        /// <summary>
        /// Updates a discount.
        /// </summary>
        /// <param name="item">The discount to update.</param>
        Task<bool> Update(Domainmodels.Discount item);

        /// <summary>
        /// Deletes a discount.
        /// </summary>
        /// <param name="id">The discount id to deleted.</param>
        Task<bool> Delete(int id);
    }
}
