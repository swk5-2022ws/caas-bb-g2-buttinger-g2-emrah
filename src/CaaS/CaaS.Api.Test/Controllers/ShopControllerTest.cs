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
using Microsoft.Extensions.Logging.Console;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Api.Test.Controllers
{
    [Category("Integration")]
    [TestFixture]
    public class ShopControllerTest
    {
        private ShopController sut;

        [OneTimeSetUp]
        public void InitializeSut()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<ShopController> logger = loggerFactory.CreateLogger<ShopController>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(ShopController));
            });
            var mapper = mockMapper.CreateMapper();


            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());
            ITenantRepository tenantRepository = new TenantRepository(Setup.GetTemplateEngine());
            IShopLogic shopLogic = new ShopLogic(shopRepository, tenantRepository);

            sut = new ShopController(shopLogic, tenantRepository ,mapper, logger);
        }

        [Test, Rollback]
        public async Task TestCreateShopWithValidShopReturnsCreatedAtActionResult()
        {
            TCreateShop shop = new("new shop", 1);

            CreatedAtActionResult actionResult = (CreatedAtActionResult) await sut.CreateShop(shop);

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(201));
            Assert.That(actionResult.RouteValues, Is.Not.Empty);
            Assert.That(actionResult.Value, Is.Not.Null);
        }

        [TestCase("", 1)]
        [TestCase("Label", int.MaxValue)]
        [Test, Rollback]
        public async Task TestCreateShopWithInvalidShopReturnsBadRequestObjectResult(string label, int tenantId)
        {
            TCreateShop shop = new(label, tenantId);

            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.CreateShop(shop);

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task TestGetWithValidIdReturnsOkObjectResult()
        {
            OkObjectResult result = (OkObjectResult)await sut.GetShopById(1);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task TestGetWithInvalidIdReturnsNotFoundResult()
        {
            NotFoundResult result = (NotFoundResult)await sut.GetShopById(int.MaxValue);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestUpdateWithValidShopReturnsNoContentResult()
        {
            TShop shop = new TShop(1, "updated", 1, Guid.NewGuid());
            NoContentResult result = (NoContentResult)await sut.UpdateShop(shop);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestUpdateWithInvaliTenantIdReturnsNotFoundResult()
        {
            TShop shop = new TShop(1, "updated", int.MaxValue, Guid.NewGuid());
            NotFoundResult result = (NotFoundResult)await sut.UpdateShop(shop);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestUpdateWithInvaliShopIdReturnsNotFoundResult()
        {
            TShop shop = new TShop(int.MaxValue, "updated", 1, Guid.NewGuid());
            NotFoundResult result = (NotFoundResult)await sut.UpdateShop(shop);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestUpdateWithInvaliLabelReturnsBadRequestResult()
        {
            TShop shop = new TShop(1, "", 1, Guid.NewGuid());
            BadRequestObjectResult result = (BadRequestObjectResult)await sut.UpdateShop(shop);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestDeleteWithValidIdReturnsNoContentResult()
        {
            NoContentResult response = (NoContentResult)await sut.DeleteShop(1);
            Assert.That(response, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestDeleteWithInvalidIdReturnsNotFoundResult()
        {
            NotFoundResult response = (NotFoundResult)await sut.DeleteShop(int.MaxValue);
            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task TestGetShopsByTenantIdWithValidIdReturnsOkObjectResult()
        {
            OkObjectResult response = (OkObjectResult)await sut.GetShopsByTenantId(1);
            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task TestGetShopsByTenantIdWithInvalidIdReturnsNotFoundResult()
        {
            NotFoundResult response = (NotFoundResult)await sut.GetShopsByTenantId(int.MaxValue);
            Assert.That(response, Is.Not.Null);
        }


    }
}
