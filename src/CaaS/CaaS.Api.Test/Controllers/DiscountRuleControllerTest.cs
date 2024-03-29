﻿using AutoMapper;
using CaaS.Api.Controllers;
using CaaS.Api.Transfers;
using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountRules;
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
    [Category("Integration")]
    [TestFixture]
    public class DiscountRuleControllerTest
    {
        private DiscountRuleController sut;
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


            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());
            IDiscountRuleRepository discountRuleRepository = new DiscountRuleRepository(Setup.GetTemplateEngine());
            IDiscountRuleLogic DiscountRuleLogic = new DiscountRuleLogic(discountRuleRepository, shopRepository);

            sut = new DiscountRuleController(DiscountRuleLogic, mapper);
        }

        [Test, Rollback]
        public async Task TestGetDiscountRuleTypes()
        {
            OkObjectResult result = (OkObjectResult)await sut.GetDiscountRules();

            IList<DiscountRulesetBase> values = ((IEnumerable<DiscountRulesetBase>)result.Value!).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(values, Has.Count.EqualTo(2));
            });
        }

        [Test, Rollback]
        public async Task TestGetByShopIdWithValidShopIdReturnsOkObjectResult()
        {
            OkObjectResult result = (OkObjectResult)await sut.GetByShopId(appKey, 1);


            IList<DiscountRule> values = ((IEnumerable<DiscountRule>)result.Value!).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(values, Has.Count.EqualTo(5));
            });
        }

        [Test, Rollback]
        public async Task TestGetByShopIdWithInvalidShopIdReturnsBadRequestObjectResult()
        {
            BadRequestObjectResult result = (BadRequestObjectResult)await sut.GetByShopId(appKey, int.MaxValue);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestGetByShopIdWithInvalidAppKeyReturnsBadRequestResult()
        {
            UnauthorizedResult result = (UnauthorizedResult)await sut.GetByShopId(Guid.NewGuid(), 1);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestDeleteWithValidRuleIdReturnsNoContent()
        {
            NoContentResult result = (NoContentResult)await sut.Delete(appKey, 1);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestDeleteWithInvalidRuleIdReturnsBadRequestObjectResult()
        {
            BadRequestObjectResult result = (BadRequestObjectResult)await sut.Delete(appKey, int.MaxValue);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestDeleteWithValidAppKeyReturnsUnauthorizedResult()
        {
            UnauthorizedResult result = (UnauthorizedResult)await sut.Delete(Guid.NewGuid(), 1);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestCreateWithValidDiscountRuleReturnsOkObjectResult()
        {
            TCreateDiscountRule discountRule = new TCreateDiscountRule(1, "new",
                        new DateDiscountRuleset(DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(5), DateTime.Now));

            CreatedAtActionResult result = (CreatedAtActionResult)await sut.Create(appKey, discountRule);
            int id = (int)result.Value!;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(id, Is.AtLeast(1));

            });
        }

        [Test, Rollback]
        public async Task TestCreateWithInvalidAppKeyReturnsUnauthorizedResult()
        {
            TCreateDiscountRule discountRule = new TCreateDiscountRule(1, "new",
            new DateDiscountRuleset(DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(5), DateTime.Now));

            UnauthorizedResult result = (UnauthorizedResult)await sut.Create(Guid.NewGuid(), discountRule);

            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestCreateWithInvalidShopIdReturnsUnauthorizedResult()
        {
            TCreateDiscountRule discountRule = new TCreateDiscountRule(int.MaxValue, "new",
            new DateDiscountRuleset(DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(5), DateTime.Now));

            BadRequestObjectResult result = (BadRequestObjectResult)await sut.Create(appKey, discountRule);

            Assert.That(result, Is.Not.Null);
        }
    }
}
