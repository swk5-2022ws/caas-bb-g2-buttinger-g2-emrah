using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Transferrecordes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    public class CouponRepositoryStub : ICouponRepository
    {
        private readonly IDictionary<int, Coupon> coupons;

        public CouponRepositoryStub(IDictionary<int, Coupon> coupons)
        {
            this.coupons = coupons;
        }

        public Task<bool> Apply(string couponKey, int cartId)
        {
            var coupon = coupons.Values.FirstOrDefault(x => x.CouponKey == couponKey);
            if (coupon is null) return Task.FromResult(false);

            coupon.CartId = cartId;
            return Task.FromResult(true);
        }

        public Task<int> Create(Coupon coupon)
        {
            var id = coupons.Keys.Max() + 1;
            coupons.Add(id, coupon);
            return Task.FromResult(id);
        }

        public Task<bool> Delete(int id)
        {
            if (coupons.TryGetValue(id, out Coupon? coupon))
            {
                coupons.Remove(id);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<Coupon?> GetByKey(string key) =>Task.FromResult(coupons.Values.FirstOrDefault(x => x.CouponKey == key));
        public Task<IList<Coupon>> GetByShopId(int id) => Task.FromResult((IList<Coupon>)coupons.Values.Where(x => x.ShopId == id));
    }
}
