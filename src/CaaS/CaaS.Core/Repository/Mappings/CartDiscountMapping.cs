using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository.Mappings
{
    internal static class CartDiscountMapping
    {
        internal static DiscountCart Read(IDataRecord record) =>
            new DiscountCart(record.GetIntByName(nameof(DiscountCart.CartId)), record.GetIntByName(nameof(DiscountCart.DiscountId)));
    }
}
