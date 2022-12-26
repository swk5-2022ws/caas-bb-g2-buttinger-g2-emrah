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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Api.Test.Controllers
{
    public class OrderControllerTest
    {
        private OrderController sut;

        [OneTimeSetUp]
        public void InitializeSut()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<OrderController> logger = loggerFactory.CreateLogger<OrderController>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(OrderController));
            });
            var mapper = mockMapper.CreateMapper();


            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());
            IProductRepository productRepository = new ProductRepository(Setup.GetTemplateEngine());
            ICartRepository cartRepository = new CartRepository(Setup.GetTemplateEngine());
            ICustomerRepository customerRepository = new CustomerRepository(Setup.GetTemplateEngine());
            IOrderRepository orderRepository = new OrderRepository(Setup.GetTemplateEngine());
            IProductCartRepository productCartRepository = new ProductCartRepository(Setup.GetTemplateEngine());
            ICouponRepository couponRepository = new CouponRepository(Setup.GetTemplateEngine());

            IDiscountRepository discountRepository = new DiscountRepository(Setup.GetTemplateEngine());
            IDiscountCartRepository discountCartRepository = new DiscountCartRepository(Setup.GetTemplateEngine());
            IDiscountRuleRepository discountRuleRepository = new DiscountRuleRepository(Setup.GetTemplateEngine());
            IDiscountActionRepository discountActionRepository = new DiscountActionRepository(Setup.GetTemplateEngine());

            IOrderLogic orderLogic = new OrderLogic(orderRepository, cartRepository, couponRepository, productRepository, productCartRepository, customerRepository, shopRepository);
            IDiscountLogic discountLogic = new DiscountLogic(discountRepository, shopRepository, cartRepository, productCartRepository, discountCartRepository, productRepository, discountActionRepository, discountRuleRepository);

            sut = new OrderController(orderLogic, discountLogic, mapper, logger);
        }

        [Test, Rollback]
        public async Task TestCreateOrderWithIncorrectAppKeyReturnsBadRequest()
        {
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Create(1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }
        
        [Test, Rollback]
        public async Task TestCreateOrderWithIncorrectCartIdReturnsNotFound()
        {
            NotFoundObjectResult actionResult = (NotFoundObjectResult)await sut.Create(-1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(404));
        }
        
        [Test, Rollback]
        public async Task TestCreateOrderWithCartWithoutCustomerReturnsBadRequest()
        {
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Create(101, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        public async Task TestCreateOrderReturnsCreatedAtActionResult()
        {
            CreatedAtActionResult actionResult = (CreatedAtActionResult)await sut.Create(1, new Guid("a82724ba-ced5-32e8-9ada-17b06d427906"));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(201));
        }

        [Test, Rollback]
        public async Task TestPayOrderWithIncorrectAppKeyReturnsBadRequest()
        {
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Create(1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }
       
        [Test, Rollback]
        public async Task TestPayOrderWithIncorrectIdReturnsNotFound()
        {
            NotFoundObjectResult actionResult = (NotFoundObjectResult)await sut.Create(-1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(404));
        }

        [Test, Rollback]
        public async Task TestEditProductWithValidIdReturnsNoContent()
        {
            var actionResult = (NoContentResult)await sut.Pay(5, new Guid("8f26f620-9957-3251-8002-d593fad0003a"));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(204));
        }
    }
}
