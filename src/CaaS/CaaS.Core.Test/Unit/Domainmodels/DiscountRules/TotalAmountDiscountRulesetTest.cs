using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Unit.Domainmodels.DiscountRules
{
    public class TotalAmountDiscountRulesetTest
    {
        [TestCase(50.0)]
        [TestCase(100.0)]
        [Test]
        public void TestIsQUalifiedForDiscountWhenMinimumAmountIsEqualOrGreaterReturnsTrue(double price)
        {
            Cart cart = new(0, "id");
            cart.ProductCarts.Add(new ProductCart(new(0, 0, "", "", "", 2.0), price, 1));

            double minimumAmountToBeQualifiedForDiscount = 50.0;

            TotalAmountDiscountRuleset sut = new(minimumAmountToBeQualifiedForDiscount);
            sut.IsQualifiedForDiscount(cart);

            Assert.That(sut.IsQualifiedForDiscount(cart), Is.True);
        }

        [TestCase(49.0)]
        [TestCase(0.0)]
        [Test]
        public void TestIsQUalifiedForDiscountWhenMinimumAmountIsSmallerReturnsFalse(double price)
        {
            Cart cart = new(0, "id");
            cart.ProductCarts.Add(new ProductCart(new(0, 0, "", "", "", 2.0), price, 1));

            double minimumAmountToBeQualifiedForDiscount = 50.0;

            TotalAmountDiscountRuleset sut = new(minimumAmountToBeQualifiedForDiscount);
            sut.IsQualifiedForDiscount(cart);

            Assert.That(sut.IsQualifiedForDiscount(cart), Is.False);
        }

        [Test]
        public void TestCtorWhenParameterIsNegativThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new TotalAmountDiscountRuleset(-1.0));
        }
    }
}
