using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using System.Data;

namespace CaaS.Core.Repository.Mappings
{
    internal class OrderMapping
    {
        internal static Order ReadOrderOnly(IDataRecord record) =>
            new(record.GetIntByName(nameof(Order.Id)),
                record.GetIntByName(nameof(Order.CartId)),
                record.GetDoubleByName(nameof(Order.Discount)),
                record.GetDateTimeByName(nameof(Order.OrderDate)));
    }
}
