using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic.Util;
using CaaS.Core.Repository;
using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic
{
    public class CartLogic : ICartLogic
    {
        private readonly ICartRepository cartRepository;
        private readonly IProductRepository productRepository;
        private readonly IProductCartRepository productCartRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IShopRepository shopRepository;

        public CartLogic(ICartRepository cartRepository, IProductRepository productRepository, IProductCartRepository productCartRepository, ICustomerRepository customerRepository, IShopRepository shopRepository)
        {
            this.cartRepository = cartRepository ?? throw ExceptionUtil.ParameterNullException(nameof(cartRepository));
            this.productCartRepository = productCartRepository ?? throw ExceptionUtil.ParameterNullException(nameof(productCartRepository));
            this.customerRepository = customerRepository ?? throw ExceptionUtil.ParameterNullException(nameof(customerRepository));
            this.productRepository = productRepository ?? throw ExceptionUtil.ParameterNullException(nameof(productRepository));
            this.shopRepository = shopRepository ?? throw ExceptionUtil.ParameterNullException(nameof(shopRepository));
        }
        public async Task<int> Create() => await cartRepository.Create(new Cart(0, Guid.NewGuid().ToString()));

        public async Task<int> CreateCartForCustomer(int customerId, Guid appKey)
        {
            await Check.Customer(shopRepository, customerRepository, customerId, appKey);

            return await cartRepository.Create(new Cart(0, Guid.NewGuid().ToString())
            {
                CustomerId = customerId
            });
        }

        public async Task<bool> DeleteProductFromCart(string sessionId, int productId, Guid appKey, uint? amount)
        {
            await Check.Product(shopRepository, productRepository, productId, appKey);
            var cart = await Check.CartAvailability(cartRepository, sessionId);

            if (!amount.HasValue)
            {
                return await productCartRepository.Delete(productId, cart.Id);
            }
            if (amount.Value < 0) throw new ArgumentOutOfRangeException("No amount allowed below 1");

            var productCart = await productCartRepository.Get(productId, cart.Id);
            if (productCart is null) throw ExceptionUtil.ParameterNullException(nameof(productCart));
            
            amount = productCart.Amount - amount;
            if(amount.Value <= 0)
            {
                return await productCartRepository.Delete(productId, cart.Id);
            }
            return await productCartRepository.Update(productId, cart.Id, amount.Value);
        }

        public async Task<bool> ReferenceCustomerToCart(int customerId, string sessionId, Guid appKey)
        {
            await Check.Customer(shopRepository, customerRepository, customerId, appKey);

            var cart = await Check.CartAvailability(cartRepository, sessionId);

            cart.CustomerId = customerId;

            return await cartRepository.Update(cart);
        }

        public async Task<bool> ReferenceProductToCart(string sessionId, int productId, Guid appKey, uint? amount)
        {
            var product = await Check.Product(shopRepository, productRepository, productId, appKey);
            var cart = await Check.CartAvailability(cartRepository, sessionId);

            if(!amount.HasValue || amount.Value < 1)
            {
                amount = 1;
            }

            var productCart = await productCartRepository.Get(productId, cart.Id);

            if(productCart is not null)
            {
                amount += productCart.Amount;
                return await productCartRepository.Update(productId, cart.Id, amount.Value);
            }

            return await productCartRepository.Create(new ProductCart(productId, cart.Id, product.Price, amount.Value)) > 0;
        }
    }
}
