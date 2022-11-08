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
        public async Task GetCartByIdWithValidIdReturnsCart(int id, int? customerId, string sessionId)
        {
            var cart = await sut.Get(id);

            Assert.That(cart, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(cart.Id, Is.EqualTo(id));
                Assert.That(cart.CustomerId, Is.EqualTo(customerId));
                Assert.That(cart.SessionId, Is.EqualTo(sessionId));
            });
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(101)]
        public async Task GetCartByIdWithInvalidValidIdReturnsNull(int id) =>
            Assert.That(await sut.Get(id), Is.Null);

        [Test, Rollback]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task DeleteCartByValidIdReturnsTrue(int id)
        {
            var deleted = await sut.Delete(id);

            Assert.That(deleted, Is.True);

            var deletedCustomer = await sut.Get(id);
            Assert.That(deletedCustomer, Is.Null);
        }

        [Test, Rollback]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(501)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task DeleteCustomerByInvalidIdReturnsFalse(int id) =>
            Assert.That(await sut.Delete(id), Is.False);
    }
}
