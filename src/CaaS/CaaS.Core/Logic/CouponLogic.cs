using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic.Util;
using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic
{
    public class CouponLogic : ICouponLogic
    {
        private readonly ICouponRepository couponRepository;
        private readonly ICartRepository cartRepository;
        private readonly IProductCartRepository productCartRepository;
        private readonly IProductRepository productRepository;
        private readonly IShopRepository shopRepository;

        public CouponLogic(ICouponRepository couponRepository, ICartRepository cartRepository, IProductCartRepository productCartRepository,
                           IProductRepository productRepository, IShopRepository shopRepository)
        {
            this.couponRepository = couponRepository ?? throw ExceptionUtil.ParameterNullException(nameof(couponRepository));
            this.cartRepository = cartRepository ?? throw ExceptionUtil.ParameterNullException(nameof(cartRepository));
            this.productCartRepository = productCartRepository;
            this.productRepository = productRepository;
            this.shopRepository = shopRepository ?? throw ExceptionUtil.ParameterNullException(nameof(shopRepository));
        }

        public async Task<bool> Apply(string couponKey, int cartId, Guid appKey)
        {
            var coupon = await couponRepository.GetByKey(couponKey) ?? throw ExceptionUtil.NoSuchIdException(nameof(Coupon));
            var cart = await cartRepository.Get(cartId) ?? throw ExceptionUtil.NoSuchIdException(nameof(Cart));

            var productCarts = await productCartRepository.GetByCartId(cartId);
            if (productCarts is null || productCarts.Count == 0) throw new ArgumentException("An empty cart can not be considered for a coupon!");

            var productIds = productCarts.Select(x => x.ProductId);
            var products = await productRepository.Get(productIds.ToList());

            if (products is null || products.Count == 0) throw ExceptionUtil.ParameterNullException(nameof(products));

            if (products.Select(x => x.ShopId).Distinct().Count() > 1 || coupon.ShopId != products.First().ShopId)
                throw new ArgumentException("No such product or coupon in shop");

            var couponShop = await shopRepository.Get(coupon.ShopId) ?? throw ExceptionUtil.NoSuchIdException(nameof(Shop));
            var productShop = await shopRepository.Get(products.First().ShopId) ?? throw ExceptionUtil.NoSuchIdException(nameof(Shop));

            if (couponShop.AppKey != appKey || couponShop.AppKey != appKey) throw new TypeAccessException("No such product or coupon in shop");

            return await couponRepository.Apply(couponKey, cartId);
        }

        public async Task<int> Create(Coupon coupon, int shopId, Guid appKey)
        {
            await Check.ShopAuthorization(shopRepository, shopId, appKey);

            coupon.ShopId = shopId;
            return await couponRepository.Create(coupon);
        }

        public async Task<bool> Delete(string couponKey, Guid appkey)
        {
            var coupon = await couponRepository.GetByKey(couponKey) ?? throw ExceptionUtil.NoSuchIdException(nameof(Coupon));

            await Check.ShopAuthorization(shopRepository, coupon.ShopId, appkey);
            if (coupon.CartId.HasValue) throw new ArgumentException("A already referenced coupon can not be deleted");

            return await couponRepository.Delete(coupon.Id);
        }

        public async Task<IList<Coupon>> GetCoupons(int shopId, Guid appKey)
        {
            await Check.ShopAuthorization(shopRepository, shopId, appKey);
            return await couponRepository.GetByShopId(shopId);
        }

    }
}
