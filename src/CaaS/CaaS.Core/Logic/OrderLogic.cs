using CaaS.Core.Domainmodels;
using CaaS.Core.Engines;
using CaaS.Core.Interfaces.Engines;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic.Util;
using CaaS.Util;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic
{
    public class OrderLogic : IOrderLogic
    {
        private readonly IOrderRepository orderRepository;
        private readonly ICartRepository cartRepository;
        private readonly ICouponRepository couponRepository;
        private readonly IProductRepository productRepository;
        private readonly IProductCartRepository productCartRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IShopRepository shopRepository;

        public OrderLogic(IOrderRepository orderRepository, ICartRepository cartRepository, ICouponRepository couponRepository, IProductRepository productRepository, IProductCartRepository productCartRepository, ICustomerRepository customerRepository, IShopRepository shopRepository)
        {
            this.orderRepository = orderRepository ?? throw ExceptionUtil.ParameterNullException(nameof(orderRepository));
            this.cartRepository = cartRepository ?? throw ExceptionUtil.ParameterNullException(nameof(cartRepository));
            this.couponRepository = couponRepository ?? throw ExceptionUtil.ParameterNullException(nameof(couponRepository));
            this.productCartRepository = productCartRepository ?? throw ExceptionUtil.ParameterNullException(nameof(productCartRepository));
            this.customerRepository = customerRepository ?? throw ExceptionUtil.ParameterNullException(nameof(customerRepository));
            this.productRepository = productRepository ?? throw ExceptionUtil.ParameterNullException(nameof(productRepository));
            this.shopRepository = shopRepository ?? throw ExceptionUtil.ParameterNullException(nameof(shopRepository));
        }

        public async Task<int> Create(int id, IEnumerable<Discount> discounts, Guid appKey)
        {
            var cart = await Check.CartAvailabilityWithReferences(cartRepository, productCartRepository, productRepository, shopRepository, id, appKey);
            if (!cart.CustomerId.HasValue) throw new KeyNotFoundException("Cannot create an order from a cart without customerId");

            cart.Discounts = discounts.ToList();
            IDiscountEngine discountEngine = new DiscountEngine(discounts);
            cart.Coupon = await couponRepository.GetByCartId(cart.Id);

            var couponValue = cart.Coupon?.Value ?? 0;
            var completeDiscount = Math.Min(cart.Price, couponValue + discountEngine.CalculateDiscountPrice(cart));
            return await orderRepository.Create(cart, completeDiscount);
        }

        public async Task<Order> Get(int id, Guid appKey)
        {
            var order = await orderRepository.Get(id);
            if (order is null) throw ExceptionUtil.ParameterNullException(nameof(order));
            order.Cart = await Check.CartAvailabilityWithReferences(cartRepository, productCartRepository, productRepository, shopRepository, order.CartId, appKey);
            if (!order.Cart.CustomerId.HasValue) throw new KeyNotFoundException("Cart in order has no CustomerID!!");

            order.Cart.Customer = await customerRepository.Get(order.Cart.CustomerId.Value);

            if (order.Cart.Customer is null) throw ExceptionUtil.NoSuchIdException("customer");

            return order;
        }

        public async Task<IList<Order>> GetByCustomerId(int customerId, Guid appKey)
        {
            var customer = await customerRepository.Get(customerId);
            if(customer is null) throw ExceptionUtil.ParameterNullException(nameof(customer));
            await Check.Customer(shopRepository, customerRepository, customerId, appKey);

            var orders = await orderRepository.GetOrdersByCustomerId(customerId);
            await ReferenceCartsToOrders(orders, appKey);
            
            return orders;
        }

        public async Task<IList<Order>> GetByShopId(int shopId, Guid appKey)
        {
            await Check.ShopAuthorization(shopRepository, shopId, appKey);
            var orders = await orderRepository.GetOrdersByShopId(shopId);
            await ReferenceCartsToOrders(orders, appKey);

            return orders;
        }

        public async Task<bool> Pay(int id, Guid appKey)
        {
            IPaymentEngine paymentEngine = new PaymentEngine();
            var order = await Get(id, appKey);
            var customer = order.Cart!.Customer!;

            if (string.IsNullOrEmpty(customer.CreditCardNumber)) throw ExceptionUtil.ParameterNullException(nameof(customer.CreditCardNumber));
            if (string.IsNullOrEmpty(customer.CVV)) throw ExceptionUtil.ParameterNullException(nameof(customer.CVV));
            if (string.IsNullOrEmpty(customer.Expiration)) throw ExceptionUtil.ParameterNullException(nameof(customer.Expiration));

            return await paymentEngine.Payment(GetOrderAmountToPayAsync(order), customer.CreditCardNumber, customer.CVV, customer.Expiration);
        }

        /// <summary>
        /// Retrieves the correct total price without the discounts
        /// </summary>
        /// <param name="order">The order with which the total price is calculated</param>
        /// <returns></returns>
        private double GetOrderAmountToPayAsync(Order order) => Math.Max(0, order.Cart!.Price - order.Discount);

        /// <summary>
        /// Checks the carts and its references if the appKey is correct 
        /// and additionally sets it to the given orders
        /// </summary>
        /// <param name="orders">The list of orders where the carts should be referenced to</param>
        /// <param name="appKey">The appKey of the given shop</param>
        /// <returns></returns>
        private async Task ReferenceCartsToOrders(IList<Order> orders, Guid appKey)
        {
            var carts = await Check.CartAvailabilityWithReferences(cartRepository, productCartRepository, productRepository, shopRepository, orders.Select(order => order.CartId).ToList(), appKey);

            foreach (var order in orders)
            {
                order.Cart = carts.FirstOrDefault(cart => cart.Id == order.CartId);
            }
        }
    }
}
