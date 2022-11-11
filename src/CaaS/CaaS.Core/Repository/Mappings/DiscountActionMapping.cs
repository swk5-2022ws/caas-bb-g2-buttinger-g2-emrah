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
    internal class DiscountActionMapping
    {
        internal static DiscountAction ReadDiscountActionMappingOnly(IDataRecord reader) => new(
           reader.GetIntByName(nameof(DiscountAction.Id)),
           reader.GetIntByName(nameof(DiscountAction.ShopId)),
           reader.GetStringByName(nameof(DiscountAction.Name)),
           reader.GetStringByName(nameof(DiscountAction.Action))
       );
    }
}
