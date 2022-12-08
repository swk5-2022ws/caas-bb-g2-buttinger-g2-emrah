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

        internal IEnumerable<string> RemoveCartIdForLast10CouponsForExistingCoupons() =>
            GenerateCouponUpdateStatementForCartIds(91, 10);
        private IEnumerable<string> GenerateCouponUpdateStatement(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                string sql = $"UPDATE Coupon SET CouponKey = '{Guid.NewGuid()}' WHERE Id = {i};";

                yield return sql;
            }
        }
        
        private IEnumerable<string> GenerateCouponUpdateStatementForCartIds(int start, int count)
        {
            for (int i = start; i < start + count; i++)
            {
                string sql = $"UPDATE Coupon SET Deleted=NULL,CartId=NULL WHERE Id = {i};";

                yield return sql;
            }
        }
    }
}
