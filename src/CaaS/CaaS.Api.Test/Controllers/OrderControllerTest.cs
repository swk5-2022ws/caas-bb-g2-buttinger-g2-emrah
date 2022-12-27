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
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Pay(1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }
       
        [Test, Rollback]
        public async Task TestPayOrderWithIncorrectIdReturnsBadRequest()
        {
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Pay(-1, new Guid("8f26f620-9957-3251-8002-d593fad0003a"));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        public async Task TestPayOrderReturnsNoContent()
        {
            var actionResult = (NoContentResult)await sut.Pay(5, new Guid("8f26f620-9957-3251-8002-d593fad0003a"));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(204));
        }

        [Test, Rollback]
        public async Task TestGetOrderWithIncorrectAppKeyReturnsBadRequest()
        {
            var actionResult = await sut.Get(1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (BadRequestObjectResult)actionResult.Result;
            Assert.That(result!.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        public async Task TestGetOrderWithIncorrectIdReturnsNotFound()
        {
            var actionResult = await sut.Get(-1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (NotFoundObjectResult)actionResult.Result;
            Assert.That(result!.StatusCode, Is.EqualTo(404));
        }

        [Test, Rollback]
        [TestCase(5, "8f26f620-9957-3251-8002-d593fad0003a", 5)]
        public async Task TestGetOrderWithValidIdReturnsOk(int orderId, string appKey, int cartId)
        {
            var actionResult = await sut.Get(orderId, new Guid(appKey));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (OkObjectResult)actionResult.Result;
            TOrder? order = (TOrder?)result!.Value;

            Assert.That(order, Is.Not.Null);
            Assert.That(order.Id, Is.EqualTo(orderId));
            Assert.That(order.CartId, Is.EqualTo(cartId));
            Assert.That(order.Cart, Is.Not.Null);

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test, Rollback]
        public async Task TestGetByShopOrderWithIncorrectAppKeyReturnsBadRequest()
        {
            var actionResult = await sut.GetByShop(1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (BadRequestObjectResult)actionResult.Result;
            Assert.That(result!.StatusCode, Is.EqualTo(400));
        }


        [Test, Rollback]
        [TestCase(5, "8f26f620-9957-3251-8002-d593fad0003a", 5)]
        public async Task TestByShopGetOrderWithValidIdReturnsOk(int orderId, string appKey, int cartId)
        {
            var actionResult = await sut.GetByShop(orderId, new Guid(appKey));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (OkObjectResult)actionResult.Result;
            IList <TOrder> orders = (IList<TOrder>)result!.Value!;
            Assert.That(orders, Is.Not.Null);
            Assert.That(orders.Count, Is.EqualTo(8));

            var order = orders[0];
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Id, Is.EqualTo(orderId));
            Assert.That(order.CartId, Is.EqualTo(cartId));
            Assert.That(order.Cart, Is.Not.Null);

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test, Rollback]
        public async Task TestGetByCustomerOrderWithIncorrectAppKeyReturnsBadRequest()
        {
            var actionResult = await sut.GetByCustomer(1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (BadRequestObjectResult)actionResult.Result;
            Assert.That(result!.StatusCode, Is.EqualTo(400));
        }
        [Test, Rollback]
        public async Task TestGetByCustomerOrderWithIncorrectIdReturnsNotFound()
        {
            var actionResult = await sut.GetByCustomer(-1, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (NotFoundObjectResult)actionResult.Result;
            Assert.That(result!.StatusCode, Is.EqualTo(404));
        }


        [Test, Rollback]
        [TestCase(5, "8f26f620-9957-3251-8002-d593fad0003a", 5)]
        public async Task TestByCustomerGetOrderWithValidIdReturnsOk(int customerId, string appKey, int cartId)
        {
            var actionResult = await sut.GetByCustomer(customerId, new Guid(appKey));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (OkObjectResult)actionResult.Result;
            IList<TOrder> orders = (IList<TOrder>)result!.Value!;
            Assert.That(orders, Is.Not.Null);
            Assert.That(orders.Count, Is.EqualTo(2));

            var order = orders[0];
            Assert.That(order, Is.Not.Null);
            Assert.That(order.Id, Is.EqualTo(customerId));
            Assert.That(order.CartId, Is.EqualTo(cartId));
            Assert.That(order.Cart, Is.Not.Null);

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
    }
}
