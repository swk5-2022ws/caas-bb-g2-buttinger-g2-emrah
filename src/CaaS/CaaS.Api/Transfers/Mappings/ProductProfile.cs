using AutoMapper;
using CaaS.Core.Domainmodels;
using System;

namespace CaaS.Api.Transfers.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, TProduct>();
            
            CreateMap<Product, TCreateProduct>();
            CreateMap<TCreateProduct, Product>()
            .ForMember(x => x.Id, opt => opt.MapFrom(y => 0));

            CreateMap<Product, TEditProduct>();
            CreateMap<TEditProduct, Product>()
            .ForMember(x => x.ShopId, opt => opt.MapFrom(x => -1));
        }
    }
}
