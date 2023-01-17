using CaaS.Core.Domainmodels;
using CaaS.Core.Engines;
using CaaS.Core.Interfaces.Engines;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic.Util;
using CaaS.Core.Repository;
using CaaS.Util;
using MySqlX.XDevAPI;
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
        private readonly IDiscountLogic discountLogic;
        private readonly IDiscountCartRepository discountCartRepository;
        private readonly ICouponRepository couponRepository;

        public CartLogic(ICartRepository cartRepository, IProductRepository productRepository, IProductCartRepository productCartRepository, 
            ICustomerRepository customerRepository, IShopRepository shopRepository,
            IDiscountLogic discountLogic,
            IDiscountCartRepository discountCartRepository,
            ICouponRepository couponRepository)
        {
            this.cartRepository = cartRepository ?? throw ExceptionUtil.ParameterNullException(nameof(cartRepository));
            this.productCartRepository = productCartRepository ?? throw ExceptionUtil.ParameterNullException(nameof(productCartRepository));
            this.customerRepository = customerRepository ?? throw ExceptionUtil.ParameterNullException(nameof(customerRepository));
            this.productRepository = productRepository ?? throw ExceptionUtil.ParameterNullException(nameof(productRepository));
            this.shopRepository = shopRepository ?? throw ExceptionUtil.ParameterNullException(nameof(shopRepository));
            this.discountLogic = discountLogic ?? throw ExceptionUtil.ParameterNullException(nameof(discountLogic));
            this.discountCartRepository = discountCartRepository ?? throw ExceptionUtil.ParameterNullException(nameof(discountCartRepository));
            this.couponRepository = couponRepository;
        }
        public async Task<string> Create()
        {
            var cartId = await cartRepository.Create(new Cart(0, Guid.NewGuid().ToString()) { ModifiedDate = DateTime.UtcNow });
            return (await cartRepository.Get(cartId))?.SessionId ?? throw new ArgumentException("Cart could not be created");
        }

        public async Task<string> CreateCartForCustomer(int customerId, Guid appKey)
        {
            await Check.Customer(shopRepository, customerRepository, customerId, appKey);

            var cartId = await cartRepository.Create(new Cart(0, Guid.NewGuid().ToString())
            {
                CustomerId = customerId,
                ModifiedDate = DateTime.UtcNow
            });

            return (await cartRepository.Get(cartId))?.SessionId ?? throw new ArgumentException("Cart could not be created");
        }

        public async Task<bool> DeleteProductFromCart(string sessionId, int productId, Guid appKey, uint? amount)
        {
            await Check.Product(shopRepository, productRepository, productId, appKey);
            var cart = await Check.CartAvailability(cartRepository, sessionId);

            if (!amount.HasValue)
            {
                return await productCartRepository.Delete(productId, cart.Id);
            }
            if (amount.Value <= 0) throw new ArgumentOutOfRangeException("No amount allowed below 1");

            var productCart = await productCartRepository.Get(productId, cart.Id);
            if (productCart is null) throw ExceptionUtil.ParameterNullException(nameof(productCart));

            if ((productCart.Amount - (int)amount.Value) <= 0)
            {
                return await productCartRepository.Delete(productId, cart.Id);
            }

            amount = productCart.Amount - amount;
            var updatedProductCart = await productCartRepository.Update(productId, cart.Id, amount.Value);
            if (!updatedProductCart) return false;

            cart.ModifiedDate = DateTime.UtcNow;
            return await cartRepository.Update(cart);
        }

        public async Task<Cart> Get(string sessionId, Guid appKey)
        {
            int? shopId = null;

            var cart = await cartRepository.GetBySession(sessionId);
            if (cart is null) throw ExceptionUtil.NoSuchIdException(nameof(sessionId));

            if (cart.CustomerId.HasValue)
            {
                await Check.Customer(shopRepository, customerRepository, cart.CustomerId.Value, appKey);
            }

            var productCarts = await productCartRepository.GetByCartId(cart.Id);

            if (productCarts is not null && productCarts.Count > 0)
            {
                var productIds = productCarts.Select(x => x.ProductId);
                var products = await productRepository.Get(productIds.ToList());


                if (products is not null && products.Count > 0)
                {
                    if (products.Select(x => x.ShopId).Distinct().Count() > 1)
                        throw new ArgumentException("No such product or coupon in shop");

                    shopId = products.First().ShopId;
                    await Check.ShopAuthorization(shopRepository, shopId.Value, appKey);

                    foreach (var pc in productCarts)
                    {
                        pc.Product = products.FirstOrDefault(x => x.Id == pc.ProductId);
                    }
                }
                cart.ProductCarts = productCarts.ToHashSet();
                // must be executed after products have been applied
                if(shopId.HasValue) await AddAlreadyAppliedDiscountsToCartIfStillValid(appKey, cart, shopId.Value);   
            }

            var coupon = await couponRepository.GetByCartId(cart.Id);
            cart.Coupon = coupon;

            return cart;
        }

        private async Task AddAlreadyAppliedDiscountsToCartIfStillValid(Guid appKey, Cart cart, int shopId)
        {
            var cartDiscounts = await discountCartRepository.GetByCartId(cart.Id);
            var allDiscounts = await discountLogic.GetByShopId(appKey, shopId);
            var discountsByCart = allDiscounts.Where(x => cartDiscounts.Any(y => y.DiscountId == x.Id));
            IDiscountEngine discountEngine = new DiscountEngine(discountsByCart);
            discountEngine.ApplyValidDiscounts(cart);
        }

        public async Task<bool> ReferenceCustomerToCart(int customerId, string sessionId, Guid appKey)
        {
            int? shopId = null;
            await Check.Customer(shopRepository, customerRepository, customerId, appKey);

            var cart = await Check.CartAvailability(cartRepository, sessionId);
            if (cart.CustomerId.HasValue) throw ExceptionUtil.ReferenceException(nameof(cart));
            cart.CustomerId = customerId;
            cart.ModifiedDate = DateTime.UtcNow;

            var productCarts = await productCartRepository.GetByCartId(cart.Id);

            if (productCarts is not null && productCarts.Count > 0)
            {
                var productIds = productCarts.Select(x => x.ProductId);
                var products = await productRepository.Get(productIds.ToList());


                if (products is not null && products.Count > 0)
                {
                    if (products.Select(x => x.ShopId).Distinct().Count() > 1)
                        throw new ArgumentException("No such product or coupon in shop");

                    shopId = products.First().ShopId;
                    await Check.ShopAuthorization(shopRepository, shopId.Value, appKey);

                    foreach (var pc in productCarts)
                    {
                        pc.Product = products.FirstOrDefault(x => x.Id == pc.ProductId);
                    }
                }
                cart.ProductCarts = productCarts.ToHashSet();
                // must be executed after products have been applied
                if (shopId.HasValue) await AddAlreadyAppliedDiscountsToCartIfStillValid(appKey, cart, shopId.Value);


            }

            return await cartRepository.Update(cart);
        }

        public async Task<bool> ReferenceProductToCart(string sessionId, int productId, Guid appKey, uint? amount)
        {
            var product = await Check.Product(shopRepository, productRepository, productId, appKey);
            var cart = await Check.CartAvailability(cartRepository, sessionId);

            if (!amount.HasValue || amount.Value < 1)
            {
                amount = 1;
            }

            var productCart = await productCartRepository.Get(productId, cart.Id);

            if (productCart is not null)
            {
                amount += productCart.Amount;
                return await productCartRepository.Update(productId, cart.Id, amount.Value);
            }

            var addedToCart = await productCartRepository.Create(new ProductCart(productId, cart.Id, product.Price, amount.Value)) > 0;

            if (!addedToCart) return false;
            cart.ModifiedDate = DateTime.UtcNow;
            return await cartRepository.Update(cart);
        }

        public async Task<Cart> GetByCustomerId(int customerId, Guid appKey)
        {
            int? shopId = null;

            var cart = await cartRepository.GetByCustomerId(customerId);
            if (cart is null) throw ExceptionUtil.NoSuchIdException(nameof(customerId));

            if (cart.CustomerId.HasValue)
            {
                await Check.Customer(shopRepository, customerRepository, cart.CustomerId.Value, appKey);
            }

            var productCarts = await productCartRepository.GetByCartId(cart.Id);

            if (productCarts is not null && productCarts.Count > 0)
            {
                var productIds = productCarts.Select(x => x.ProductId);
                var products = await productRepository.Get(productIds.ToList());


                if (products is not null && products.Count > 0)
                {
                    if (products.Select(x => x.ShopId).Distinct().Count() > 1)
                        throw new ArgumentException("No such product or coupon in shop");

                    shopId = products.First().ShopId;
                    await Check.ShopAuthorization(shopRepository, shopId.Value, appKey);

                    foreach (var pc in productCarts)
                    {
                        pc.Product = products.FirstOrDefault(x => x.Id == pc.ProductId);
                    }



                }
                cart.ProductCarts = productCarts.ToHashSet();
                // must be executed after products have been applied
                if (shopId.HasValue) await AddAlreadyAppliedDiscountsToCartIfStillValid(appKey, cart, shopId.Value);
            }

            var coupon = await couponRepository.GetByCartId(cart.Id);
            cart.Coupon = coupon;

            return cart;
        }


    }
}
