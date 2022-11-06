using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Integration.Repository
{
    internal class ShopRepositoryTest
    {
        private readonly IShopRepository sut = new ShopRepository(Test.Setup.GetTemplateEngine());

        [Test]
        [TestCase(1, 1, "raynor", "a82724ba-ced5-32e8-9ada-17b06d427906")]
        [TestCase(2, 2, "krisbogan", "37f3184c-0dcd-3a58-975f-803648fcc73b")]
        [TestCase(3, 3, "purdy", "21de65da-b9f6-309f-973c-7fcbc36192cc")]
        public async Task TestGetWithValidIdReturnsValidShop(int id, int tenantId, string label, string appKey)
        {
            Domainmodels.Shop? shop = await sut.Get(id);
            Assert.That(shop, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(shop.Id, Is.EqualTo(id));
                Assert.That(shop.TenantId, Is.EqualTo(tenantId));
                Assert.That(shop.Label, Is.EqualTo(label));
                Assert.That(shop.AppKey, Is.EqualTo(Guid.Parse(appKey)));
            });
        }

        [Test]
        public async Task TestGetWithInvalidIdReturnsNull()
        {
            int id = int.MaxValue;
            Domainmodels.Shop? shop = await sut.Get(id);
            Assert.That(shop, Is.Null);
        }

        [Test]
        public async Task TestCreateWithValidShopInsertsShop()
        {
            Domainmodels.Shop shop = new Domainmodels.Shop(0, 1, Guid.NewGuid(), "new shop");
            int id = await sut.Create(shop);
            Assert.That(id, Is.AtLeast(1));
            
        }
    }
}
