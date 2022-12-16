using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;
using CaaS.Core.Test.Util.MemoryRepositories;
using CaaS.Core.Test.Util.RepositoryStubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Logic
{
    public class CartLogicTest
    {
        private ICartLogic sut;
        private ICartRepository cartRepository;
        private IProductRepository productRepository;
        private IProductCartRepository productCartRepository;
        private ICustomerRepository customerRepository;
        private IShopRepository shopRepository;

        [SetUp]
        public void InitializeSut()
        {
            customerRepository = new CustomerRepositoryStub(new Dictionary<int, Customer>()
            {
                {1, new Customer(1, 1, "Test", "mail") }
            });
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

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906"), "shop") }
            });


            sut = new CartLogic(cartRepository, productRepository, productCartRepository, customerRepository, shopRepository);
        }

        [Test, Rollback]
        public async Task CreateCart() => Assert.That(await sut.Create(), Is.EqualTo(5));

        [Test, Rollback]
        public void CreateCartForCustomerWithInvalidCustomerIdReturnsException() =>
            Assert.CatchAsync(async () => await sut.CreateCartForCustomer(-1, Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906")));

        [Test, Rollback]
        public void CreateCartForCustomerWithInvalidAppKeyReturnsException() =>
            Assert.CatchAsync(async () => await sut.CreateCartForCustomer(1, Guid.NewGuid()));

        [Test, Rollback]
        [TestCase(1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public async Task CreateCartForCustomerReturnsCart(int customerId, string appKey)
        {
            var createdId = await sut.CreateCartForCustomer(customerId, Guid.Parse(appKey));
            Assert.That(createdId, Is.EqualTo(5));

        }
    }
}
