﻿using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;

namespace CaaS.Core.Test.Integration.Repository
{
    public class CartRepositoryTest
    {
        private ICartRepository sut;
        [OneTimeSetUp]
        public void InitializeSut()
        {
            sut = new CartRepository(Setup.GetTemplateEngine());
        }

        [Test]
        [TestCase(1, 1, "7ee2dcbd-8e42-366d-9919-b96d65afd844")]
        [TestCase(2, 2, "747c7000-c0b2-330a-930a-1d14e39b1e64")]
        [TestCase(3, 3, "5c662d68-ead5-35fe-af4c-cf4470a8ff3d")]
        public async Task GetCartByIdWithValidIdReturnsCart(int id, int? customerId, string sessionId) =>
            BaseGetAssertions(await sut.Get(id), id, customerId, sessionId);


        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(101)]
        public async Task GetCartByIdWithInvalidValidIdReturnsNull(int id) =>
            Assert.That(await sut.Get(id), Is.Null);


        [Test]
        [TestCase(1, 1, "7ee2dcbd-8e42-366d-9919-b96d65afd844")]
        [TestCase(2, 2, "747c7000-c0b2-330a-930a-1d14e39b1e64")]
        [TestCase(3, 3, "5c662d68-ead5-35fe-af4c-cf4470a8ff3d")]
        public async Task GetCartByCustomerIdWithValidCustomerIdReturnsCart(int id, int? customerId, string sessionId) =>
            BaseGetAssertions(await sut.GetByCustomer(id), id, customerId, sessionId);

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(101)]
        public async Task GetCartByInvalidCustomerIdReturnsNull(int id) =>
            Assert.That(await sut.GetByCustomer(id), Is.Null);

        [Test,Rollback]
        public async Task CreateValidCartReturnsId()
        {
            var sessionId = Guid.NewGuid().ToString();
            var insertedId = await sut.Create(new Domainmodels.Cart(0, sessionId));
            Assert.That(insertedId, Is.GreaterThan(0));
            var cart = await sut.Get(insertedId);
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.SessionId, Is.EqualTo(sessionId));
        }

        [Test, Rollback]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        public void CreateValidCartWithInvalidReferenceThrowsException(int customerId) =>
            Assert.CatchAsync(async () => await sut.Create(new Domainmodels.Cart(0, Guid.NewGuid().ToString())
            {
                CustomerId = customerId
            }));

        [Test, Rollback]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void DeleteCartByValidIdWithReferencesToOrdersThrowsException(int id) =>
            Assert.CatchAsync(async () => await sut.Delete(id));


            

        [Test, Rollback]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(501)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task DeleteCustomerByInvalidIdReturnsFalse(int id) =>
            Assert.That(await sut.Delete(id), Is.False);
        
        [Test, Rollback]       
        public async Task DeleteCartByValidIdReturnsTrue()
        {
            var insertedId = await sut.Create(new Domainmodels.Cart(0, Guid.NewGuid().ToString()));

            Assert.That(insertedId, Is.GreaterThan(0));
            Assert.That(await sut.Delete(insertedId), Is.True);
            var deletedCustomer = await sut.Get(insertedId);
            Assert.That(deletedCustomer, Is.Null);
        }

        private void BaseGetAssertions(Cart? cart, int id, int? customerId, string sessionId)
        {
            Assert.That(cart, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(cart.Id, Is.EqualTo(id));
                Assert.That(cart.CustomerId, Is.EqualTo(customerId));
                Assert.That(cart.SessionId, Is.EqualTo(sessionId));
            });
        }
    }
}
