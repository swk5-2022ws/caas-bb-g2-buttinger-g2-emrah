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

namespace CaaS.Api.Test.Controllers
{
    [Category("Integration")]
    [TestFixture]
    public class DiscountControllerTest
    {
        private DiscountController sut;

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

        [TestCase("a82724ba-ced5-32e8-9ada-17b06d427906", 1)]
        [Test]
        public async Task TestGetDiscountsForCartWithValidIdReturnsOkObjectResult(Guid appKey, int cartId)
        {
            OkObjectResult response = (OkObjectResult) (await sut.GetDiscountsForCart(appKey, cartId));
            Assert.That(response, Is.Not.Null);
        }

        [TestCase("a82724ba-ced5-32e8-9ada-17b06d427906", 1)]
        [Test]
        public async Task TestGetDiscountForCartWithInvalidCartIdReturnsNotFoundResult(Guid appKey, int cartId)
        {
            OkObjectResult response = (OkObjectResult)await sut.GetDiscountsForCart(appKey, cartId);
            Assert.That(response, Is.Not.Null);
        }
    }
}
