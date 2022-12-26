using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Interfaces.Engines.PaymentRepository;
using CaaS.Core.Interfaces.Repository;
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
        
        internal IEnumerable<string> GenerateCreditInformationForExistingCustomers()
        {
            int count = 500;
            return GenererateCustomerCreditInformationStatement(count);
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

        private IEnumerable<string> GenererateCustomerCreditInformationStatement(int count)
        {
            IPaymentRepository repository = new PaymentRepositoryStub();

            var infos = repository.GetAll();
            for (int i = 1; i <= count; i++)
            {
                var info = infos[i % 5];
                string sql = $"UPDATE Customer SET CreditCardNumber='{info.CreditCardNumber}',CVV='{info.CVV}',Expiration='{info.Expiration}' WHERE Id = {i};";
                yield return sql;
            }

        }
    }
}
