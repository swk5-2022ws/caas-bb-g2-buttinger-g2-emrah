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
    internal class ShopRepositoryTest
    {
        private readonly IShopRepository sut = new ShopRepository(Test.Setup.GetTemplateEngine());

        [Test]
        [TestCase(1, 1, "raynor", "a82724ba-ced5-32e8-9ada-17b06d427906")]
        [TestCase(2, 2, "krisbogan", "37f3184c-0dcd-3a58-975f-803648fcc73b")]
        [TestCase(3, 3, "purdy", "21de65da-b9f6-309f-973c-7fcbc36192cc")]
        public async Task TestGetWithValidIdReturnsValidShop(int id, int tenantId, string label, string appKey)
        {
            Shop? shop = await sut.Get(id);
            AssertShop(id, tenantId, label, appKey, shop);
        }

        [Test]
        public async Task TestGetWithInvalidIdReturnsNull()
        {
            int id = int.MaxValue;
            Shop? shop = await sut.Get(id);
            Assert.That(shop, Is.Null);
        }

        [Test]
        [TestCase(1, "shop 1", "a82724ba-ced5-32e8-9ada-17b06d427907")]
        [TestCase(2, "shop 2", "a82724ba-ced5-32e8-9ada-17b06d427908")]
        [TestCase(3, "shop 3", "a82724ba-ced5-32e8-9ada-17b06d427909")]
        public async Task TestCreateWithValidShopInsertsShop(int tenantId, string label, Guid appKey)
        {
            Shop shop = new(0, tenantId, appKey, label);

            int actualId = await sut.Create(shop);
            Shop? actualShop = await sut.Get(actualId);

            Assert.That(actualId, Is.AtLeast(1));
            AssertShop(actualId, tenantId, label, appKey, actualShop);
        }

        #region Asserts
        private static void AssertShop(int id, int tenantId, string label, Guid appKey, Shop? shop)
        {
            Assert.That(shop, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(shop.Id, Is.EqualTo(id));
                Assert.That(shop.TenantId, Is.EqualTo(tenantId));
                Assert.That(shop.Label, Is.EqualTo(label));
                Assert.That(shop.AppKey, Is.EqualTo(appKey));
            });
        }

        private static void AssertShop(int id, int tenantId, string label, string appKey, Shop? shop)
        {
            AssertShop(id, tenantId, label, Guid.Parse(appKey), shop);
        }
        #endregion
    }
}
