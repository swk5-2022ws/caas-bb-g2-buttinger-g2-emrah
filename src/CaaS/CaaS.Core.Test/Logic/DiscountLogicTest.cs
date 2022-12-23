using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
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
    public class DiscountLogicTest
    {
        private IDiscountLogic sut;
        private ICartRepository cartRepository;
        private IShopRepository shopRepository;
        private IDiscountRepository discountRepository;
        private IProductCartRepository productCartRepository;
        private IDiscountCartRepository discountCartRepository;
        private IProductRepository productRepository;
        private IDiscountActionRepository discountActionRepository;
        private IDiscountRuleRepository discountRuleRepository;

        private static readonly Guid appKey = Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906");

        [SetUp]
        public void InitializeSut()
        {

            discountActionRepository = new DiscountActionRepositoryStub(new Dictionary<int, DiscountAction>()
            {
                {1, new DiscountAction(1, 1, "valid", new FixedValueDiscountAction(100.0))},
                {2, new DiscountAction(3, 1, "invalid", new FixedValueDiscountAction(100.0))},
                {3, new DiscountAction(3, 1, "invalid", new FixedValueDiscountAction(100.0))},
            });

            discountRuleRepository = new DiscountRuleRepositoryStub(new Dictionary<int, DiscountRule>()
            {
                {1, new DiscountRule(1, 1, "valid",
                        new DateDiscountRuleset(DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(5), DateTime.Now)) },
                {2, new DiscountRule(2, 1, "valid",
                        new TotalAmountDiscountRuleset(300.0)) },
                {3, new DiscountRule(3, 1, "invalid",
                        new TotalAmountDiscountRuleset(500.0))}
            });

            discountRepository = new DiscountRepositoryStub(new Dictionary<int, Discount>()
            {
                // valid
                {1, new Discount(1, new DiscountRule(1, 1, "valid",
                        new DateDiscountRuleset(DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(5), DateTime.Now)),
                    new DiscountAction(1, 1, "valid",
                        new FixedValueDiscountAction(100.0)
                    )) },
                {2, new Discount(2,
                    new DiscountRule(2, 1, "valid",
                        new TotalAmountDiscountRuleset(300.0)),
                    new DiscountAction(2, 1, "valid",
                        new FixedValueDiscountAction(100.0)
                    )
               ) },
                // not valid
                {3,
                new Discount(3,
                   new DiscountRule(3, 1, "invalid",
                        new TotalAmountDiscountRuleset(500.0)),
                    new DiscountAction(3, 1, "invalid",
                        new FixedValueDiscountAction(100.0)
                    ) )}
            },
            discountActionRepository,
            discountRuleRepository);

            var cart = new Cart(1, "a82724ba-ced5-32e8-9ada-17b06d427906")
            {
                CustomerId = 1
            };
            // sum of products = 300
            // one discount applies _NOW_
            // one discount applies sum >= 300
            cart.ProductCarts.Add(new ProductCart(new Product(1, 1, "", "", "", 2.0), 1, 100.0, 3));
            cart.ProductCarts.Add(new ProductCart(new Product(2, 1, "", "", "", 2.0), 1, 100.0, 1));

            cartRepository = new CartRepositoryStub(new Dictionary<int, Cart>()
            {
                {1, cart },
                {2, new Cart(2, "a82724ba-ced5-32e8-9ada-17b06d427907") }
            });

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, appKey, "shop") }
            });

            productCartRepository = new ProductCartRepositoryStub(new Dictionary<(int, int), ProductCart>()
            {
                {(1, 1), new ProductCart(1, 1, 10, 1) { Product = new(1, 1, "Produkt 1", "http://produkt.at", "Produkt 1", 10.0)} },
                {(2, 1), new ProductCart(2, 1, 20, 2) { Product = new(2, 1, "Produkt 2", "http://produkt.at", "Produkt 2", 10.0)} },
                {(2, 2), new ProductCart(2, 2, 20, 2) { Product = new(3, 1, "Produkt 3", "http://produkt.at", "Produkt 3", 10.0)} },
                {(5, 2), new ProductCart(5, 2, 20, 2) { Product = new(4, 1, "Produkt 4", "http://produkt.at", "Produkt 4", 10.0)} },
                {(3, 3), new ProductCart(3, 3, 20, 1) { Product = new(5, 1, "Produkt 5", "http://produkt.at", "Produkt 5", 10.0)} },
                {(4, 4), new ProductCart(4, 4, 20, 1) { Product = new(6, 1, "Produkt 6", "http://produkt.at", "Produkt 6", 10.0)} },
            });

            productRepository = new ProductRepositoryStub(new Dictionary<int, Product>()
            {
                {1 ,new(1, 1, "Produkt 1", "http://produkt.at", "Produkt 1", 10.0)},
                {2 ,new(2, 1, "Produkt 2", "http://produkt.at", "Produkt 2", 10.0)},
                {3 ,new (3, 1, "Produkt 3", "http://produkt.at", "Produkt 3", 10.0)},
                { 4 ,new(4, 1, "Produkt 4", "http://produkt.at", "Produkt 4", 10.0)},
                { 5 ,new(5, 1, "Produkt 5", "http://produkt.at", "Produkt 5", 10.0)},
                { 6 ,new(6, 1, "Produkt 6", "http://produkt.at", "Produkt 6", 10.0)}
            });


            discountCartRepository = new DiscountCartRepositoryStub(new List<DiscountCart>()
            {
                // valid 
                new DiscountCart(1, 1),
                new DiscountCart(1, 2),
                // not valid
                new DiscountCart(1, 3)
            });

            sut = new DiscountLogic(
                discountRepository, shopRepository, cartRepository,
                productCartRepository, discountCartRepository, productRepository,
                discountActionRepository, discountRuleRepository);
        }

        [Test]
        public async Task TestGetAvailableDiscountsByCartIdWithValidCartAndDiscountsAppliesDiscounts()
        {
            var discounts = (await sut.GetAvailableDiscountsByCartId(appKey, 1)).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(discounts, Has.Count.EqualTo(2));

                Assert.That(discounts[0].Id, Is.EqualTo(1));
                Assert.That(discounts[0].ActionId, Is.EqualTo(1));
                Assert.That(discounts[0].RuleId, Is.EqualTo(1));

                Assert.That(discounts[1].Id, Is.EqualTo(2));
                Assert.That(discounts[1].ActionId, Is.EqualTo(2));
                Assert.That(discounts[1].RuleId, Is.EqualTo(2));
            });
        }

        [Test]
        public void TestGetAvailableDiscountsByCartIdWithInvalidCartIdThrowsKeyNotFoundException()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(async ()
                => await sut.GetAvailableDiscountsByCartId(appKey, int.MaxValue));
        }

        [Test]
        public void TestGetAvailableDiscountsByCartIdWithoutCustomerIdThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async ()
                => await sut.GetAvailableDiscountsByCartId(appKey, 2));
        }


        [Test]
        public void TestAddDiscountsToCartWithoutCustomerIdThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async ()
                => await sut.AddDiscountsToCart(appKey, 2, new List<int>() { 1 }));
        }

        [Test]
        public void TestAddDiscountsToCartWhenCartIdIsInvalidThrowsKeyNotFoundException()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(async ()
                => await sut.AddDiscountsToCart(appKey, int.MaxValue, new List<int>() { 1 }));
        }


        [Test]
        public async Task TestAddDiscountsToCartWhenProductsFromCartAreMissingThrowsArgumentException()
        {
            var productCarts = await productCartRepository.GetByCartId(1);
            foreach (var protuctCart in productCarts)
                await productCartRepository.Delete(protuctCart.ProductId, protuctCart.CartId);

            Assert.ThrowsAsync<ArgumentException>(async ()
                => await sut.AddDiscountsToCart(appKey, 1, new List<int>() { 1 }));
        }

        [Test]
        public void TestAddDiscountsToCartWhenAppKeyIsInvalidThrowsUnauthorizedAccessException()
        {
            Assert.ThrowsAsync<UnauthorizedAccessException>(async ()
                => await sut.AddDiscountsToCart(Guid.NewGuid(), 1, new List<int>() { 1 }));
        }

        [Test]
        public void TestAddDiscountsToCartWhenDiscountIdsAreInvalidThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async ()
                => await sut.AddDiscountsToCart(appKey, 1, new List<int>() { int.MaxValue }));
        }

        [Test]
        public void TestAddDiscountsToCartWhenDiscountIdsAreEmptyThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async ()
                => await sut.AddDiscountsToCart(appKey, 1, new List<int>()));
        }

        [Test]
        public void TestAddDiscountsToCartWhenDiscountIdsAreNullThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async ()
                => await sut.AddDiscountsToCart(appKey, 1, null!));
        }

        [Test]
        public async Task TestAddDiscountsToCartWithValidDiscountIdsRemovesOldDiscounts()
        {
            await sut.AddDiscountsToCart(appKey, 1, new List<int>() { 1 });

            var actualDiscounts = await discountCartRepository.GetByCartId(1);
            Assert.That(actualDiscounts, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task TestAddDiscountsToCartWithValidDiscountIdsAddsDiscounts()
        {
            var cartDiscounts = await discountCartRepository.GetByCartId(1);
            foreach (var cartDiscount in cartDiscounts)
                await discountCartRepository.Delete(cartDiscount);

            await sut.AddDiscountsToCart(appKey, 1, new List<int>() { 1, 2 });

            var actualDiscounts = await discountCartRepository.GetByCartId(1);
            Assert.That(actualDiscounts, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task TestAddDiscountsToCartWithOneInvalidDiscountRuleAddsOnyValidDiscounts()
        {
            var cartDiscounts = await discountCartRepository.GetByCartId(1);
            foreach (var cartDiscount in cartDiscounts)
                await discountCartRepository.Delete(cartDiscount);

            // rule from discount 3 is not valid
            await sut.AddDiscountsToCart(appKey, 1, new List<int>() { 1, 2, 3 });

            var actualDiscounts = await discountCartRepository.GetByCartId(1);
            Assert.That(actualDiscounts, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task TestGetDiscountWithValidIdReturnsDiscount()
        {
            var discount = await sut.Get(appKey, 1);

            Assert.Multiple(() =>
            {
                Assert.That(discount, Is.Not.Null);
                Assert.That(discount.Id, Is.EqualTo(1));
            });
        }

        [Test]
        public void TestGetDiscountWithInvalidAppKeyThrowsUnauthorizedException()
        {
            Assert.CatchAsync<UnauthorizedAccessException>(async () => await sut.Get(Guid.NewGuid(), 1));
        }

        [Test]
        public void TestGetDiscountWithInvalidDiscountIdThrowsKeyNotFoundException()
        {
            Assert.CatchAsync<KeyNotFoundException>(async () => await sut.Get(appKey, int.MaxValue));
        }

        [Test]
        public async Task TestDeleteWithValidIdDeletesDiscount()
        {
            var isDeleted = await sut.Delete(appKey, 1);

            Assert.Multiple(() =>
            {
                Assert.That(isDeleted, Is.True);
                Assert.CatchAsync<KeyNotFoundException>(async () => await sut.Get(appKey, 1));
            });
        }

        [Test]
        public void TestDeleteWithInvalidIdThrowsKeyNotFoundException()
        {
            Assert.CatchAsync<KeyNotFoundException>(async () => await sut.Delete(appKey, int.MaxValue));
        }

        [Test]
        public void TestDeleteDiscountWithInvalidAppKeyThrowsUnauthorizedException()
        {
            Assert.CatchAsync<UnauthorizedAccessException>(async () => await sut.Delete(Guid.NewGuid(), 1));
        }

        [Test]
        public async Task TestCreateWithValidDiscountReturnsId()
        {
            Discount discount = new(4, 1, 1);

            var id = await sut.Create(appKey, discount);

            Discount actual = await sut.Get(appKey, id);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.RuleId, Is.EqualTo(discount.RuleId));
                Assert.That(actual.ActionId, Is.EqualTo(discount.ActionId));
            });
        }

        [Test]
        public void TestCreateWithInvalidActionThrowsKeyNotFoundException()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sut.Create(appKey, new(4, 1, int.MaxValue)));
        }

        [Test]
        public void TestCreateWithInvalidRuleThrowsKeyNotFoundException()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await sut.Create(appKey, new(4, int.MaxValue, 1)));
        }

        [Test]
        public async Task TestGetByShopIdWithValidShopIdReturnsDiscounts()
        {
            var discounts = (await sut.GetByShopId(appKey, 1)).ToList();

            Assert.That(discounts, Has.Count.EqualTo(3));
        }

        [Test]
        public void TestGetByShopIdWithInvalidShopIdThrowsUnauthorizedException()
        {
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.GetByShopId(Guid.NewGuid(), 1));
        }
    }
}
