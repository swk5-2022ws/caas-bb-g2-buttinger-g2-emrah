using CaaS.Core.Domainmodels;
using CaaS.Core.Test.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Integration.Template
{
    [Category("Integration")]
    [TestFixture]
    public class StoredProcedureQueryTemplateTest
    {
        record CartPrice(int ShopId, double AverageCartPrice);

        private CartPrice ReadToCustomer(IDataRecord reader) => new CartPrice((int)reader["ShopId"], (double)reader["AverageCartPrice"]);

        [Test, Rollback]
        public async Task DeleteSingleDisountActionById()
        {
            var selectStoredProcedure = await Setup.GetTemplateEngine().QueryStoredProcedure(ReadToCustomer, "sp_GetAverageCartPriceInPeriod", new
            {
                pshopid = 1,
                pstartdate = DateTime.MinValue,
                penddate = DateTime.Now
            });

            Assert.That(selectStoredProcedure.Count, Is.EqualTo(1));

        }
    }
}
