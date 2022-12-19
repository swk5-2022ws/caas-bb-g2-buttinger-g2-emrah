using AutoMapper;
using CaaS.Api.Controllers;
using CaaS.Api.Transfers;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Test;
using CaaS.Core.Test.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Api.Test.Controllers
{
    public class CartControllerTest
    {
        private CartController sut;

        [OneTimeSetUp]
        public void InitializeSut()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<CouponController> logger = loggerFactory.CreateLogger<CouponController>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(ProductController));
            });
            var mapper = mockMapper.CreateMapper();


            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());
            IProductRepository productRepository = new ProductRepository(Setup.GetTemplateEngine());
            IProductCartRepository productCartRepository = new ProductCartRepository(Setup.GetTemplateEngine());
            ICartRepository cartRepository = new CartRepository(Setup.GetTemplateEngine());
            ICustomerRepository customerRepository = new CustomerRepository(Setup.GetTemplateEngine());
            ICartLogic cartLogic = new CartLogic(cartRepository, productRepository, productCartRepository, customerRepository, shopRepository);

            sut = new CartController(cartLogic, mapper, logger);
        }

        [Test]
        [TestCase("7ee2dcbd-8e42-366d-9919-b96d65afd844", "a82724ba-ced5-32e8-9ada-17b06d427906", 50)]
        [TestCase("747c7000-c0b2-330a-930a-1d14e39b1e64", "37f3184c-0dcd-3a58-975f-803648fcc73b", 50)]
        public async Task TestGetCartReturnsCart(string sessionId, string key, int count)
        {
            var actionResult = await sut.Get(sessionId, new Guid(key));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (OkObjectResult)actionResult.Result;
            var cart = result!.Value as TCart;

            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.SessionId, Is.EqualTo(sessionId));
            Assert.That(cart.ProductCarts.Count, Is.EqualTo(count));
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        [TestCase("7ee2dcbd-8e42-366d-9919-b96d65afd844", "c82724ba-ced5-32e8-9ada-17b06d427906")]
        [TestCase("747c7000-c0b2-330a-930a-1d14e39b1e64", "27f3184c-0dcd-3a58-975f-803648fcc73b")]
        public async Task TestGetCartReturnsBadRequest(string sessionId, string key)
        {
            var actionResult = await sut.Get(sessionId, new Guid(key));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (BadRequestObjectResult)actionResult.Result;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase("a82724ba-ced5-32e8-9ada-17b06d427906")]
        [TestCase("37f3184c-0dcd-3a58-975f-803648fcc73b")]
        public async Task TestCreateCartReturnsCreatedAtActionResult(string key)
        {
            CreatedAtActionResult actionResult = (CreatedAtActionResult)(await sut.Create(new Guid(key)));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(201));
        }

        [Test, Rollback]
        [TestCase(1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        [TestCase(2, "37f3184c-0dcd-3a58-975f-803648fcc73b")]
        public async Task TestCreateCartForCustomerReturnsCreatedAtActionResult(int customerId, string key)
        {
            CreatedAtActionResult actionResult = (CreatedAtActionResult)(await sut.Create(customerId, new Guid(key)));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(201));
        }

        [Test, Rollback]
        [TestCase(1, "282724ba-ced5-32e8-9ada-17b06d427906")]
        [TestCase(2, "17f3184c-0dcd-3a58-975f-803648fcc73b")]
        public async Task TestCreateCartForCustomerReturnsBadRequest(int customerId, string key)
        {
            BadRequestObjectResult actionResult = (BadRequestObjectResult)(await sut.Create(customerId, new Guid(key)));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase("7ee2dcbd-8e42-366d-9919-b96d65afd844", "37f3184c-0dcd-3a58-975f-803648fcc73b", 2, 1)]
        [TestCase("747c7000-c0b2-330a-930a-1d14e39b1e64", "37f3184c-0dcd-3a58-975f-803648fcc73b", 2, 1)]
        public async Task TestReferenceProductToCartReturnsNoContentResult(string sessionId, string key, int productId, int amount)
        {
            var actionResult = (NoContentResult)(await sut.ReferenceProduct(sessionId, productId, (uint)amount, new Guid(key)));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(204));
        }

        [Test, Rollback]
        [TestCase("7ee2dcbd-8e42-366d-9919-b96d65afd844", "a82724ba-ced5-32e8-9ada-17b06d427906", -1, 1)]
        [TestCase("747c7000-c0b2-330a-930a-1d14e39b1e64", "37f3184c-0dcd-3a58-975f-803648fcc73b", -100, 1)]
        public async Task TestReferenceProductToCartReturnsBadRequest(string sessionId, string key, int productId, int amount)
        {
            var actionResult = (BadRequestObjectResult)(await sut.ReferenceProduct(sessionId, productId, (uint)amount, new Guid(key)));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase("747c7000-c0b2-330a-930a-1d14e39b1e64", "37f3184c-0dcd-3a58-975f-803648fcc73b", 2, 1)]
        public async Task TestDeleteProductToCartReturnsNoContentResult(string sessionId, string key, int productId, int amount)
        {
            var actionResult = (NoContentResult)(await sut.Delete(sessionId, productId, (uint)amount, new Guid(key)));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(204));
        }

        [Test, Rollback]
        [TestCase("7ee2dcbd-8e42-366d-9919-b96d65afd844", "a82724ba-ced5-32e8-9ada-17b06d427906", -1, 1)]
        [TestCase("747c7000-c0b2-330a-930a-1d14e39b1e64", "37f3184c-0dcd-3a58-975f-803648fcc73b", -100, 1)]
        public async Task TestDeleteProductToCartReturnsBadRequest(string sessionId, string key, int productId, int amount)
        {
            var actionResult = (BadRequestObjectResult)(await sut.Delete(sessionId, productId, (uint)amount, new Guid(key)));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase("a82724ba-ced5-32e8-9ada-17b06d427906", 1)]
        [TestCase("37f3184c-0dcd-3a58-975f-803648fcc73b", 2)]
        public async Task TestReferenceCartToCartReturnsNoContentResult(string key, int customerId)
        {
            var resultCart = (CreatedAtActionResult)(await sut.Create(Guid.Parse(key)));
            Assert.That(resultCart, Is.Not.Null);
            Assert.That(resultCart.RouteValues, Is.Not.Null);
            Assert.That(resultCart.RouteValues["sessionId"], Is.Not.Null);
            var actionResult = (NoContentResult)(await sut.ReferenceCustomer(customerId, resultCart!.RouteValues["sessionId"]!.ToString(), new Guid(key)));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(204));
        }
        
        [Test, Rollback]
        [TestCase("c82724ba-ced5-32e8-9ada-17b06d427906", 1)]
        [TestCase("27f3184c-0dcd-3a58-975f-803648fcc73b", 2)]
        public async Task TestReferenceCartToCartReturnsBadRequestResult(string key, int customerId)
        {
            var resultCart = (CreatedAtActionResult)(await sut.Create(Guid.Parse(key)));
            Assert.That(resultCart, Is.Not.Null);
            Assert.That(resultCart.RouteValues, Is.Not.Null);
            Assert.That(resultCart.RouteValues["sessionId"], Is.Not.Null);
            var actionResult = (BadRequestObjectResult)(await sut.ReferenceCustomer(customerId, resultCart!.RouteValues["sessionId"]!.ToString(), new Guid(key)));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

    }
}
