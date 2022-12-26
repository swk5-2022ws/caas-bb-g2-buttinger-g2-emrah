using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Test.Util.MemoryRepositories;
using CaaS.Core.Test.Util.RepositoryStubs;
using Docker.DotNet.Models;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Logic
{
    public class OrderLogicTest
    {
        private IOrderLogic sut;
        private ICartRepository cartRepository;
        private IProductRepository productRepository;
        private IProductCartRepository productCartRepository;
        private ICustomerRepository customerRepository;
        private IShopRepository shopRepository;
        private IOrderRepository orderRepository;
        private ICouponRepository couponRepository;

        [SetUp]
        public async Task InitializeSutAsync()
        {            
            customerRepository = new CustomerRepositoryStub(new Dictionary<int, Customer>()
            {
                {1, new Customer(1, 1, "Test", "mail") },
                {2, new Customer(2, 2, "Test", "mail") },
                {3, new Customer(3, 1, "Test", "mail") },
                {4, new Customer(4, 1, "Test", "mail") },
                {5, new Customer(5, 1, "Test", "mail") },
                {6, new Customer(6, 1, "Test", "mail") },
                {7, new Customer(7, 1, "Test", "mail") },
                {8, new Customer(8, 1, "Test", "mail") },
                {9, new Customer(9, 3, "Test", "mail"){ 
                    CVV="+mY3fecYSQ0ivYVMkovf1UCOtQvD+2u9FLrBEXoNJuJOzgmYsH6WK6jZMbvs9pmm", 
                    CreditCardNumber="OrkRWqplepEtBD8m++7Cgnoc66VFVaAky7xs7NnbjeZOMzttjLSsOQf+7rYXr0va1Ydhlv7BAWAJ1lZyM3WGpQ==",
                    Expiration="RwvMfB/obNiIgHDM8kru60zdjRuGf162LDGjUCNDdeQiLEJIMVCy//RV3gTztC4W"
                } },
                {10, new Customer(10, 4, "Test", "mail"){ 
                    CreditCardNumber="OrkRWqplepEtBD8m++7Cgnoc66VFVaAky7xs7NnbjeZOMzttjLSsOQf+7rYXr0va1Ydhlv7BAWAJ1lZyM3WGpQ==",
                    Expiration="RwvMfB/obNiIgHDM8kru60zdjRuGf162LDGjUCNDdeQiLEJIMVCy//RV3gTztC4W"
                } },
                {11, new Customer(11, 4, "Test", "mail"){
                    CVV="+mY3fecYSQ0ivYVMkovf1UCOtQvD+2u9FLrBEXoNJuJOzgmYsH6WK6jZMbvs9pmm",
                    Expiration="RwvMfB/obNiIgHDM8kru60zdjRuGf162LDGjUCNDdeQiLEJIMVCy//RV3gTztC4W"
                } },
                {12, new Customer(12, 4, "Test", "mail"){
                    CVV="+mY3fecYSQ0ivYVMkovf1UCOtQvD+2u9FLrBEXoNJuJOzgmYsH6WK6jZMbvs9pmm",
                    CreditCardNumber="OrkRWqplepEtBD8m++7Cgnoc66VFVaAky7xs7NnbjeZOMzttjLSsOQf+7rYXr0va1Ydhlv7BAWAJ1lZyM3WGpQ=="
                } },
            });
            cartRepository = new CartRepositoryStub(new Dictionary<int, Cart>()
            {
                {1, new Cart(1, "test1") },
                {2, new Cart(2, "test2"){
                    CustomerId = 3,
                    Customer = await customerRepository.Get(3)
                }
                },
                {3, new Cart(3, "test3") },
                {4, new Cart(4, "test4") },
                {5, new Cart(5, "test5"){
                    CustomerId = 6,
                    Customer = await customerRepository.Get(6)
                }
                },
                {6, new Cart(6, "test6"){
                    CustomerId = -1
                }
                },
                {7, new Cart(7, "test7"){
                    CustomerId = 4,
                    Customer = await customerRepository.Get(5)
                }
                },
                {8, new Cart(8, "test8"){
                    CustomerId = 5,
                    Customer = await customerRepository.Get(5)
                } 
                },
                {9, new Cart(9, "test9") {
                    CustomerId = -1,
                    Customer = await customerRepository.Get(6)
                }
                },
                {10, new Cart(10, "test10") {
                    CustomerId = 1
                }
                },
                {11, new Cart(11, "test11") {
                    CustomerId = 7,
                    Customer = await customerRepository.Get(7),
                }
                },
                {12, new Cart(12, "test12") {
                    CustomerId = 9,
                    Customer = await customerRepository.Get(9),
                }
                }, 
                {13, new Cart(13, "test13") {
                    CustomerId = 10,
                    Customer = await customerRepository.Get(10),
                }
                }, {14, new Cart(14, "test14") {
                    CustomerId = 11,
                    Customer = await customerRepository.Get(11),
                }
                },
                {15, new Cart(15, "test15") {
                    CustomerId = 12,
                    Customer = await customerRepository.Get(12),
                }
                }

            });
            orderRepository = new OrderRepositoryStub(new Dictionary<int, Order>()
            {
                {1, new Order(1, 1, 10, DateTime.Now){ Cart = await cartRepository.Get(1) } },
                {2, new Order(2, 2, 0, new DateTime(2022,12,23)){ Cart = await cartRepository.Get(2) } },
                {3, new Order(3, 100, 0, new DateTime(2022,12,23)) },
                {4, new Order(4, 5, 0, DateTime.Now){ Cart = await cartRepository.Get(5) } },
                {5, new Order(5, 7, 0, DateTime.Now){ Cart = await cartRepository.Get(7) } },
                {6, new Order(6, 8, 0, DateTime.Now){ Cart = await cartRepository.Get(8) } },
                {7, new Order(7, 9, 0, DateTime.Now){ Cart = await cartRepository.Get(9) } },
                {8, new Order(8, 10, 0, new DateTime(2022, 12, 23)){ Cart = await cartRepository.Get(10) } },
                {9, new Order(9, 11, 0, new DateTime(2022, 12, 23)){ Cart = await cartRepository.Get(11) } },
                {10, new Order(10, 12, 0, new DateTime(2022, 12, 23)){ Cart = await cartRepository.Get(12) } },
                {11, new Order(11, 13, 0, new DateTime(2022, 12, 23)){ Cart = await cartRepository.Get(13) } },
                {12, new Order(12, 14, 0, new DateTime(2022, 12, 23)){ Cart = await cartRepository.Get(14) } },
                {13, new Order(13, 15, 0, new DateTime(2022, 12, 23)){ Cart = await cartRepository.Get(15) } },
            });
            productRepository = new ProductRepositoryStub(new Dictionary<int, Product>()
            {
                {1, new Product(1, 1, "test", "test", "test", 10) },
                {2, new Product(2, 1, "test", "test", "test", 20) },
                {3, new Product(3, 1, "test", "test", "test", 20) },
                {4, new Product(4, 1, "test", "test", "test", 20) },
                {5, new Product(5, 2, "test", "test", "test", 20) },
                {6, new Product(6, 2, "test", "test", "test", 20) },
                {7, new Product(7, 1, "test", "test", "test", 20) },
                {8, new Product(8, 3, "test", "test", "test", 20) },
                {9, new Product(9, 4, "test", "test", "test", 20) },
            });
            productCartRepository = new ProductCartRepositoryStub(new Dictionary<(int, int), ProductCart>()
            {
                {(1, 1), new ProductCart(1, 1, 10, 1) },
                {(2, 1), new ProductCart(2, 1, 20, 2) },
                {(2, 2), new ProductCart(2, 2, 20, 2) },
                {(5, 2), new ProductCart(5, 2, 20, 2) },
                {(3, 3), new ProductCart(3, 3, 20, 1) },
                {(4, 4), new ProductCart(4, 4, 20, 1) },
                {(10, 7), new ProductCart(10, 7, 20, 1) },
                {(6, 8), new ProductCart(6, 8, 20, 1) },
                {(7, 9), new ProductCart(7, 9, 20, 1) },
                {(7, 10), new ProductCart(7, 10, 20, 1) },
                {(7, 11), new ProductCart(7, 11, 20, 1) },
                {(9, 12), new ProductCart(8, 12, 20, 1) },
                {(9, 13), new ProductCart(9, 13, 20, 1) },
                {(9, 14), new ProductCart(9, 14, 20, 1) },
                {(9, 15), new ProductCart(9, 15, 20, 1) },
            });

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"), "shop") },
                {3, new Shop(3, 1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427901"), "shop") },
                {4, new Shop(4, 1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427900"), "shop") },
            });

            couponRepository = new CouponRepositoryStub(new Dictionary<int, Coupon>()
            {
                {1, new Coupon(1, 1, 10){ CouponKey = "testCoupon" } },
                {2, new Coupon(2, 1, 20){ CartId = 1, CouponKey="test" } },
                {3, new Coupon(3, 2, 20){ CouponKey="test2" } }
            });


            sut = new OrderLogic(orderRepository, cartRepository, couponRepository, productRepository, productCartRepository, customerRepository, shopRepository);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void GetOrderWithInvalidIdThrowsException(int id) =>
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.Get(id, Guid.NewGuid()));

        [Test]
        public void GetOrderWithInvalidCartIdThrowsException() =>
            Assert.CatchAsync<ArgumentOutOfRangeException>(async () => await sut.Get(3, Guid.NewGuid()));

        [Test]
        public void GetOrderWithNoProductsInCartThrowsException() =>
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.Get(4, Guid.NewGuid()));

        [Test]
        public void GetOrderWithNoProductsValidThrowsException() =>
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.Get(5, Guid.NewGuid()));

        [Test]
        public void GetOrderWithProductsFromMultipleShopsThrowsException() =>
            Assert.CatchAsync<ArgumentException>(async () => await sut.Get(2, Guid.NewGuid()));

        [Test]
        public void GetOrderWithInvalidShopIdThrowsException() =>
            Assert.CatchAsync<ArgumentException>(async () => await sut.Get(6, Guid.NewGuid()));

        [Test]
        public void GetOrderWithAppKeyThrowsException() =>
            Assert.CatchAsync<ArgumentException>(async () => await sut.Get(1, Guid.NewGuid()));
        [Test]
        public void GetOrderWithoutCustomerIdThrowsException() =>
            Assert.CatchAsync<KeyNotFoundException>(async () => await sut.Get(1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));
        
        [Test]
        public void GetOrderWithInvalidCustomerIdThrowsException() =>
            Assert.CatchAsync<ArgumentOutOfRangeException>(async () => await sut.Get(7, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        [TestCase(8, 0, 10, 2022, 12, 23)]
        public async Task GetOrderReturnsOrder(int id, double discount, int cartId, int year, int month, int day)
        {
            var order = await sut.Get(id, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"));

            BaseOrderAssertions(order, id, cartId, discount, year, month, day);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void GetOrderByCustomerIdWithInvalidCustomerIdThrowsException(int customerId) =>
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.GetByCustomerId(customerId, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public void GetOrderByCustomerIdWithInvalidShopIdThrowsException() =>
            Assert.CatchAsync<ArgumentException>(async () => await sut.GetByCustomerId(2, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));
        
        [Test]
        public void GetOrderByCustomerIdWithInvalidAppKeyThrowsException() =>
            Assert.CatchAsync<ArgumentException>(async () => await sut.GetByCustomerId(1, Guid.Parse("ac2724ba-ced5-32e8-9ada-17b06d427906")));
        [Test]
        public void GetOrderByCustomerIdWithEmptyProductCartsThrowsException() =>
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.GetByCustomerId(6, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));
        [Test]
        public void GetOrderByCustomerIdWithNoValidProductsThrowsException() =>
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.GetByCustomerId(4, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));
        [Test]
        public void GetOrderByCustomerIdWithProductsFromMultipleShopsThrowsException() =>
            Assert.CatchAsync<ArgumentException>(async () => await sut.GetByCustomerId(3, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));
        
        [Test]
        public void GetOrderByCustomerIdWithInvalidShopIdInCartThrowsException() =>
            Assert.CatchAsync<ArgumentException>(async () => await sut.GetByCustomerId(5, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));
        [Test]
        [TestCase(7, "a82724ba-ced5-32e8-9ada-17b06d427906", 9, 11, 0, 2022, 12, 23)]
        public async Task GetOrderByCustomerIdReturnsOrdersAsync(int customerId, string key, int orderId, int cartId, double discount, int year, int month, int day)
        {
            var orders = await sut.GetByCustomerId(customerId, Guid.Parse(key));
            Assert.That(orders, Is.Not.Null);
            Assert.That(orders.Count, Is.EqualTo(1));
            BaseOrderAssertions(orders[0], orderId, cartId, discount, year, month, day);

        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void GetOrderByShopIdWithInvalidIdThrowsException(int shopId) =>
            Assert.CatchAsync<ArgumentException>(async () => await sut.GetByShopId(shopId, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        public void GetOrderByShopIdWithInvalidAppKeyThrowsException() =>
            Assert.CatchAsync<ArgumentException>(async () => await sut.GetByShopId(1, Guid.Parse("ac2724ba-ced5-32e8-9ada-17b06d427906")));

        [Test]
        [TestCase(3, "a82724ba-ced5-32e8-9ada-17b06d427901", 10, 12, 0, 2022, 12, 23)]
        public async Task GetOrderByShopIdReturnsOrders(int shopId, string key, int orderId, int cartId, double discount, int year, int month, int day)
        {
            var orders = await sut.GetByShopId(shopId, Guid.Parse(key));
            Assert.That(orders, Is.Not.Null);
            Assert.That(orders.Count, Is.EqualTo(1));
            BaseOrderAssertions(orders[0], orderId, cartId, discount, year, month, day);
        }

        [Test]
        public void PayOrderWithoutCVVThrowsException() =>
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.Pay(11, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427900")));
        [Test]
        public void PayOrderWithoutExpirationThrowsException() =>
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.Pay(12, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427900")));
        [Test]
        public void PayOrderWithoutCreditCardNumberThrowsException() =>
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.Pay(13, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427900")));

        [Test]
        public async Task PayOrderReturnTrue() =>
            Assert.That(await sut.Pay(10, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427901")), Is.True);


        private void BaseOrderAssertions(Order order, int id, int cartId, double discount, int year, int month, int day)
        {
            Assert.That(order.Id, Is.EqualTo(id));
            Assert.That(order.Discount, Is.EqualTo(discount));
            Assert.That(order.CartId, Is.EqualTo(cartId));
            Assert.That(order.OrderDate, Is.EqualTo(new DateTime(year, month, day)));
            Assert.That(order.Cart, Is.Not.Null);
        }

    }
}
