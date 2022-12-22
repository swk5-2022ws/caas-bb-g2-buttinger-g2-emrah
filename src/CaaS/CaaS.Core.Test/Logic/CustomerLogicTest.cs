using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Test.Util.MemoryRepositories;
using CaaS.Core.Test.Util.RepositoryStubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Logic
{
    [Category("Unit")]
    [TestFixture]
    public class CustomerLogicTest
    {
        private Guid appKey = Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906");

        private ICustomerLogic sut;
        private ICustomerRepository customerRepository;
        private IShopRepository shopRepository;

        [SetUp]
        public void Init()
        {
            customerRepository = new CustomerRepositoryStub(new Dictionary<int, Customer>()
            {
                {1, new Customer(1, 1, "Test1", "mail1") },
                {2, new Customer(2, 1, "Test2", "mail2") },
                {3, new Customer(3, 2, "Test3", "mail3") },
            });

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, appKey, "shop") }
            });

            sut = new CustomerLogic(customerRepository, shopRepository);
        }

        [Test]
        public async Task TestGetWithValidIdReturnsCustomer()
        {
            var customer = await sut.Get(appKey, 1);

            Assert.Multiple(() =>
            {
                Assert.That(customer, Is.Not.Null);
                Assert.That(customer.Id, Is.EqualTo(1));
            });
        }

        [Test]
        public void TestGetWithInvalidIdThrowsArgumentException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.Get(appKey, int.MinValue));
        }

        [Test]
        public void TestGetWithInvalidAppKeyThrowArgumentException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.Get(Guid.NewGuid(), 1));
        }

        [Test]
        public async Task TestGetByShopIdWithValidIdReturnsCustomers()
        {
            var customers = (await sut.GetByShopId(appKey, 1)).ToList();

            Assert.That(customers, Has.Count.EqualTo(2));
        }

        [Test]
        public void TestGetByShopIdWithInvalidIdThrowsArgumentException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.GetByShopId(appKey, int.MinValue));
        }

        [Test]
        public void TestGetByShopIdWithInvalidAppKeyThrowUnauthorizedException()
        {
            Assert.CatchAsync<UnauthorizedAccessException>(async () => await sut.GetByShopId(Guid.NewGuid(), 1));
        }

        [Test]
        public async Task TestDeleteWithValidIdReturnsTrue()
        {
            var isDeleted = await sut.Delete(appKey, 1);

            Assert.That(isDeleted, Is.True);
        }

        [Test]
        public void TestDeleteWithInvalidIdThrowsArgumentException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.Delete(appKey, int.MaxValue));
        }

        [Test]
        public void TestDeleteWithInvalidAppKeyThrowsUnauthorizedException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.Delete(Guid.NewGuid(), 1));
        }

        [Test]
        public async Task TestUpdateWithValidCustomerReturnsTrue()
        {
            var isUpdated = await sut.Update(appKey, new(1, 1, "neu", "neu"));

            Assert.That(isUpdated, Is.True);
        }

        [Test]
        public void TestUpdateWithInvalidIdThrowsArgumentException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.Update(appKey, new(int.MaxValue, 1, "neu", "neu")));
        }

        [Test]
        public void TestUpdateWithInvalidAppKeyThrowsUnauthorizedException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.Update(Guid.NewGuid(), new(1, 1, "neu", "neu")));
        }


        [Test]
        public async Task TestCreateWithValidCustomerReturnsId()
        {
            var id = await sut.Create(appKey, new(0, 1, "neu", "neu"));

            Assert.That(id, Is.EqualTo(4));
        }

        [Test]
        public void TestCreateWithInvalidIdThrowsArgumentException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.Create(appKey, new(int.MaxValue, 1, "neu", "neu")));
        }

        [Test]
        public void TestCreateWithInvalidAppKeyThrowsUnauthorizedException()
        {
            Assert.CatchAsync<UnauthorizedAccessException>(async () => await sut.Create(Guid.NewGuid(), new(0, 1, "neu", "neu")));
        }
    }
}
