using CaaS.Core.Domainmodels.DiscountActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Tools.Test.Data.Sql.InsertGenerator
{
    internal class CustomerUpdateSqlGenerator
    {
        internal IEnumerable<string> GenerateCartIdForExistingCustomers()
        {
            int count = 500;
            return GenerateCustomerUpdateStatement(count);
        }

        private IEnumerable<string> GenerateCustomerUpdateStatement(int count)
        {
            int cnt = 1;
            for (int i = 1; i <= count; i++)
            {
                string sql = $"UPDATE Customer SET CartId = '{cnt}' WHERE Id = {i};";

                if (cnt == 100)
                {
                    cnt = 1;
                }
                else { cnt++; }

                yield return sql;
            }
        }
    }
}
