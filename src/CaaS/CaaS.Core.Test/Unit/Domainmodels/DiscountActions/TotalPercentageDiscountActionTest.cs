using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Unit.Domainmodels.DiscountActions
{
    [Category("Unit")]
    [TestFixture]
    public class TotalPercentageDiscountActionTest
    {
        [TestCase(1.1)]
        [TestCase(-0.1)]
        [Test]
        public void TestCtorWhenValueIsOutOfRangeThrowsArgumentException(double discountPercentage)
        {
            Assert.Throws<ArgumentException>(() => new TotalPercentageDiscountAction(discountPercentage));
        }

        [TestCase(0.1, 1.0, 0.1)]
        [TestCase(1.0, 1.0, 1.0)]
        [TestCase(0.5, 1000.0, 500.0)]
        [Test]
        public void TestGetDiscountWithValidValuesReturnsDiscount(double discountPercentage, double cartPrice, double expected)
        {
            Cart cart = new(0, "id");
            cart.ProductCarts.Add(new ProductCart(new(0, 0, "", "", "", 2.0), cartPrice, 1));
            TotalPercentageDiscountAction sut = new(discountPercentage);
            double actual = sut.GetDiscount(cart);

            Assert.That(actual, Is.EqualTo(expected));
        }

    }
}

