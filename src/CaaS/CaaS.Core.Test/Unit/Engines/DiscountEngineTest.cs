using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Engines;
using CaaS.Core.Interfaces.Engines;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Unit.Engines
{
    [Category("Unit")]
    [TestFixture]
    public class DiscountEngineTest
    {
        [Test]
        public void TestApplyDateDiscountRulesetAddsDiscountstoCart()
        {
            List<Discount> discounts = new List<Discount>
            {
                // valid
                 new Discount(1,
                    new DiscountRule(3, 1, "",
                        new DateDiscountRuleset(DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(5), DateTime.Now)),
                    new DiscountAction(3, 1, "",
                        new FixedValueDiscountAction(100.0)
                    )
                ),
                new Discount(1,
                    new DiscountRule(1, 1, "",
                        new DateDiscountRuleset(DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1), DateTime.Now)),
                    new DiscountAction(1, 1, "",
                        new FixedValueDiscountAction(100.0)
                    )
               ),
                // not valid
                new Discount(1,
                    new DiscountRule(2, 1, "",
                        new DateDiscountRuleset(DateTime.Now.AddMinutes(-3), DateTime.Now.AddMinutes(-2), DateTime.Now)),
                    new DiscountAction(2, 1, "",
                        new FixedValueDiscountAction(100.0)
                    )
               )
            };

            Cart cart = new(1, "id");
            cart.ProductCarts.Add(new ProductCart(new(1, 1, "", "", "", 2.0), 100.0, 2));
            cart.ProductCarts.Add(new ProductCart(new(2, 1, "", "", "", 2.0), 100.0, 1));

            IDiscountEngine sut = new DiscountEngine(discounts);
            sut.ApplyValidDiscounts(cart);
            Assert.That(cart.Discounts, Has.Count.EqualTo(2));
        }

        [Test]
        public void TestApplyFixedValueDiscountRulesetAddsDiscountstoCart()
        {
            List<Discount> discounts = new List<Discount>
            {
                // valid
                 new Discount(1,
                    new DiscountRule(3, 1, "",
                        new TotalAmountDiscountRuleset(100.0)),
                    new DiscountAction(3, 1, "",
                        new FixedValueDiscountAction(100.0)
                    )
                ),
                new Discount(1,
                    new DiscountRule(1, 1, "",
                        new TotalAmountDiscountRuleset(300.0)),
                    new DiscountAction(1, 1, "",
                        new FixedValueDiscountAction(100.0)
                    )
               ),
                // not valid
                new Discount(1,
                    new DiscountRule(2, 1, "",
                        new TotalAmountDiscountRuleset(500.0)),
                    new DiscountAction(2, 1, "",
                        new FixedValueDiscountAction(100.0)
                    )
               )
            };

            Cart cart = new(1, "id");
            cart.ProductCarts.Add(new ProductCart(new(1, 1, "", "", "", 2.0), 100.0, 2));
            cart.ProductCarts.Add(new ProductCart(new(2, 1, "", "", "", 2.0), 100.0, 1));

            IDiscountEngine sut = new DiscountEngine(discounts);
            sut.ApplyValidDiscounts(cart);
            Assert.That(cart.Discounts, Has.Count.EqualTo(2));
        }

        [Test]
        public void TestCalculateDiscountPriceWithValidDiscountsReturnsDiscountedPrice()
        {
            List<Discount> discounts = new List<Discount>
            {
                 new Discount(1,
                    new DiscountRule(3, 1, "",
                        new TotalAmountDiscountRuleset(100.0)),
                    new DiscountAction(3, 1, "",
                        new FixedValueDiscountAction(100.0)
                    )
                ),
                new Discount(1,
                    new DiscountRule(1, 1, "",
                        new TotalAmountDiscountRuleset(300.0)),
                    new DiscountAction(1, 1, "",
                        new FixedValueDiscountAction(100.0)
                    )
               
               )
            };

            IDiscountEngine sut = new DiscountEngine(discounts);

            Cart cart = new(1, "id");
            cart.ProductCarts.Add(new ProductCart(new(1, 1, "", "", "", 2.0), 100.0, 10));
            sut.ApplyValidDiscounts(cart);

            var actual = sut.CalculateDiscountPrice(cart);
            var expected = (1000.0 - 100.0) - 100.0;
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestApplyValidDiscountsWithNullCartThrowsArgumentException()
        {
            IDiscountEngine sut = new DiscountEngine(new List<Discount>());
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentException>(() => sut.ApplyValidDiscounts(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

    }
}
