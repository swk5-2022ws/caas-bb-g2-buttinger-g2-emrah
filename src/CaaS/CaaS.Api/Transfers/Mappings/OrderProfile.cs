using AutoMapper;
using CaaS.Core.Domainmodels;

namespace CaaS.Api.Transfers.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Product, TProduct>();
            CreateMap<ProductCart, TProductCart>().ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));
            CreateMap<Cart, TCart>().ForMember(dest => dest.ProductCarts, opt => opt.MapFrom(src => src.ProductCarts));
            CreateMap<Order, TOrder>().ForMember(dest => dest.Cart, opt => opt.MapFrom(src => src.Cart));
        }
    }
}
