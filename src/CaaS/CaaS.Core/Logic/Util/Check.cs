using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Util;
using System.Data;

namespace CaaS.Core.Logic.Util
{
    public static class Check
    {
        // TODO merge with ShopAuthorization => adapt tests to use UnauthorizedAccessException
        public static async Task Shop(IShopRepository shopRepository, int shopId, Guid appKey)
        {
            var availableShop = await shopRepository.Get(shopId) ?? throw new ArgumentException($"The shop with id={shopId} is currently not available.");
            if (availableShop.AppKey != appKey) throw new UnauthorizedAccessException($"You have not the right privileges.");
        }

        /// <summary>
        /// Checks weather a shop with a given id is in the repository and
        /// if the shops appkey matches with the passed one
        /// </summary>
        /// <param name="shopRepository">ShopRepository</param>
        /// <param name="shopId">Passed shopId</param>
        /// <param name="appKey">Passed appkey</param>
        /// <returns></returns>
        public static async Task ShopAuthorization(IShopRepository shopRepository, int shopId, Guid appKey)
        {
            var availableShop = await shopRepository.Get(shopId) ?? throw new ArgumentException($"The shop with id={shopId} is currently not available.");
            if (availableShop.AppKey != appKey) throw new ArgumentException($"You have not the right privileges.");
        }

        /// <summary>
        /// Checks weather a given customer is already there and if the customers shop matches the passed appKeys shop
        /// </summary>
        /// <param name="shopRepository">The shopRepository</param>
        /// <param name="customerRepository">The customerRepository</param>
        /// <param name="customerId">the customer id</param>
        /// <param name="appKey">the passed shop key</param>
        /// <returns>void</returns>
        public static async Task Customer(IShopRepository shopRepository, ICustomerRepository customerRepository, int customerId, Guid appKey)
        {
            var customer = await customerRepository.Get(customerId);

            if (customer is null) throw ExceptionUtil.ParameterNullException(nameof(customerId));
            await Check.ShopAuthorization(shopRepository, customer.ShopId, appKey);
        }

        /// <summary>
        /// Checks weather a product is there and weather or not the product references the correct shop
        /// </summary>
        /// <param name="shopRepository">the shoprepository</param>
        /// <param name="productRepository">the productRepository</param>
        /// <param name="productId">the id of the product</param>
        /// <param name="appKey">the key of the current shop</param>
        /// <returns></returns>
        public static async Task<Product> Product(IShopRepository shopRepository, IProductRepository productRepository, int productId, Guid appKey)
        {
            var product = await productRepository.Get(productId);

            if (product is null) throw ExceptionUtil.ParameterNullException(nameof(product));
            await ShopAuthorization(shopRepository, product.ShopId, appKey);

            return product;
        }

        public static async Task<Cart> CartAvailability(ICartRepository cartRepository, string sessionId)
        {
            var cart = await cartRepository.GetBySession(sessionId);
            if (cart is null) throw ExceptionUtil.NoSuchIdException(nameof(cart));

            return cart;
        }
        public static async Task<Cart> CartAvailability(ICartRepository cartRepository, int id)
        {
            var cart = await cartRepository.Get(id);
            if (cart is null) throw ExceptionUtil.NoSuchIdException(nameof(cart));

            return cart;
        }
        public static async Task<IList<Cart>> CartAvailability(ICartRepository cartRepository, IList<int> ids)
        {
            var carts = await cartRepository.Get(ids);
            if (carts is null) throw ExceptionUtil.NoSuchIdException(nameof(carts));
            if (carts.Count != ids.Count) throw ExceptionUtil.NoSuchIdException(nameof(carts));

            return carts;
        }

        public static async Task<Cart> CartAvailabilityWithReferences(ICartRepository cartRepository, IProductCartRepository productCartRepository, IProductRepository productRepository,
                                                        IShopRepository shopRepository, int id, Guid appKey)
        {
            var cart = await CartAvailability(cartRepository, id);

            var productCarts = await productCartRepository.GetByCartId(cart.Id);
            if (productCarts is null || !productCarts.Any()) throw new ArgumentNullException("Cannot create an order from an empty cart!");

            var productIds = productCarts.Select(x => x.ProductId);
            var products = await productRepository.Get(productIds.ToList());

            if (products is null || !products.Any()) throw new ArgumentNullException("Cannot create an order without referenced products!");

            if (products.Select(x => x.ShopId).Distinct().Count() > 1)
                throw new ArgumentException("Products from multiple shops in cart!");

            await ShopAuthorization(shopRepository, products.First().ShopId, appKey);

            foreach (var pc in productCarts)
            {
                pc.Product = products.FirstOrDefault(x => x.Id == pc.ProductId);
            }

            cart.ProductCarts = productCarts.ToHashSet();
            return cart;
        }

        public static async Task<IList<Cart>> CartAvailabilityWithReferences(ICartRepository cartRepository, IProductCartRepository productCartRepository, IProductRepository productRepository,
                                                        IShopRepository shopRepository, List<int> ids, Guid appKey)
        {
            var carts = await CartAvailability(cartRepository, ids);

            var productCarts = await productCartRepository.GetByCartIds(ids);
            if (productCarts is null || !productCarts.Any()) throw new ArgumentNullException("Cannot create an order from an empty cart!");

            var productIds = productCarts.Select(x => x.ProductId);
            var products = await productRepository.Get(productIds.ToList());

            if (products is null || !products.Any()) throw new ArgumentNullException("Cannot create an order without referenced products!");

            if (products.Select(x => x.ShopId).Distinct().Count() > 1)
                throw new ArgumentException("Products from multiple shops in cart!");

            await ShopAuthorization(shopRepository, products.First().ShopId, appKey);

            foreach (var pc in productCarts)
            {
                pc.Product = products.FirstOrDefault(x => x.Id == pc.ProductId);
            }

            foreach (var cart in carts)
            {
                cart.ProductCarts = productCarts.Where(pc => pc.CartId == cart.Id).ToHashSet();
            }
            return carts;
        }

        public static async Task DiscountRule(IShopRepository shopRepository, IDiscountRuleRepository discountRuleRepository, Guid appKey, int ruleId)
        {
            var rule = await discountRuleRepository.Get(ruleId);
            if (rule is null) throw ExceptionUtil.NoSuchIdException(nameof(rule));
            await Shop(shopRepository, rule.ShopId, appKey);

        }

        internal async static Task DiscountAction(IShopRepository shopRepository, IDiscountActionRepository discountActionRepository, Guid appKey, int actionId)
        {
            var action = await discountActionRepository.Get(actionId);
            if (action is null) throw ExceptionUtil.NoSuchIdException(nameof(action));
            await Shop(shopRepository, action.ShopId, appKey);
        }
    }
}
