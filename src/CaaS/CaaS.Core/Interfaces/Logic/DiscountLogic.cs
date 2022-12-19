using CaaS.Core.Domainmodels;
using CaaS.Core.Engines;
using CaaS.Core.Interfaces.Engines;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CaaS.Core.Interfaces.Logic
{
    public class DiscountLogic : IDiscountLogic
    {
        private readonly IDiscountRepository discountRepository;
        private readonly IShopRepository shopRepository;
        private readonly ICartRepository cartRepository;
        private readonly IProductCartRepository productCartRepository;
        private readonly IDiscountCartRepository discountCartRepository;
        private readonly IProductRepository productRepository;

        public DiscountLogic(IDiscountRepository discountRepository, IShopRepository shopRepository,
            ICartRepository cartRepository,
            IProductCartRepository productCartRepository,
            IDiscountCartRepository discountCartRepository, IProductRepository productRepository)
        {
            this.discountRepository = discountRepository;
            this.shopRepository = shopRepository;
            this.cartRepository = cartRepository;
            this.productCartRepository = productCartRepository;
            this.discountCartRepository = discountCartRepository;
            this.productRepository = productRepository;
        }

        public async Task<IEnumerable<Domainmodels.Discount>> GetAvailableDiscountsByCartId(Guid appKey, int cartId)
        {
            var cart = await cartRepository.Get(cartId) ?? throw new KeyNotFoundException($"The cart with id={cartId} is currently not available.");
            if (!cart.CustomerId.HasValue) throw new ArgumentException($"Cart {cart?.Id} is not yet qualified for discounts. Discounts must be applied during check out.");

            var allDiscounts = await discountRepository.GetByShopId(cartId);

            IDiscountEngine discountEngine = new DiscountEngine(allDiscounts);
            // do not persist the cart, we are just using DiscountEngine to retrieve possible discounts
            discountEngine.ApplyValidDiscounts(cart);

            return cart.Discounts ?? new List<Domainmodels.Discount>();
        }

        public async Task AddDiscountsToCart(Guid appKey, int cartId, IList<int> requestedDiscountIds)
        {
            if (requestedDiscountIds == null || requestedDiscountIds.Count == 0)
                throw new ArgumentNullException($"No discounts provided.");

            Cart cart = await GetCartById(cartId);
            int shopId = await GetShopIdByCartId(cartId);
            await AuthorizationCheck(shopId, appKey);

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
        private async Task<IList<Domainmodels.Discount>> ValidateDiscountsByShopId(IList<int> requestedDiscountIds, int shopId)
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
            var cart = (await cartRepository.Get(cartId)) ?? throw new KeyNotFoundException($"The cart with id={cartId} is currently not available.");
            if (!cart.CustomerId.HasValue) throw new ArgumentException($"Cart {cart?.Id} is not yet qualified for discounts. Discounts must be applied during check out.");
            return cart;
        }

        /// <summary>
        /// Returns a shopId by cartId. Throws ArgumentException if cart is not yet linked to a shop.            
        /// </summary>
        private async Task<int> GetShopIdByCartId(int cartId)
        {
            var productCarts = (await productCartRepository.GetByCartId(cartId));
            if (productCarts == null || productCarts.Count == 0)
                throw new ArgumentException($"You can't apply a discount to a empty cart.");

            var productCart = productCarts.First();
            var product = await productRepository.Get(productCart.ProductId);
            if (product == null) // this should never happen
                throw new Exception($"The product ({productCart.ProductId}) is currently not available. Please contact your shop vendor.");
             
            var shopId = product.ShopId;
            return shopId;
        }

        /// <summary>
        /// Returns discounts which are respecting the discount rules for the passed cart.
        /// </summary>
        private static IList<Domainmodels.Discount> ValidateDiscountsByCart(Cart cart, IList<Domainmodels.Discount> discounts)
        {
            IDiscountEngine discountEngine = new DiscountEngine(discounts);
            discountEngine.ApplyValidDiscounts(cart);
            IList<Domainmodels.Discount> validDiscounts = cart.Discounts ?? new List<Domainmodels.Discount>();
            return validDiscounts;
        }

        /// <summary>
        /// Deletes currently to cart linked discounts. Links passed discounts to cart.
        /// Top caller must take care of transactional use.
        /// </summary>
        private async Task UpdateDiscounts(int cartId, IList<Domainmodels.Discount> validDiscounts)
        {
            IList<Task> deleteTasks = await GetDiscountCartDeleteTasks(cartId);
            IList<Task> createTasks = GetDiscountCartCreateTasks(cartId, validDiscounts);

            await Task.WhenAll(
                createTasks.Concat(deleteTasks)
                );
        }

        private IList<Task> GetDiscountCartCreateTasks(int cartId, IList<Domainmodels.Discount> validDiscounts)
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
    }
}
