using CaaS.Core.Domainmodels;
using CaaS.Core.Engines;
using CaaS.Core.Interfaces.Engines;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic.Util;
using CaaS.Core.Repository;
using CaaS.Core.Repository.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CaaS.Core.Logic
{
    public class DiscountLogic : IDiscountLogic
    {
        private readonly IDiscountRepository discountRepository;
        private readonly IShopRepository shopRepository;
        private readonly ICartRepository cartRepository;
        private readonly IProductCartRepository productCartRepository;
        private readonly IDiscountCartRepository discountCartRepository;
        private readonly IProductRepository productRepository;
        private readonly IDiscountActionRepository discountActionRepository;
        private readonly IDiscountRuleRepository discountRuleRepository;
        private readonly ICustomerRepository customerRepository;

        public DiscountLogic(IDiscountRepository discountRepository, IShopRepository shopRepository,
            ICartRepository cartRepository, IProductCartRepository productCartRepository,
            IDiscountCartRepository discountCartRepository, IProductRepository productRepository,
            IDiscountActionRepository discountActionRepository, IDiscountRuleRepository discountRuleRepository,
            ICustomerRepository customerRepository)
        {
            this.discountRepository = discountRepository;
            this.shopRepository = shopRepository;
            this.cartRepository = cartRepository;
            this.productCartRepository = productCartRepository;
            this.discountCartRepository = discountCartRepository;
            this.productRepository = productRepository;
            this.discountActionRepository = discountActionRepository;
            this.discountRuleRepository = discountRuleRepository;
            this.customerRepository = customerRepository;
        }

        public async Task<Discount> Get(Guid appKey, int discountId)
        {
            return await GetDiscountWithAuthorizationCheck(appKey, discountId); ;
        }

        public async Task<IEnumerable<Discount>> GetByShopId(Guid appKey, int shopId)
        {
            await AuthorizationCheck(shopId, appKey);
            return await discountRepository.GetByShopId(shopId);
        }

        public async Task<int> Create(Guid appKey, Discount discount)
        {
            var rule = await discountRuleRepository.Get(discount.RuleId);
            var action = await discountActionRepository.Get(discount.ActionId);

            if (rule == null) throw new KeyNotFoundException($"The rule with id={discount.RuleId} is currently not available.");
            if (action == null) throw new KeyNotFoundException($"The action with id={discount.ActionId} is currently not available.");

            await AuthorizationCheck(rule.ShopId, appKey);
            await AuthorizationCheck(action.ShopId, appKey);

            return await discountRepository.Create(new Discount(0, rule.Id, action.Id));
        }


        public async Task<bool> Delete(Guid appKey, int discountId)
        {
            await GetDiscountWithAuthorizationCheck(appKey, discountId);
            return await discountRepository.Delete(discountId);
        }

        public async Task<IEnumerable<Discount>> GetAvailableDiscountsByCartId(Guid appKey, int cartId)
        {
            var cart = await cartRepository.Get(cartId) ?? throw new KeyNotFoundException($"The cart with id={cartId} is currently not available.");
            if (!cart.CustomerId.HasValue) throw new ArgumentException($"Cart {cart?.Id} is not yet qualified for discounts. Discounts must be applied during check out.");
            var customer = await customerRepository.Get(cart.CustomerId.Value) ?? throw new KeyNotFoundException($"The cart with id={cartId} is currently not available.");

            await AddProductsToCart(appKey, cart, customer.ShopId);

            var allDiscounts = await discountRepository.GetByShopId(customer.ShopId);

            IDiscountEngine discountEngine = new DiscountEngine(allDiscounts);
            // do not persist the cart, we are just using DiscountEngine to retrieve possible discounts
            discountEngine.ApplyValidDiscounts(cart);

            return cart.Discounts ?? new List<Discount>();
        }

        private async Task AddProductsToCart(Guid appKey, Cart cart, int shopId)
        {
            var productCarts = await productCartRepository.GetByCartId(cart.Id);

            // TODO util class for adding products
            if (productCarts is not null && productCarts.Count > 0)
            {
                var productIds = productCarts.Select(x => x.ProductId);
                var products = await productRepository.Get(productIds.ToList());

                if (products is not null && products.Count > 0)
                {
                    if (products.Select(x => x.ShopId).Distinct().Count() > 1)
                        throw new ArgumentException("No such product or coupon in shop");

                    shopId = products.First().ShopId;
                    await Check.ShopAuthorization(shopRepository, shopId, appKey);

                    foreach (var pc in productCarts)
                    {
                        pc.Product = products.FirstOrDefault(x => x.Id == pc.ProductId);
                    }
                }
                cart.ProductCarts = productCarts.ToHashSet();
            }
        }

        public async Task AddDiscountsToCart(Guid appKey, int cartId, IList<int> requestedDiscountIds)
        {
            if (requestedDiscountIds == null || requestedDiscountIds.Count == 0)
                throw new ArgumentNullException($"No discounts provided.");

            Cart cart = await GetCartById(cartId);
            int shopId = await GetShopIdByCartId(cartId);
            await AuthorizationCheck(shopId, appKey);

            await AddProductsToCart(appKey, cart, shopId);

            var discountsValidatedByShopId = await ValidateDiscountsByShopId(requestedDiscountIds, shopId);
            var validDiscounts = ValidateDiscountsByCart(cart, discountsValidatedByShopId);

            using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
            await UpdateDiscounts(cartId, validDiscounts);
            transactionScope.Complete();
        }

        /// <summary>
        /// Checks if requestedDiscountIds are valid for the passed shopId
        /// </summary>
        /// <param name="requestedDiscountIds"></param>
        /// <param name="shopId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<IList<Discount>> ValidateDiscountsByShopId(IList<int> requestedDiscountIds, int shopId)
        {
            // fetch all available discounts from shop to double check the requested discounts
            var allDiscountsByShopId = await discountRepository.GetByShopId(shopId);

            // extract the requested discounts
            var discountIdsChecked = allDiscountsByShopId.Select(x => x.Id).Intersect(requestedDiscountIds).ToList();
            if (discountIdsChecked.Count != requestedDiscountIds.Count)
            {
                throw new ArgumentException($"The passed discount ids do not satisfy this shops ({shopId}) current discount policy." +
                    $" Received {requestedDiscountIds.Count} discounts, but only {allDiscountsByShopId.Count} are valid");
            }

            return allDiscountsByShopId.Where(x => discountIdsChecked.Contains(x.Id)).ToList();
        }

        /// <summary>
        /// Returns a Cart by cartId. 
        /// </summary>
        /// <exception cref="KeyNotFoundException">If cart does not exist</exception>
        /// <exception cref="ArgumentException">If cart is not yet linked to a customer (=at least in payment process)</exception>
        private async Task<Cart> GetCartById(int cartId)
        {
            var cart = await cartRepository.Get(cartId) ?? throw new KeyNotFoundException($"The cart with id={cartId} is currently not available.");
            if (!cart.CustomerId.HasValue) throw new ArgumentException($"Cart {cart?.Id} is not yet qualified for discounts. Discounts must be applied during check out.");
            return cart;
        }

        /// <summary>
        /// Returns a shopId by cartId. Throws ArgumentException if cart is not yet linked to a shop.            
        /// </summary>
        private async Task<int> GetShopIdByCartId(int cartId)
        {
            var productCarts = await productCartRepository.GetByCartId(cartId);
            if (productCarts == null || productCarts.Count == 0)
                throw new ArgumentException($"You can't apply a discount to a empty cart.");

            var productCart = productCarts.First();
            var products = await productRepository.Get(productCarts.Select(x => x.ProductId).ToList());
            if (products is null || products.Count == 0) // this should never happen
                throw new Exception($"The product ({productCart.ProductId}) is currently not available. Please contact your shop vendor.");

            var shopId = products.First().ShopId;
            return shopId;
        }

        /// <summary>
        /// Returns discounts which are respecting the discount rules for the passed cart.
        /// </summary>
        private static IList<Discount> ValidateDiscountsByCart(Cart cart, IList<Discount> discounts)
        {
            IDiscountEngine discountEngine = new DiscountEngine(discounts);
            discountEngine.ApplyValidDiscounts(cart);
            IList<Discount> validDiscounts = cart.Discounts ?? new List<Discount>();
            return validDiscounts;
        }

        /// <summary>
        /// Deletes currently to cart linked discounts. Links passed discounts to cart.
        /// Top caller must take care of transactional use.
        /// </summary>
        private async Task UpdateDiscounts(int cartId, IList<Discount> validDiscounts)
        {
            IList<Task> deleteTasks = await GetDiscountCartDeleteTasks(cartId);
            IList<Task> createTasks = GetDiscountCartCreateTasks(cartId, validDiscounts);

            await Task.WhenAll(
                createTasks.Concat(deleteTasks)
                );
        }

        private IList<Task> GetDiscountCartCreateTasks(int cartId, IList<Discount> validDiscounts)
        {
            var createTasks = new List<Task>();
            foreach (var validDiscount in validDiscounts)
            {
                createTasks.Add(
                    discountCartRepository.Create(
                        new DiscountCart(cartId, validDiscount.Id)
                    )
                );
            }

            return createTasks;
        }

        private async Task<IList<Task>> GetDiscountCartDeleteTasks(int cartId)
        {
            var oldDiscounts = await discountCartRepository.GetByCartId(cartId);

            var deleteTasks = new List<Task>();
            foreach (var oldDiscount in oldDiscounts)
            {
                deleteTasks.Add(
                    discountCartRepository.Delete(oldDiscount)
                );
            }

            return deleteTasks;
        }

        private async Task AuthorizationCheck(int shopId, Guid appKey)
        {
            var availableShop = await shopRepository.Get(shopId);

            if (availableShop is null) throw new KeyNotFoundException($"The shop with id={shopId} is currently not available.");
            if (availableShop.AppKey != appKey) throw new UnauthorizedAccessException($"You have not the right privileges.");
        }

        private async Task<Discount> GetDiscountWithAuthorizationCheck(Guid appKey, int discountId)
        {
            var discount = await discountRepository.Get(discountId) ?? throw new KeyNotFoundException($"The discount with id={discountId} is currently not available.");
            var shopId = discount.DiscountAction!.ShopId;
            await AuthorizationCheck(shopId, appKey);
            return discount;
        }
    }
}
