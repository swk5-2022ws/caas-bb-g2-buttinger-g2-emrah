using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util.MemoryRepositories;
using CaaS.Core.Test.Util.RepositoryStubs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Logic
{
    [Category("Unit")]
    [TestFixture]
    public class ShopLogicTest
    {
        IShopLogic sut;
        ITenantRepository tenantRepository;
        IShopRepository shopRepository;

        [SetUp]
        public void InitializeSut()
        {
            tenantRepository = new TenantRepositoryStub(new Dictionary<int, Tenant>()
            {
                {1, new Tenant(1, "test@mail.com", "1") },
                {2, new Tenant(2, "test@mail2.com", "2") }

            });

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, Guid.NewGuid(), "shop") }
            });


            sut = new ShopLogic(shopRepository, tenantRepository);
        }

        [Test]
        public async Task CreateWithValidShopCreatesShop()
        {
            var id = await sut.Create(new Shop(0, 1, Guid.NewGuid(), "neu"));
            Assert.That(id, Is.EqualTo(2));
        }

        [TestCase(1, 0, "neu")]
        [TestCase(0, 1, "")]
        [TestCase(0, 1, null)]
        [Test]
        public void CreateWithInvalidShopThrowsArgumentException(int shopId, int tenantId, string label)
        {
            var shop = new Shop(shopId, tenantId, Guid.NewGuid(), label);
            Assert.ThrowsAsync<ArgumentException>(() => sut.Create(shop));
        }

        [Test]
        public async Task GetWithValidShopIdReturnsShopAsync()
        {
            var shop = await sut.Get(1);
            Assert.That(shop, Is.Not.Null);
            Assert.That(shop.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetWithInvalidShopIdReturnsNull()
        {
            var shop = await sut.Get(int.MaxValue);
            Assert.That(shop, Is.Null);
        }

        [Test]
        public async Task UpdateWithValidShopUpdatesShop()
        {
            Guid appkey = Guid.NewGuid();
            var shop = await shopRepository.Get(1);
            shop.Label = "neu";
            shop.AppKey = appkey;
            shop.TenantId = 2;
            var result = await sut.Update(shop);
            shop = await shopRepository.Get(1);
            Assert.Multiple(() =>
            {
                Assert.That(appkey, Is.EqualTo(shop.AppKey));
                Assert.That("neu", Is.EqualTo(shop.Label));
                Assert.That(2, Is.EqualTo(shop.TenantId));
            });
        }

        [TestCase(1, "")]
        [TestCase(1, null)]
        [TestCase(int.MaxValue, null)]
        [Test]
        public async Task UpdateWithInvalidArgumentsThrowsException(int tenantId, string label)
        {
            Guid appkey = Guid.NewGuid();
            var shop = await shopRepository.Get(1);
            shop!.Label = label;
            shop.AppKey = appkey;
            shop.TenantId = tenantId;
            Assert.ThrowsAsync<ArgumentException>(async () => await sut.Update(shop) );
        }

        [Test]
        public async Task DeleteWithValidIdDeletesShop()
        {
            await shopRepository.Delete(1);
            var shop = await shopRepository.Get(1);
            Assert.That(shop, Is.Null);
        }

        [Test]
        public async Task DeleteWithValidIdReturnsTrue()
        {
            Assert.That(await shopRepository.Delete(1), Is.True);
        }

        [Test]
        public async Task DeleteWithInvalidIdReturnsFalse()
        {
            Assert.IsFalse(await shopRepository.Delete(int.MaxValue));
        }

    }
}
