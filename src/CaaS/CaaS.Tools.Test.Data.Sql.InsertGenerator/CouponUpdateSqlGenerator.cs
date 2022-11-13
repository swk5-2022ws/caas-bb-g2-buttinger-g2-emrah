using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Tools.Test.Data.Sql.InsertGenerator
{
    internal class CouponUpdateSqlGenerator
    {
        internal IEnumerable<string> GenerateCouponKeyForExistingCoupons()
        {
            int count = 100;
            return GenerateCouponUpdateStatement(count);
        }

        private IEnumerable<string> GenerateCouponUpdateStatement(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                string sql = $"UPDATE Coupon SET CouponKey = '{Guid.NewGuid()}' WHERE Id = {i};";

                yield return sql;
            }
        }
    }
}
