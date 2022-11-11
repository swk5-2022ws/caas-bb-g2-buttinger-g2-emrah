using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Unit.Domainmodels
{
    public class CartTest
    {
        [Test]
        public void TestGetPriceWithValidItemsReturnsPrice1()
        {
            Cart cart = new(0, "id");
            cart.ProductCarts.Add(new ProductCart(new Product(0, 0, "", "", "", 2.0),0, 1.0, 1));
            cart.ProductCarts.Add(new ProductCart(new Product(1, 1, "", "", "", 2.0),0, 1.0, 1));

            Assert.That(cart.Price, Is.EqualTo(1.0 * 1 + 1.0 * 1));
        }

        [Test]
        public void TestGetPriceWithValidItemsReturnsPrice2()
        {
            Cart cart = new(0, "id");
            cart.ProductCarts.Add(new ProductCart(new Product(0, 0, "", "", "", 2.0),0, 1.0, 2));
            cart.ProductCarts.Add(new ProductCart(new Product(1, 0, "", "", "", 2.0),0, 1.0, 1));

            Assert.That(cart.Price, Is.EqualTo(1.0 * 2 + 1.0 * 1));
        }

        [Test]
        public void TestGetPriceWithNegativePriceThrowsArgumentException()
        {
            Cart cart = new(0, "id");
            cart.ProductCarts.Add(new ProductCart(new Product(0, 0, "", "", "", 2.0),0, -1.0, 2));
            cart.ProductCarts.Add(new ProductCart(new Product(1, 0, "", "", "", 2.0),0, 1.0, 1));

            Assert.Throws<ArgumentException>(() => _ = cart.Price);
        }
    }
}
