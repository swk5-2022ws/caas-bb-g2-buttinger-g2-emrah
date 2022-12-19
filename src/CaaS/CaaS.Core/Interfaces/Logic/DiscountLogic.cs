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

namespace CaaS.Core.Interfaces.Logic
{
    public class DiscountLogic : IDiscountLogic
    {
        private readonly IDiscountRepository discountRepository;
        private readonly IShopRepository shopRepository;
        private readonly ICartRepository cartRepository;
        private readonly IProductCartRepository productCartRepository;

        public DiscountLogic(IDiscountRepository discountRepository, IShopRepository shopRepository, 
            ICartRepository cartRepository,
            IProductCartRepository productCartRepository)
        {
            this.discountRepository = discountRepository;
            this.shopRepository = shopRepository;
            this.cartRepository = cartRepository;
            this.productCartRepository = productCartRepository;
        }

        public async Task AddDiscountsToCart(Guid appKey, int cartId, IList<int> discountIds)
        {
            var cart = (await cartRepository.Get(cartId)) ?? throw new KeyNotFoundException($"The cart with id={cartId} is currently not available.");
            if (!cart.CustomerId.HasValue) throw new ArgumentException($"Cart {cart?.Id} is not yet qualified for discounts. Discounts must be applied during check out.");
            
            // we need products to get the shopId since we decided to not store it in a cart since it is not really necessary
            var productCarts = (await productCartRepository.GetByCartId(cartId));
            if(productCarts == null || productCarts.Count == 0)
                throw new ArgumentException($"You can't apply a discount to a empty cart.");

            var shopId = productCarts.First().Product!.ShopId;

            // fetch all available discounts from a shop to double check the requested discounts
            var discounts = await discountRepository.GetByShopId(shopId);

            // extract the requested discounts
            discounts = discounts.Where(x => discountIds.Contains(x.Id)).ToList();

            IDiscountEngine discountEngine = new DiscountEngine(discounts);

            discountEngine.ApplyValidDiscounts(cart);
            var validDiscounts = cart.Discounts;
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

        private async Task AuthorizationCheck(int shopId, Guid appKey)
        {
            var availableShop = await shopRepository.Get(shopId);

            if (availableShop is null) throw new KeyNotFoundException($"The shop with id={shopId} is currently not available.");
            if (availableShop.AppKey != appKey) throw new UnauthorizedAccessException($"You have not the right privileges.");
        }
    }
}
