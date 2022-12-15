using AutoMapper;
using CaaS.Api.Controllers;
using CaaS.Api.Transfers;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Test;
using CaaS.Core.Test.Util;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Api.Test.Controllers
{
    public class CouponControllerTest
    {
        private CouponController sut;

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
            ICouponRepository couponRepository = new CouponRepository(Setup.GetTemplateEngine());
            ICouponLogic couponLogic = new CouponLogic(couponRepository, cartRepository, productCartRepository, productRepository, shopRepository);

            sut = new CouponController(couponLogic, mapper, logger);
        }

        [Test, Rollback]
        public async Task TestCreateCouponWithValidProductReturnsCreatedAtActionResult()
        {
            var createCoupon = new TCreateCoupon(1, 10);
            CreatedAtActionResult actionResult = (CreatedAtActionResult)await sut.Create(createCoupon, new Guid("a82724ba-ced5-32e8-9ada-17b06d427906"));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(201));
        }

        [Test, Rollback]
        public async Task TestCreateCouponWithInvalidShopIdReturnsBadRequest()
        {
            var createCoupon = new TCreateCoupon(-1, 10);
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Create(createCoupon, new Guid("a82724ba-ced5-32e8-9ada-17b06d427906"));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        public async Task TestCreateCouponWithInvalidAppKeyReturnsBadRequest()
        {
            var createCoupon = new TCreateCoupon(1, 10);
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Create(createCoupon, new Guid("c82724ba-ced5-32e8-9ada-17b06d427906"));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase("5c3b41d7-b823-45e3-b6d2-bdf749076687", "a82724ba-ced5-32e8-9ada-17b06d427906")]
        [TestCase("97e03e5c-9e07-4977-b0ec-6f9e8d8db335", "37f3184c-0dcd-3a58-975f-803648fcc73b")]
        [TestCase("b7eafb28-ae8d-4898-a466-8b8d011cd32e", "21de65da-b9f6-309f-973c-7fcbc36192cc")]
        public async Task TestDeleteCouponWithReferencedToCartReturnsBadRequest(string key, string guid)
        {
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Delete(key, new Guid(guid));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase("22e8ea91-cfd6-4e54-8f3a-e3f7559bd528", "7649eea8-89fd-345c-b797-46230be72d8d")]
        [TestCase("116fd4d0-7cde-489a-af08-940be1809182", "56ba8b49-e2a8-329e-96a9-cbb5ea49d7be")]
        [TestCase("374850f9-4700-4ce2-a793-7bac870c9440", "26d31f6e-16fd-38b9-b9ce-0319c626d301")]
        public async Task TestDeleteCouponWithoutReferenceReturnsNoContent(string key, string guid)
        {
            NoContentResult actionResult = (NoContentResult)await sut.Delete(key, new Guid(guid));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(204));
        }

        [Test]
        [TestCase(1, "a82724ba-ced5-32e8-9ada-17b06d427906", 0)]
        [TestCase(20, "7649eea8-89fd-345c-b797-46230be72d8d", 1)]
        public async Task TestGetListOfCouponsReturnsListOfCoupons(int id, string key, int count)
        {
            var actionResult = await sut.GetList(id, new Guid(key));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (OkObjectResult)actionResult.Result;
            var coupons = (List<TCoupon>?)result!.Value;

            Assert.That(coupons, Is.Not.Null);
            Assert.That(coupons.Count, Is.EqualTo(count));

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task TestGetListOfcouponsReturnsBadRequest()
        {
            var actionResult = await sut.GetList(-1, Guid.NewGuid());

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (BadRequestObjectResult)actionResult.Result;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase("22e8ea91-cfd6-4e54-8f3a-e3f7559bd528", "7649eea8-89fd-345c-b797-46230be72d8d")]
        [TestCase("116fd4d0-7cde-489a-af08-940be1809182", "56ba8b49-e2a8-329e-96a9-cbb5ea49d7be")]
        [TestCase("374850f9-4700-4ce2-a793-7bac870c9440", "26d31f6e-16fd-38b9-b9ce-0319c626d301")]
        public async Task TestApplyWithWrongCartIdReturnsBadRequest(string key, string appKey)
        {
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Apply(-1, key, new Guid(appKey));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase("22e8ea91-cfd6-4e54-8f3a-e3f7559bd528", "7649eea8-89fd-345c-b797-46230be72d8d", 1)]
        [TestCase("116fd4d0-7cde-489a-af08-940be1809182", "56ba8b49-e2a8-329e-96a9-cbb5ea49d7be", 2)]
        [TestCase("374850f9-4700-4ce2-a793-7bac870c9440", "26d31f6e-16fd-38b9-b9ce-0319c626d301", 3)]
        public async Task TestApplyWithDifferentCartIdsReturnsBadRequest(string key, string appKey, int cartId)
        {
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Apply(cartId, key, new Guid(appKey));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase("22e8ea91-cfd6-4e54-8f3a-e3f7559bd528", "7649eea8-89fd-345c-b797-46230be72d8d", 20)]
        [TestCase("116fd4d0-7cde-489a-af08-940be1809182", "56ba8b49-e2a8-329e-96a9-cbb5ea49d7be", 19)]
        [TestCase("374850f9-4700-4ce2-a793-7bac870c9440", "26d31f6e-16fd-38b9-b9ce-0319c626d301", 18)]
        public async Task TestApplyWithDifferentCartIdsReturnsOkRequest(string key, string appKey, int cartId)
        {
            var actionResult = (NoContentResult)await sut.Apply(cartId, key, new Guid(appKey));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(204));
        }

    }
}
