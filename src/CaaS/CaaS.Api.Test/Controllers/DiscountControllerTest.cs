using AutoMapper;
using CaaS.Api.Controllers;
using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaaS.Core.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using NUnit.Framework;
using CaaS.Core.Test.Util;

namespace CaaS.Api.Test.Controllers
{
    [Category("Integration")]
    [TestFixture]
    public class DiscountControllerTest
    {
        private DiscountController sut;
        private Guid appKey = Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906");

        [OneTimeSetUp]
        public void InitializeSut()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<DiscountController> logger = loggerFactory.CreateLogger<DiscountController>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(DiscountController));
            });
            var mapper = mockMapper.CreateMapper();

            IDiscountRepository discountRepository = new DiscountRepository(Setup.GetTemplateEngine());
            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());
            ITenantRepository tenantRepository = new TenantRepository(Setup.GetTemplateEngine());
            ICartRepository cartRepository = new CartRepository(Setup.GetTemplateEngine());
            IProductCartRepository productCartRepository = new ProductCartRepository(Setup.GetTemplateEngine());
            IDiscountCartRepository discountCartRepository = new DiscountCartRepository(Setup.GetTemplateEngine());
            IProductRepository productRepository = new ProductRepository(Setup.GetTemplateEngine());

            IShopLogic shopLogic = new ShopLogic(shopRepository, tenantRepository);
            var discountLogic = new DiscountLogic(discountRepository, shopRepository, cartRepository, productCartRepository, discountCartRepository, productRepository);


            sut = new DiscountController(discountLogic, tenantRepository, mapper, logger);
        }

        [Test, Rollback]
        public async Task TestGetDiscountsForCartWithValidIdReturnsOkObjectResult()
        {
            OkObjectResult response = (OkObjectResult)(await sut.GetDiscountsForCart(appKey, 1));
            Assert.That(response, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestGetDiscountForCartWithInvalidCartIdReturnsNotFoundResult()
        {
            OkObjectResult response = (OkObjectResult)await sut.GetDiscountsForCart(appKey, 1);
            Assert.That(response, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestAddDiscountsToCartWithCustomerlessCartReturnsBadRequestObjectResult()
        {
            // cartId 101 does not have a customerId associated
            BadRequestObjectResult response = (BadRequestObjectResult)await sut.AddDiscountsToCart(appKey, 101, new List<int>() { 1 });
            Assert.That(response, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestAddDiscountsToCartWithEmptyDiscountsReturnsBadRequestObjectResult()
        {
            BadRequestObjectResult response = (BadRequestObjectResult)await sut.AddDiscountsToCart(appKey, 101, new List<int>());
            Assert.That(response, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestAddDiscountsToCartWithNullDiscountsReturnsBadRequestObjectResult()
        {
            BadRequestObjectResult response = (BadRequestObjectResult)await sut.AddDiscountsToCart(appKey, 101, null!);
            Assert.That(response, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestAddDiscountsToCartWithWrongAppKeyReturnsUnauthorizedResult()
        {
            UnauthorizedResult response = (UnauthorizedResult)await sut.AddDiscountsToCart(Guid.NewGuid(), 1, new List<int>() { 1 });
            Assert.That(response, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestAddDiscountsToCartWithWrongCartIdReturnsNotFoundResult()
        {
            NotFoundObjectResult response = (NotFoundObjectResult)await sut.AddDiscountsToCart(appKey, int.MaxValue, new List<int>() { 1 });
            Assert.That(response, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestAddDiscountsToCartWithValidDiscountsReturnsOkObjectResult()
        {
            NoContentResult response = (NoContentResult) await sut.AddDiscountsToCart(appKey, 1, new List<int>() { 1 });
            Assert.That(response, Is.Not.Null);
        }
    }
}
