using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;

namespace CaaS.Core.Test.Integration.Repository
{
    public class OrderRepositoryTest
    {
        private IOrderRepository sut;
        [OneTimeSetUp]
        public void InitializeSut()
        {
            sut = new OrderRepository(Setup.GetTemplateEngine());
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public async Task GetOrderByIdWithInvalidIdReturnsNull(int id) =>
            Assert.That(await sut.Get(id), Is.Null);

        [Test]
        [TestCase(1, 1, 0.63)]
        [TestCase(2, 2, 0.81)]
        [TestCase(3, 3, 0.19)]
        public async Task GetOrderByIdWithValidIdReturnsOrder(int id, int cartId, double discount)
        {
            var order = await sut.Get(id);
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Id, Is.EqualTo(id));
            Assert.That(order.CartId, Is.EqualTo(cartId));
            Assert.That(order.Discount, Is.EqualTo(discount));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task GetOrderByCartIdWithInvalidCartIdReturnsEmptyList(int id) =>
            Assert.That((await sut.GetOrdersByCartId(id)).Count, Is.EqualTo(0));

        [Test]
        [TestCase(1, 2)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        public async Task GetOrderByCartIdWithValidCartIdReturnsOrderList(int id, int count)
        {
            var order = await sut.GetOrdersByCartId(id);

            Assert.That(order, Is.Not.Null);
            Assert.That(order.Count, Is.EqualTo(count));
            Assert.That(order.Any(order => order is null), Is.False);
            Assert.That(order.All(order => order.CartId == id), Is.True);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task GetOrdersByCustomerIdWithInvalidCustomerIdReturnsEmptyList(int id) =>
            Assert.That((await sut.GetOrdersByCustomerId(id)).Count, Is.EqualTo(0));

        [Test]
        [TestCase(1, 1, 2)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 2)]
        public async Task GetOrdersByCustomerIdWithValidCustomerIdReturnsOrderList(int id, int cartId, int count)
        {
            var order = await sut.GetOrdersByCartId(id);

            Assert.That(order, Is.Not.Null);
            Assert.That(order.Count, Is.EqualTo(count));
            Assert.That(order.Any(order => order is null), Is.False);
            Assert.That(order.All(order => order.CartId == cartId), Is.True);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task GetOrdersByShopIdWithInvalidShopIdReturnsEmptyList(int id) =>
           Assert.That((await sut.GetOrdersByShopId(id)).Count, Is.EqualTo(0));

        [Test]
        [TestCase(1, 1, 2)]
        [TestCase(2, 2, 2)]
        [TestCase(3, 3, 2)]
        public async Task GetOrdersByShopIdWithValidShopIdReturnsOrderList(int id, int cartId, int count)
        {
            var order = await sut.GetOrdersByShopId(id);

            Assert.That(order, Is.Not.Null);
            Assert.That(order.Count, Is.EqualTo(count));
            Assert.That(order.Any(order => order is null), Is.False);
            Assert.That(order.All(order => order.CartId == cartId), Is.True);
        }

        [Test, Rollback]
        [TestCase( -1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void CreateOrderWithInValidValuesReturnsException(int id) =>
            Assert.CatchAsync(async () => await sut.Create(new Cart(id, Guid.NewGuid().ToString())));


        [Test, Rollback]
        [TestCase(11, "7ee2dcbd-8e42-366d-9919-b96d65afd844")]
        [TestCase(12, "747c7000-c0b2-330a-930a-1d14e39b1e64")]
        [TestCase(13, "5c662d68-ead5-35fe-af4c-cf4470a8ff3d")]
        public async Task CreateOrderWithValidValuesReturnsOrder(int id, string sessionId)
        {
                var cart = new Cart(id, sessionId);
                var insertedId = await sut.Create(cart);

                Assert.That(insertedId, Is.GreaterThan(0));

                var insertedOrder = await sut.Get(insertedId);
                Assert.That(insertedOrder, Is.Not.Null);
                Assert.That(insertedOrder.CartId, Is.EqualTo(cart.Id));
        }

    }
}
