using AutoMapper;
using CaaS.Core.Domainmodels;
using System;

namespace CaaS.Api.Transfers.Mappings
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupon, TCoupon>();
            
            CreateMap<Coupon, TCreateCoupon>();
            CreateMap<TCreateCoupon, Coupon>()
            .ConstructUsing(x => new Coupon(0, x.ShopId, x.Value) { CouponKey = Guid.NewGuid().ToString()});
        }
    }
}
