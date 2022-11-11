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
    internal class DiscountMapping
    {
        internal static Discount ReadDiscountOnly(IDataRecord reader) => new(
               reader.GetIntByName(nameof(Discount.Id)),
               reader.GetIntByName(nameof(Discount.ActionId)),
               reader.GetIntByName(nameof(Discount.RuleId))
               );
    }
}
