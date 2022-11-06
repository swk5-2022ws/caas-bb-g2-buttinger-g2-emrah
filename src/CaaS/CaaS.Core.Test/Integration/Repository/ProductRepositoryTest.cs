using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Integration.Repository
{
    public class ProductRepositoryTest
    {
        private readonly IProductRepository sut = new ProductRepository(Test.Setup.GetTemplateEngine());

        [Test]
        [TestCase(1, 1, "raynor", "a82724ba-ced5-32e8-9ada-17b06d427906")]
        [TestCase(2, 2, "krisbogan", "37f3184c-0dcd-3a58-975f-803648fcc73b")]
        [TestCase(3, 3, "purdy", "21de65da-b9f6-309f-973c-7fcbc36192cc")]
        public async Task TestGetWithValidIdReturnsValidShop(int id, int tenantId, string label, string appKey)
        {
        //    Shop? shop = await sut.Get(id);
        //    AssertShop(id, tenantId, label, appKey, shop);
        }

        [Test]
        public async Task TestGetWithInvalidIdReturnsNull()
        {
            //int id = int.MaxValue;
            //Shop? shop = await sut.Get(id);
            //Assert.That(shop, Is.Null);
        }
    }
}
