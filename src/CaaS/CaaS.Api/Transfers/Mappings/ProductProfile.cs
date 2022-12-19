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
            .ConstructUsing(x => new Product(0, x.ShopId, x.Description, x.ImageUrl, x.Label, x.Price));

            CreateMap<Product, TEditProduct>();
            CreateMap<TEditProduct, Product>()
            .ConstructUsing(x => new Product(x.Id, -1, x.Description, x.ImageUrl, x.Label, x.Price));
        }
    }
}
