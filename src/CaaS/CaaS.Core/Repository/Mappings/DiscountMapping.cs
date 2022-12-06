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

        internal static Discount ReadDiscountWithActionAndRule(IDataRecord reader) => new(
                        reader.GetInt32(0),
                        new DiscountRule(
                            reader.GetInt32(3),
                            reader.GetInt32(4),
                            reader.GetString(5),
                            reader.GetString(6)
                            ),
                        new DiscountAction(
                            reader.GetInt32(7),
                            reader.GetInt32(8),
                            reader.GetString(9),
                            reader.GetString(10))
                        
            );
    }
}
