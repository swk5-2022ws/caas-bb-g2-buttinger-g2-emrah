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

namespace CaaS.Api.Test.UseCases
{
    [Category("System")]
    [TestFixture]
    public class PaymentUseCase
    {
        private CartController cartController;
        private OrderController orderController;
        private readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        private Guid appKey = Guid.Parse("8f26f620-9957-3251-8002-d593fad0003a");

        [OneTimeSetUp]
        public void Init()
        {
            ICartRepository cartRepository = new CartRepository(Setup.GetTemplateEngine());
            IProductCartRepository productCartRepository = new ProductCartRepository(Setup.GetTemplateEngine());
            IProductRepository productRepository = new ProductRepository(Setup.GetTemplateEngine());
            ICustomerRepository customerRepository = new CustomerRepository(Setup.GetTemplateEngine());
            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());
            IDiscountRepository discountRepository = new DiscountRepository(Setup.GetTemplateEngine());
            IDiscountActionRepository discountActionRepository = new DiscountActionRepository(Setup.GetTemplateEngine());
            IDiscountRuleRepository discountRuleRepository = new DiscountRuleRepository(Setup.GetTemplateEngine());
            IDiscountCartRepository discountCartRepository = new DiscountCartRepository(Setup.GetTemplateEngine());
            IDiscountLogic discountLogic = new DiscountLogic(discountRepository, shopRepository, cartRepository, productCartRepository, discountCartRepository, productRepository, discountActionRepository, discountRuleRepository);
            InitializeCartController(cartRepository, productRepository, productCartRepository, customerRepository, shopRepository, discountLogic, discountCartRepository);
            InitializeOrderController(discountLogic, cartRepository, productRepository, productCartRepository, customerRepository, shopRepository);
        }

        private void InitializeCartController(ICartRepository cartRepository, IProductRepository productRepository, IProductCartRepository productCartRepository, ICustomerRepository customerRepository, IShopRepository shopRepository, IDiscountLogic discountLogic, IDiscountCartRepository discountCartRepository)
        {
            ICartLogic cartLogic = new CartLogic(cartRepository, productRepository, productCartRepository, customerRepository, shopRepository, discountLogic, discountCartRepository);
            cartController = new CartController(cartLogic, GetMapper(typeof(CartController)), loggerFactory.CreateLogger<CartController>());
        }

        private void InitializeOrderController(IDiscountLogic discountLogic, ICartRepository cartRepository, IProductRepository productRepository, IProductCartRepository productCartRepository, ICustomerRepository customerRepository, IShopRepository shopRepository)
        {
            IOrderRepository orderRepository = new OrderRepository(Setup.GetTemplateEngine());
            ICouponRepository couponRepository = new CouponRepository(Setup.GetTemplateEngine());

            IOrderLogic orderLogic = new OrderLogic(orderRepository, cartRepository, couponRepository, productRepository, productCartRepository, customerRepository, shopRepository);
            orderController = new OrderController(orderLogic, discountLogic, GetMapper(typeof(OrderController)), loggerFactory.CreateLogger<OrderController>());
        }

        private IMapper GetMapper(Type t)
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(t);
            });
            return mockMapper.CreateMapper();
        }

        [Test, Rollback]
        public async Task PaymentProcessUseCase()
        {
            //create a cart
            var createdCartAction = (CreatedAtActionResult)await cartController.Create(appKey);
            Assert.That(createdCartAction, Is.Not.Null);
            Assert.That(createdCartAction.Value, Is.Not.Null);
            string sessionId = (string)createdCartAction.Value!;

            //map customer to cart
            var noContentCustomerReference = (NoContentResult)await cartController.ReferenceCustomer(5, sessionId, appKey);
            Assert.That(noContentCustomerReference, Is.Not.Null);
            Assert.That(noContentCustomerReference.StatusCode, Is.EqualTo(204));

            //reference product to cart
            var noContentProductReference = (NoContentResult) await cartController.ReferenceProduct(sessionId, 5, 1, appKey);
            Assert.That(noContentProductReference, Is.Not.Null);
            Assert.That(noContentProductReference.StatusCode, Is.EqualTo(204));

            //create order
            var okCart = await cartController.Get(sessionId, appKey);
            Assert.That(okCart, Is.Not.Null);
            Assert.That(okCart.Result, Is.Not.Null);

            var result = (OkObjectResult)okCart.Result;
            var cart = result!.Value as TCart;

            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.SessionId, Is.EqualTo(sessionId));

            int cartId = cart.Id;

            var createdOrderAction = (CreatedAtActionResult)await orderController.Create(cartId, appKey);
            Assert.That(createdOrderAction, Is.Not.Null);
            Assert.That(createdOrderAction.Value, Is.GreaterThan(0));
            int orderId = (int)createdOrderAction.Value!;

            //pay the order
            var noContentPay = (NoContentResult)await orderController.Pay(orderId, appKey);
            Assert.That(noContentPay, Is.Not.Null);
            Assert.That(noContentPay.StatusCode, Is.EqualTo(204));
        }

    }
}
