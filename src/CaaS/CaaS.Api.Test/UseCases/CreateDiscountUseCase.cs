using AutoMapper;
using CaaS.Api.Controllers;
using CaaS.Api.Transfers;
using CaaS.Core.Domainmodels.DiscountActions;
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

namespace CaaS.Api.Test.UseCases
{
    [Category("System")]
    [TestFixture]
    public class CreateDiscountUseCase
    {
        private DiscountController discountController;
        private DiscountActionController discountActionController;
        private DiscountRuleController DiscountRuleController;
        private Guid appKey = Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906");
        private readonly int shopId = 1;

        [OneTimeSetUp]
        public void Init()
        {
            InitializeDiscountController();
            InitializeDiscountActionController();
            InitializeDiscountRuleController();
        }

        private void InitializeDiscountController()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<DiscountController> logger = loggerFactory.CreateLogger<DiscountController>();
            IDiscountRepository discountRepository = new DiscountRepository(Setup.GetTemplateEngine());
            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());
            ITenantRepository tenantRepository = new TenantRepository(Setup.GetTemplateEngine());
            ICartRepository cartRepository = new CartRepository(Setup.GetTemplateEngine());
            IProductCartRepository productCartRepository = new ProductCartRepository(Setup.GetTemplateEngine());
            IDiscountCartRepository discountCartRepository = new DiscountCartRepository(Setup.GetTemplateEngine());
            IProductRepository productRepository = new ProductRepository(Setup.GetTemplateEngine());
            IDiscountActionRepository discountActionRepository = new DiscountActionRepository(Setup.GetTemplateEngine());
            IDiscountRuleRepository discountRuleRepository = new DiscountRuleRepository(Setup.GetTemplateEngine());

            IShopLogic shopLogic = new ShopLogic(shopRepository, tenantRepository);
            var discountLogic = new DiscountLogic(discountRepository, shopRepository, cartRepository, productCartRepository, discountCartRepository, productRepository
                , discountActionRepository, discountRuleRepository);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(DiscountController));
            });
            var mapper = mockMapper.CreateMapper();

            discountController = new DiscountController(discountLogic, tenantRepository, mapper, logger);
        }

        private void InitializeDiscountActionController()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<DiscountController> logger = loggerFactory.CreateLogger<DiscountController>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(DiscountController));
            });
            var mapper = mockMapper.CreateMapper();


            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());
            IDiscountActionRepository discountActionRepository = new DiscountActionRepository(Setup.GetTemplateEngine());
            IDiscountActionLogic discountActionLogic = new DiscountActionLogic(discountActionRepository, shopRepository);

            discountActionController = new DiscountActionController(discountActionLogic, mapper);
        }

        private void InitializeDiscountRuleController()
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

            DiscountRuleController = new DiscountRuleController(DiscountRuleLogic, mapper);
        }

        [Test, Rollback]
        public async Task TestUseCaseCreateDiscount()
        {
            // 1. create discountAction
            // fetch action types
            // create new action
            // cache id            
            OkObjectResult getActionsResponse = ((OkObjectResult)await discountActionController.GetDiscountActions());
            Assert.That(getActionsResponse, Is.Not.Null);

            var actions = (getActionsResponse!.Value! as IEnumerable<DiscountActionBase>)!.ToList();
            Assert.That(actions,Is.Not.Null);
            Assert.That(actions, Has.Count.EqualTo(2));
            Assert.That(actions.Any(x => x.GetType() == typeof(TotalPercentageDiscountAction)));

            TotalPercentageDiscountAction totalPercentageDiscountAction = new(0.3d);

            CreatedAtActionResult createDiscountActionResponse = (CreatedAtActionResult)await discountActionController.
                Create(appKey, new Transfers.TCreateDiscountAction(shopId, "UseCase Action", totalPercentageDiscountAction));

            Assert.That(createDiscountActionResponse, Is.Not.Null);
            Assert.That(createDiscountActionResponse.Value, Is.GreaterThan(0));
            int discountActionId = (int)createDiscountActionResponse.Value!;

            // 2. create discountRule
            // fetch rule types
            // create new rule
            // cache id
            OkObjectResult getRuleResponse = ((OkObjectResult)await DiscountRuleController.GetDiscountRules());
            Assert.That(getRuleResponse, Is.Not.Null);

            var rules = (getRuleResponse!.Value! as IEnumerable<DiscountRulesetBase>)!.ToList();
            Assert.That(rules, Is.Not.Null);
            Assert.That(rules, Has.Count.EqualTo(2));
            Assert.That(rules.Any(x => x.GetType() == typeof(TotalAmountDiscountRuleset)));

            TotalAmountDiscountRuleset totalAmountRuleset = new(99.9d);

            CreatedAtActionResult createDiscountRuleResponse = (CreatedAtActionResult)await DiscountRuleController.
                Create(appKey, new Transfers.TCreateDiscountRule(shopId, "UseCase Rule", totalAmountRuleset));

            Assert.That(createDiscountRuleResponse, Is.Not.Null);
            Assert.That(createDiscountRuleResponse.Value, Is.GreaterThan(0));
            int discountRuleId = (int)createDiscountRuleResponse.Value!;

            // 3. create discount
            // fetch discount to validate it
            CreatedAtActionResult createDiscountReponse = (CreatedAtActionResult)await discountController.CreateDiscount(appKey, new Transfers.TCreateDiscount(discountActionId, discountRuleId));
            Assert.That(createDiscountReponse, Is.Not.Null);
            Assert.That(createDiscountReponse.Value, Is.Not.Null);
            Assert.That(createDiscountReponse.RouteValues, Is.Not.Null);
            Assert.That(createDiscountReponse.RouteValues.ContainsKey("id"), Is.True);

            int discountId = (int)createDiscountReponse.RouteValues!.GetValueOrDefault("id")!;
            Assert.That(discountId, Is.GreaterThan(0)); 

            OkObjectResult getDiscountResponse = (OkObjectResult)await discountController.Get(appKey, discountId);
            Assert.That(getDiscountResponse, Is.Not.Null);

            TDiscount discount = (TDiscount)getDiscountResponse!.Value!;
            Assert.Multiple(() =>
            {
                Assert.That(discount, Is.Not.Null);
                Assert.That(discount!.DiscountRule.Id, Is.EqualTo(discountRuleId));
                Assert.That(discount!.discountAction.Id, Is.EqualTo(discountActionId));
            });
        }
    }
}
