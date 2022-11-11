using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Integration.Repository
{
    [Category("Integration")]
    [TestFixture]
    public class ShopRepositoryTest
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

        [Test, Rollback]
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

        [Test, Rollback]
        public async Task TestCreateWithExistingIdThrowsException()
        {
            Shop shop = await sut.Get(1) ?? throw new NullReferenceException("No Shop for id 1 found.");
            shop.Label = "different label";

            Assert.CatchAsync(async () => await sut.Create(shop));
        }

        [TestCase(1, 2, "shop 1", "a82724ba-ced5-32e8-9ada-17b06d427907")]
        [TestCase(2, 3, "shop 2", "a82724ba-ced5-32e8-9ada-17b06d427908")]
        [TestCase(3, 4, "shop 3", "a82724ba-ced5-32e8-9ada-17b06d427909")]
        [Test, Rollback]
        public async Task TestUpdateWithExistingShopUpdatesShop(int id, int tenantId, string label, Guid appKey)
        {
            Shop shop = await sut.Get(id) ?? throw new NullReferenceException($"No Shop for id {id} found.");

            shop.Label = label;
            shop.AppKey = appKey;
            shop.TenantId = tenantId;

            bool isSuccess = await sut.Update(shop);

            shop = await sut.Get(id) ?? throw new NullReferenceException($"No Shop for id {id} found after update.");
            Assert.That(isSuccess, Is.True);
            AssertShop(id, tenantId, label, appKey, shop);
        }

        [Test, Rollback]
        public async Task TestUpdateWithNonExistendShopThrowsExceptionAsync()
        {
            Shop shop = new(int.MaxValue, int.MaxValue, Guid.NewGuid(), "new");
            bool isSuccess = await sut.Update(shop);
            Assert.That(isSuccess, Is.False);
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
