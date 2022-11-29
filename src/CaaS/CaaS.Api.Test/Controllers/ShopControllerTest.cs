﻿using AutoMapper;
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
    }
}