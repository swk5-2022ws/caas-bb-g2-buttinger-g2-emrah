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
    internal static class CouponMapping
    {
        public static Coupon ReadCouponOnly(IDataRecord record) =>
            new(record.GetIntByName(nameof(Coupon.Id)),
                record.GetIntByName(nameof(Coupon.ShopId)),
                record.GetDoubleByName(nameof(Coupon.Value)))
            {
                Deleted = record.GetNullableDateTimeByName(nameof(Coupon.Deleted)),
                CouponKey = record.GetStringByName(nameof(Coupon.CouponKey)),
                CartId = record.GetNullableIntByName(nameof(Coupon.CartId))
            };
    }
}
