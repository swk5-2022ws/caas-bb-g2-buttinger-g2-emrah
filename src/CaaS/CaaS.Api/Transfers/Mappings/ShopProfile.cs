using AutoMapper;
using CaaS.Core.Domainmodels;
using System;

namespace CaaS.Api.Transfers.Mappings
{
    public class ShopProfile : Profile
    {
        public ShopProfile()
        {
            CreateMap<Shop, TShop>();

            CreateMap<TCreateShop, Shop>()
                .ConstructUsing(x => new Shop(0, x.TenantId, Guid.NewGuid(), x.Label));

            CreateMap<TShop, Shop>().ReverseMap();
                

        }
    }
}
