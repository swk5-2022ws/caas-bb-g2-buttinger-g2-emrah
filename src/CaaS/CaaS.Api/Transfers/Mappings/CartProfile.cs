using AutoMapper;
using CaaS.Core.Domainmodels;
using Org.BouncyCastle.Utilities.Collections;

namespace CaaS.Api.Transfers.Mappings
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<ProductCart, TProductCart>();
            CreateMap<Cart, TCart>().ForMember(dest => dest.ProductCarts, opt => opt.MapFrom(src => src.ProductCarts));
        }
    }
}
