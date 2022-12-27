using AutoMapper;
using CaaS.Core.Domainmodels;

namespace CaaS.Api.Transfers.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, TCustomer>();

            CreateMap<TCustomer, Customer>()
                .ConstructUsing(x =>
                    new Customer(x.id, x.shopId, x.name, x.email, x.cartId, null, null, null));

            CreateMap<TCreateCustomer, Customer>()
                .ConstructUsing(x =>
                    new Customer(0, x.shopId, x.name, x.email, null, null, null, null));
        }
    }
}
