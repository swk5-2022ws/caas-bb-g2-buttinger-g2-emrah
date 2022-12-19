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
        IDiscountLogic sut;
        ICartRepository cartRepository;
        IShopRepository shopRepository;
        IDiscountRepository discountRepository;

        private static readonly Guid appKey = Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906");

        [SetUp]
        public void InitializeSut()
        {
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


            });

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

            sut = new DiscountLogic(discountRepository, shopRepository, cartRepository);
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
                => await sut.GetAvailableDiscountsByCartId(Guid.Empty, 2));
        }
    }
}
