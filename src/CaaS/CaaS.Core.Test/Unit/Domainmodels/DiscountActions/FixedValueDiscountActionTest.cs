using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountActions;
using DotNet.Testcontainers.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Unit.Domainmodels.DiscountAction
{
    public class FixedValueDiscountActionTest
    {
        [TestCase(10.0, 10.0f, 10.0)]
        [TestCase(10.0, 100.0f, 10.0)]
        [Test]
        public void TestGetDiscountWithValidFixedValueReturnsDifference(double cartPrice, float discountValue, double expected)
        {
            Cart cart = new(0, "id");
            cart.ProductCarts.Add(new ProductCart(new(0, 0, "", "", "", 2.0), cartPrice, 1));

            FixedValueDiscountAction sut = new(discountValue);
            double actual = sut.GetDiscount(cart);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(10.0, 15.0f)]
        [TestCase(0.0, 10.0f)]
        [Test]
        public void TestGetDiscountWithFixedValueBiggerThanCartPriceReturnsCartPrice(double cartPrice, float discountValue)
        {
            Cart cart = new(0, "id");
            cart.ProductCarts.Add(new ProductCart(new(0, 0, "", "", "", 2.0), cartPrice, 1));

            FixedValueDiscountAction sut = new(discountValue);
            double actual = sut.GetDiscount(cart);

            Assert.That(actual, Is.EqualTo(cart.Price));
        }

        [Test]
        public void TestCtorWhenParameterIsNegativeThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new FixedValueDiscountAction(-1));
        }
    }
}
