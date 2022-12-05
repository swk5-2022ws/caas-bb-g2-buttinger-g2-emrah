using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    public interface IDiscountLogic
    {
        Task<IEnumerable<Domainmodels.Discount>> GetAvailableDiscountsByCartId(Guid appKey, int id);

    }
}
