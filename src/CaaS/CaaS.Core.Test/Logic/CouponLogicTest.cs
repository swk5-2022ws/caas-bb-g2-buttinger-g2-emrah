using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Test.Util.MemoryRepositories;
using CaaS.Core.Test.Util.RepositoryStubs;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Logic
{
    public class CouponLogicTest
    {
        ICouponLogic sut;
        ICouponRepository couponRepository;
        ICartRepository cartRepository;
        IProductCartRepository productCartRepository;
        IProductRepository productRepository;
        IShopRepository shopRepository;

        [SetUp]
        public void InitializeSut()
        {
            cartRepository = new CartRepositoryStub(new Dictionary<int, Cart>()
            {
                {1, new Cart(1, "test") },
                {2, new Cart(2, "test") },
                {3, new Cart(3, "test") },
                {4, new Cart(4, "test") },
            });
            productRepository = new ProductRepositoryStub(new Dictionary<int, Product>()
            {
                {1, new Product(1, 1, "test", "test", "test", 10) },
                {2, new Product(2, 1, "test", "test", "test", 20) },
                {4, new Product(4, 2, "test", "test", "test", 20) },
            });
            productCartRepository = new ProductCartRepositoryStub(new Dictionary<(int, int), ProductCart>()
            {
                {(1, 1), new ProductCart(1, 1, 10, 1) },
                {(2, 1), new ProductCart(2, 1, 20, 1) },
                {(3, 3), new ProductCart(3, 3, 20, 1) },
                {(4, 4), new ProductCart(4, 4, 20, 1) },
            });
            couponRepository = new CouponRepositoryStub(new Dictionary<int, Coupon>()
            {
                {1, new Coupon(1, 1, 10){ CouponKey = "testCoupon" } },
                {2, new Coupon(2, 1, 20){ CartId = 1, CouponKey="test" } },
                {3, new Coupon(3, 2, 20){ CouponKey="test2" } }
            });

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"), "shop") }
            });


            sut = new CouponLogic(couponRepository, cartRepository, productCartRepository, productRepository, shopRepository);
        }

        [Test]
        public void CreateCouponWithInvalidShopIdThrowsException() =>
            Assert.CatchAsync(async () => await sut.Create(new Coupon(0, 3, 10), Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public void CreateCouponWithInvalidAppKeyThrowsException() =>
            Assert.CatchAsync(async () => await sut.Create(new Coupon(0, 1, 10), Guid.NewGuid()));

        [Test]
        public async Task CreateCouponWithValidCouponReturnsCouponId()
        {
            var couponId = await sut.Create(new Coupon(0, 1, 10), Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"));
            Assert.That(couponId, Is.EqualTo(4));
        }

        [Test]
        public void DeleteCouponWithInvalidCouponKeyReturnsException() =>
            Assert.CatchAsync(async () => await sut.Delete("lala", Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public void DeleteCouponWithNonEmptyCartIdReturnsException() =>
            Assert.CatchAsync(async () => await sut.Delete("test", Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public async Task DeleteCouponWithEmptyCartIdReturnsTrue()
        {
            var deleted = await sut.Delete("testCoupon", Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"));
            Assert.That(deleted, Is.True);
        }

        [Test]
        public void GetCouponsWithInvalidShopIdReturnsException() =>
            Assert.CatchAsync(async () => await sut.GetCoupons(2, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));
        
        [Test]
        public void GetCouponsWithInvalidAppKeyReturnsException() =>
            Assert.CatchAsync(async () => await sut.GetCoupons(1, Guid.Parse("182724ba-ced5-32e8-9ada-17b06d427906")));
        [Test]
        public async Task GetCouponsWithValidShopIdAndAppKeyReturnsListOfCoupons()
        {
            var coupons = await sut.GetCoupons(1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"));

            Assert.That(coupons, Is.Not.Null);
            Assert.That(coupons.Count, Is.EqualTo(1));
        }

        [Test]
        public void ApplyCouponToCartWithInvalidCouponKeyReturnsException() =>
            Assert.CatchAsync(async () => await sut.Apply("no", 1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));
        [Test]
        public void ApplyCouponToCartWithInvalidCartIdReturnsException() =>
           Assert.CatchAsync(async () => await sut.Apply("testCoupon", 5, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));
        [Test]
        public void ApplyCouponToCartWithEmptyProductCartsReturnsException() =>
           Assert.CatchAsync(async () => await sut.Apply("testCoupon", 2, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public void ApplyCouponToCartWithEmptyProductsReturnsException() =>
            Assert.CatchAsync(async () => await sut.Apply("testCoupon", 3, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public void ApplyCouponToCartWithWrongShopInOnProductReturnsException() =>
            Assert.CatchAsync(async () => await sut.Apply("testCoupon", 4, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public void ApplyCouponToCartWithWrongShopInOnCouponReturnsException() =>
            Assert.CatchAsync(async () => await sut.Apply("test2", 2, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public void ApplyCouponToCartWithWrongAppKeyCouponReturnsException() =>
           Assert.CatchAsync(async () => await sut.Apply("testCoupon", 1, Guid.Parse("c82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public async Task ApplyCouponToCartReturnsTrue()
        {
            var applied = await sut.Apply("testCoupon", 1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"));
            Assert.That(applied, Is.True);
        }
    }
}
