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
    internal class DiscountRuleMapping
    {
        internal static DiscountRule ReadDiscountMappingOnly(IDataRecord reader) => new(
           reader.GetIntByName(nameof(DiscountRule.Id)),
           reader.GetIntByName(nameof(DiscountRule.ShopId)),
           reader.GetStringByName(nameof(DiscountRule.Name)),
           reader.GetStringByName(nameof(DiscountRule.Ruleset))
        );
        
    }
}
