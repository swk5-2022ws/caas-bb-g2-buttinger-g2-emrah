using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;

namespace CaaS.Core.Repository
{
    public class CouponRepository : AdoRepository, ICouponRepository
    {
        public CouponRepository(IAdoTemplate adoTemplate) : base(adoTemplate)
        {
        }

        public async Task<bool> Apply(string couponKey, int cartId)
        {
            var coupon = await GetByKey(couponKey);
            if(coupon == null || coupon.CartId.HasValue) return false;

            coupon.CartId = cartId;
            return await Update(coupon);
        }

        public async Task<int> Create(Coupon coupon) =>
            (await template.InsertAsync<Coupon>(coupon))?.ElementAt(0) ?? 0;


        public async Task<bool> Delete(int id)
        {
            Coupon? coupon = await Get(id);
            if (coupon == null || coupon.CartId.HasValue)
                return false;

            coupon.Deleted = DateTime.Now;
            return await Update(coupon);
        }

        public async Task<Coupon?> GetByKey(string couponKey) =>
            await template.QueryFirstOrDefaultAsync(CouponMapping.ReadCouponOnly, whereExpression: new
            {
                CouponKey = couponKey
            });       

        public async Task<IList<Coupon>> GetByShopId(int id) =>
             (IList<Coupon>)await template.QueryAsync(CouponMapping.ReadCouponOnly, whereExpression: new
             {
                 ShopId = id
             });

        private async Task<Coupon?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(CouponMapping.ReadCouponOnly, whereExpression: new
            {
                Id = id
            });

        private async Task<bool> Update(Coupon coupon) =>
            (await template.UpdateAsync<Coupon>(coupon, whereExpression: new { Id = coupon.Id })) == 1;
    }
}
