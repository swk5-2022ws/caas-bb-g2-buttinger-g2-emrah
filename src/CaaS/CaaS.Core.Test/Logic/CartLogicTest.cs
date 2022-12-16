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
                {1, new Customer(1, 1, "Test", "mail") },
            });
            cartRepository = new CartRepositoryStub(new Dictionary<int, Cart>()
            {
                {1, new Cart(1, "test1") },
                {2, new Cart(2, "test2") },
                {3, new Cart(3, "test3") },
                {4, new Cart(4, "test4") },
                {5, new Cart(5, "test5"){
                    CustomerId = 1
                }
                },
                {6, new Cart(6, "test6"){
                    CustomerId = -1
                }
                }
            });
            productRepository = new ProductRepositoryStub(new Dictionary<int, Product>()
            {
                {1, new Product(1, 1, "test", "test", "test", 10) },
                {2, new Product(2, 1, "test", "test", "test", 20) },
                {3, new Product(3, 1, "test", "test", "test", 20) },
                {4, new Product(4, 1, "test", "test", "test", 20) },
                {5, new Product(5, 2, "test", "test", "test", 20) },
            });
            productCartRepository = new ProductCartRepositoryStub(new Dictionary<(int, int), ProductCart>()
            {
                {(1, 1), new ProductCart(1, 1, 10, 1) },
                {(2, 1), new ProductCart(2, 1, 20, 2) },
                {(2, 2), new ProductCart(2, 2, 20, 2) },
                {(5, 2), new ProductCart(5, 2, 20, 2) },
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
        public async Task CreateCart() => Assert.That(await sut.Create(), Is.EqualTo(7));

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
            Assert.That(createdId, Is.EqualTo(7));
        }

        [Test, Rollback]
        [TestCase("nope", 1, "a82724ba-ced5-32e8-9ada-17b06d427906", null)]
        public void DeleteProductFromCartWithInvalidSessionIdReturnsException(string sessionId, int productId, string appKey, uint? amount) =>
            Assert.CatchAsync(async () => await sut.DeleteProductFromCart(sessionId, productId, Guid.Parse(appKey), amount));
        
        [Test, Rollback]
        [TestCase("test1", -1, "a82724ba-ced5-32e8-9ada-17b06d427906", null)]
        public void DeleteProductFromCartWithInvalidProductIdReturnsException(string sessionId, int productId, string appKey, uint? amount) =>
            Assert.CatchAsync(async () => await sut.DeleteProductFromCart(sessionId, productId, Guid.Parse(appKey), amount));
        
        [Test, Rollback]
        [TestCase("test1", 1, "c82724ba-ced5-32e8-9ada-17b06d427906", null)]
        public void DeleteProductFromCartWithInvalidAppKeyReturnsException(string sessionId, int productId, string appKey, uint? amount) =>
            Assert.CatchAsync(async () => await sut.DeleteProductFromCart(sessionId, productId, Guid.Parse(appKey), amount));

        [Test, Rollback]
        [TestCase("test1", 1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void DeleteProductFromCartWithInvalidAppKeyReturnsException(string sessionId, int productId, string appKey) =>
            Assert.CatchAsync(async () => await sut.DeleteProductFromCart(sessionId, productId, Guid.Parse(appKey), 0));

        [Test, Rollback]
        [TestCase("test1", 3, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void DeleteProductFromCartWithInvalidReferenceReturnsException(string sessionId, int productId, string appKey) =>
            Assert.CatchAsync(async () => await sut.DeleteProductFromCart(sessionId, productId, Guid.Parse(appKey), 1));
        
        [Test, Rollback]
        [TestCase("test1", 1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public async Task DeleteProductFromCartWithEmptyAmountReturnCartWithoutGivenProduct(string sessionId, int productId, string appKey)
        {
            var deleted = await sut.DeleteProductFromCart(sessionId, productId, Guid.Parse(appKey), null);
            Assert.That(deleted, Is.True);
            var cart = await sut.Get(sessionId, Guid.Parse(appKey));

            Assert.NotNull(cart);
            Assert.That(cart.ProductCarts.Count, Is.EqualTo(1));
        }
        
        [Test, Rollback]
        [TestCase("test1", 2, "a82724ba-ced5-32e8-9ada-17b06d427906", (uint)1)]
        public async Task DeleteProductFromCartWithSpecificAmounttReturnCartWithProduct(string sessionId, int productId, string appKey, uint amount)
        {
            var deleted = await sut.DeleteProductFromCart(sessionId, productId, Guid.Parse(appKey), amount);
            Assert.That(deleted, Is.True);
            var cart = await sut.Get(sessionId, Guid.Parse(appKey));

            Assert.NotNull(cart);
            Assert.That(cart.ProductCarts.Count, Is.EqualTo(2));
        }
        
        [Test, Rollback]
        [TestCase("test1", 2, "a82724ba-ced5-32e8-9ada-17b06d427906", (uint)2)]
        [TestCase("test1", 2, "a82724ba-ced5-32e8-9ada-17b06d427906", (uint)3)]
        public async Task DeleteProductFromCartWithFullAmountReturnCartWithoutProduct(string sessionId, int productId, string appKey, uint amount)
        {
            var deleted = await sut.DeleteProductFromCart(sessionId, productId, Guid.Parse(appKey), amount);
            Assert.That(deleted, Is.True);
            var cart = await sut.Get(sessionId, Guid.Parse(appKey));

            Assert.NotNull(cart);
            Assert.That(cart.ProductCarts.Count, Is.EqualTo(1));
        }

        [Test, Rollback]
        [TestCase("test1", -1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void ReferenceCustomerToCartWithInvalidCustomerIdReturnsException(string sessionId, int customerId, string appKey) =>
            Assert.CatchAsync(async () => await sut.ReferenceCustomerToCart(customerId, sessionId, Guid.Parse(appKey)));
        
        [Test, Rollback]
        [TestCase("nop2", 1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void ReferenceCustomerToCartWithInvalidSessionIdReturnsException(string sessionId, int customerId, string appKey) =>
            Assert.CatchAsync(async () => await sut.ReferenceCustomerToCart(customerId, sessionId, Guid.Parse(appKey)));
        
        [Test, Rollback]
        [TestCase("test5", 5, "c82724ba-ced5-32e8-9ada-17b06d427906")]
        public void ReferenceCustomerToCartWithInvalidAppKeyReturnsException(string sessionId, int customerId, string appKey) =>
            Assert.CatchAsync(async () => await sut.ReferenceCustomerToCart(customerId, sessionId, Guid.Parse(appKey))); 
        
        [Test, Rollback]
        [TestCase("test5", 1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void ReferenceCustomerToCartWithAlreadyReferencedCustomerIdReturnsException(string sessionId, int customerId, string appKey) =>
            Assert.CatchAsync(async () => await sut.ReferenceCustomerToCart(customerId, sessionId, Guid.Parse(appKey)));

        [Test, Rollback]
        [TestCase("test1", 1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public async Task ReferenceCustomerToCartReturnsTrue(string sessionId, int customerId, string appKey)
        {
            Assert.That(await sut.ReferenceCustomerToCart(customerId, sessionId, Guid.Parse(appKey)), Is.True);

            var cart = await sut.Get(sessionId, Guid.Parse(appKey));
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.CustomerId, Is.Not.Null);
            Assert.That(cart.CustomerId.Value, Is.EqualTo(customerId));
        }

        [Test, Rollback]
        [TestCase("nope", 1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void ReferenceProductToCartWithInvalidSessionIdException(string sessionId, int productId, string appKey) =>
           Assert.CatchAsync(async () => await sut.ReferenceProductToCart(sessionId, productId, Guid.Parse(appKey), null));
        
        [Test, Rollback]
        [TestCase("test1", -1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void ReferenceProductToCartWithInvalidProductIdException(string sessionId, int productId, string appKey) =>
           Assert.CatchAsync(async () => await sut.ReferenceProductToCart(sessionId, productId, Guid.Parse(appKey), null));
        
        [Test, Rollback]
        [TestCase("test1", 1, "c82724ba-ced5-32e8-9ada-17b06d427906")]
        public void ReferenceProductToCartWithInvalidAppKeyException(string sessionId, int productId, string appKey) =>
           Assert.CatchAsync(async () => await sut.ReferenceProductToCart(sessionId, productId, Guid.Parse(appKey), null));
        
        [Test, Rollback]
        [TestCase("test1", 1, "a82724ba-ced5-32e8-9ada-17b06d427906", null)]
        [TestCase("test1", 1, "a82724ba-ced5-32e8-9ada-17b06d427906", (uint)0)]
        public async Task ReferenceProductToCartReturnsTrueWithExistingProduct(string sessionId, int productId, string appKey, uint? amount)
        {
            Assert.That(await sut.ReferenceProductToCart(sessionId, productId, Guid.Parse(appKey), amount), Is.True);
            var cart = await sut.Get(sessionId, Guid.Parse(appKey));
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.ProductCarts.Count, Is.EqualTo(2));
            Assert.That(cart.ProductCarts.First(x => x.ProductId == productId).Amount, Is.EqualTo(2));
        }
        
        [Test, Rollback]
        [TestCase("test1", 3, "a82724ba-ced5-32e8-9ada-17b06d427906", (uint)2)]
        [TestCase("test1", 3, "a82724ba-ced5-32e8-9ada-17b06d427906", (uint)3)]
        public async Task ReferenceProductToCartReturnsTrueWithNewProduct(string sessionId, int productId, string appKey, uint? amount)
        {
            Assert.That(await sut.ReferenceProductToCart(sessionId, productId, Guid.Parse(appKey), amount), Is.True);
            var cart = await sut.Get(sessionId, Guid.Parse(appKey));
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.ProductCarts.Count, Is.EqualTo(3));
            Assert.That(cart.ProductCarts.First(x => x.ProductId == productId).Amount, Is.EqualTo(amount));
        }

        [Test, Rollback]
        [TestCase("nope", "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void GetCartWithInvalidSessionIdReturnsException(string sessionId, string appKey) =>
           Assert.CatchAsync(async () => await sut.Get(sessionId, Guid.Parse(appKey)));

        [Test, Rollback]
        [TestCase("test5", "c82724ba-ced5-32e8-9ada-17b06d427906")]
        public void GetCartWithInvalidAppKeyReturnsException(string sessionId, string appKey) =>
           Assert.CatchAsync(async () => await sut.Get(sessionId, Guid.Parse(appKey)));
        
        [Test, Rollback]
        [TestCase("test6", "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void GetCartWithInvalidCustomerIdReturnsException(string sessionId, string appKey) =>
           Assert.CatchAsync(async () => await sut.Get(sessionId, Guid.Parse(appKey)));

        [Test, Rollback]
        [TestCase("test2", "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public void GetCartWithProductsFromMultipleShopsReturnsException(string sessionId, string appKey) =>
           Assert.CatchAsync(async () => await sut.Get(sessionId, Guid.Parse(appKey)));

        [Test, Rollback]
        [TestCase("test1", "c82724ba-ced5-32e8-9ada-17b06d427906")]
        public void GetCartWithProductsAndInvalidAppKeyReturnsException(string sessionId, string appKey) =>
           Assert.CatchAsync(async () => await sut.Get(sessionId, Guid.Parse(appKey)));

        [Test, Rollback]
        [TestCase("test5", "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public async Task GetCartWithoutProductsReturnscart(string sessionId, string appKey)
        {
            var cart = await sut.Get(sessionId, Guid.Parse(appKey));
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.ProductCarts.Count, Is.EqualTo(0));
            Assert.That(cart.SessionId, Is.EqualTo(sessionId));
        }

        [Test, Rollback]
        [TestCase("test1", "a82724ba-ced5-32e8-9ada-17b06d427906")]
        public async Task GetCartWithProductsReturnscart(string sessionId, string appKey)
        {
            var cart = await sut.Get(sessionId, Guid.Parse(appKey));
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.ProductCarts.Count, Is.EqualTo(2));
            Assert.That(cart.SessionId, Is.EqualTo(sessionId));
            Assert.That(cart.ProductCarts.First().Product, Is.Not.Null);
            Assert.That(cart.ProductCarts.Last().Product, Is.Not.Null);
        }
    }
}
