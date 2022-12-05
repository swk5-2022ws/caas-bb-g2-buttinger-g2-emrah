using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;

namespace CaaS.Core.Test.Integration.Repository
{
    public class CouponRepositoryTest
    {
        private ICouponRepository sut;
        [OneTimeSetUp]
        public void InitializeSut()
        {
            sut = new CouponRepository(Setup.GetTemplateEngine());
        }

        [Test]
        [TestCase("n")]
        [TestCase("")]
        [TestCase("nope")]
        public async Task GetCouponByKeyWithInvalidKeyReturnsNull(string key) =>
            Assert.That(await sut.GetByKey(key), Is.Null);

        [Test]
        [TestCase("5c3b41d7-b823-45e3-b6d2-bdf749076687", 1, 1, 1, 0.01)]
        [TestCase("97e03e5c-9e07-4977-b0ec-6f9e8d8db335", 2, 2, 2, 0.43)]
        [TestCase("b7eafb28-ae8d-4898-a466-8b8d011cd32e", 3, 3, 3, 0.86)]
        public async Task GetCouponByKeyWithValidKeyReturnsCoupon(string key, int id, int shopId, int cartId, double value)
        {
            var coupon = await sut.GetByKey(key);
            Assert.That(coupon, Is.Not.Null);
            Assert.That(coupon.Id, Is.EqualTo(id));
            Assert.That(coupon.CartId, Is.EqualTo(cartId));
            Assert.That(coupon.ShopId, Is.EqualTo(shopId));
            Assert.That(coupon.Value, Is.EqualTo(value));

        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task GetCouponByShopIdWithInvalidIdReturnsEmptyList(int id) =>
           Assert.That((await sut.GetByShopId(id)).Count, Is.EqualTo(0));

        [Test]
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        public async Task GetCouponByShopIdWithValidIdReturnsCouponList(int shopId, int listCount)
        {
            var coupons = await sut.GetByShopId(shopId);

            Assert.That(coupons, Is.Not.Null);
            Assert.That(coupons.Count, Is.EqualTo(listCount));
            Assert.That(coupons.Any(coupon => coupon is null), Is.False);
            Assert.That(coupons.All(coupon => coupon.ShopId == shopId), Is.True);
            Assert.That(coupons.All(coupon => coupon.Deleted is null), Is.True);
        }

        [Test, Rollback]
        [TestCase(7, 1, 15.1)]
        [TestCase(8, 2, 3)]
        [TestCase(9, 3, 10)]
        public async Task CreateCouponWithValidValuesReturnsId(int shopId, int cartId, double value)
        {
            var previousCustomerCount = (await sut.GetByShopId(shopId)).Count;
            var key = Guid.NewGuid().ToString();
            var insertedId = await sut.Create(new Domainmodels.Coupon(0, shopId, value)
            {
                CartId = cartId,
                CouponKey = key
            });
            var afterwardsCustomerCount = (await sut.GetByShopId(shopId)).Count;

            Assert.That(insertedId, Is.GreaterThan(0));
            Assert.That(afterwardsCustomerCount, Is.EqualTo(previousCustomerCount + 1));

            var insertedCustomer = await sut.GetByKey(key);
            Assert.That(insertedCustomer, Is.Not.Null);
            Assert.That(insertedCustomer.Id, Is.EqualTo(insertedId));
            Assert.That(insertedCustomer.ShopId, Is.EqualTo(shopId));
            Assert.That(insertedCustomer.CouponKey, Is.EqualTo(key));
            Assert.That(insertedCustomer.Value, Is.EqualTo(value));
        }

        [Test, Rollback]
        [TestCase(-1, 1)]
        [TestCase(0, 2)]
        [TestCase(int.MaxValue, 3)]
        [TestCase(int.MinValue, 4)]
        public void CreateCouponWithInValidValuesReturnsException(int shopId, double value) =>
            Assert.CatchAsync(async () => await sut.Create(new Coupon(0, shopId, value)));

        [Test, Rollback]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(100)]
        public async Task DeleteCouponWithDeletedIdReturnsFalse(int id) =>
            Assert.That(await sut.Delete(id), Is.False);

        [Test, Rollback]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task DeleteCouponWithInValidIdReturnsFalse(int id) =>
            Assert.That(await sut.Delete(id), Is.False);

        [Test, Rollback]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task DeleteCouponWithValidIdReturnsTrue(int shopId)
        {
            var key = Guid.NewGuid().ToString();
            var insertedId = await sut.Create(new Coupon(0, shopId, 0.0d)
            {
                CouponKey = key
            });

            Assert.That(await sut.Delete(insertedId), Is.True);
        }

        [Test, Rollback]
        [TestCase(1, "a")]
        [TestCase(2, "b")]
        [TestCase(3, "c")]
        public async Task ApplyCouponWithWrongKeyReturnsFalse(int id, string appkey) =>
            Assert.That(await sut.Apply(appkey, id), Is.False);

        [Test, Rollback]
        [TestCase(1, "5c3b41d7-b823-45e3-b6d2-bdf749076687")]
        [TestCase(2, "97e03e5c-9e07-4977-b0ec-6f9e8d8db335")]
        [TestCase(3, "b7eafb28-ae8d-4898-a466-8b8d011cd32e")]
        public async Task ApplyCouponWithWrongReferencedCartReturnsFalse(int id, string appkey) =>
            Assert.That(await sut.Apply(appkey, id), Is.False);

        [Test, Rollback]
        [TestCase(1, 10)]
        [TestCase(2, 2)]
        [TestCase(3, 25.5)]
        public async Task ApplyCouponWithValidIdReturnsTrue(int shopId, double value)
        {
            var sutCart = new CartRepository(Setup.GetTemplateEngine());
            var cartId = await sutCart.Create(new Cart(0, Guid.NewGuid().ToString()));
            var key = Guid.NewGuid().ToString();
            var insertedId = await sut.Create(new Coupon(0, shopId, value)
            {
                CouponKey = key
            });

            Assert.That(await sut.Apply(key, cartId), Is.True);
            var coupon = await sut.GetByKey(key);
            Assert.That(coupon, Is.Not.Null);
            Assert.That(coupon.Id, Is.EqualTo(insertedId));
            Assert.That(coupon.ShopId, Is.EqualTo(shopId));
            Assert.That(coupon.CouponKey, Is.EqualTo(key));
            Assert.That(coupon.Value, Is.EqualTo(value));
        }
    }
}
