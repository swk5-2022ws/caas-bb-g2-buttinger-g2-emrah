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

        public DiscountLogic(IDiscountRepository discountRepository, IShopRepository shopRepository, ICartRepository cartRepository)
        {
            this.discountRepository = discountRepository;
            this.shopRepository = shopRepository;
            this.cartRepository = cartRepository;
        }

        public async Task<IEnumerable<Domainmodels.Discount>> GetAvailableDiscountsByCartId(Guid appKey, int id)
        {
            await AuthorizationCheck(id, appKey);

            var cart = await cartRepository.Get(id) ?? throw new KeyNotFoundException($"The cart with id={id} is currently not available.");
            var allDiscounts = await discountRepository.GetByShopId(id);

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
