using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Logic
{
    internal interface IStatisticLogic
    {
        Task<int> GetAppliedCouponCount(Guid appKey, int shopId);



    }
}
