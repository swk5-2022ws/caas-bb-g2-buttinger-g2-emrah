using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    public interface IDiscountLogic
    {
        Task AddDiscountsToCart(Guid appKey, int id, IList<int> discountIds);
        Task<IEnumerable<Domainmodels.Discount>> GetAvailableDiscountsByCartId(Guid appKey, int id);

    }
}
