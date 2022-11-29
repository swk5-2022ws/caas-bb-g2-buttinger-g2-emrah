﻿using CaaS.Core.Domainmodels;
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

        [SetUp]
        public void InitializeSut()
        {
            ITenantRepository tenantRepository = new TenantRepositoryStub(new Dictionary<int, Tenant>()
            {
                {1, new Tenant(1, "test@mail.com", "xaver") }
            });

            IShopRepository shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
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
    }
}