using AutoMapper;
using CaaS.Api.Controllers;
using CaaS.Api.Transfers;
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

            sut = new ShopController(shopLogic, mapper, logger);
        }

        [Test, Rollback]
        public async Task TestCreateShopWithValidShopReturnsCreatedAtActionResult()
        {
            TCreateShop shop = new(0, "new shop", 1);

            CreatedAtActionResult actionResult = (CreatedAtActionResult) await sut.CreateShop(shop);

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(201));
        }
    }
}
