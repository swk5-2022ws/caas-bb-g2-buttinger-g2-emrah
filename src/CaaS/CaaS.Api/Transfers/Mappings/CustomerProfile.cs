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
                    new Customer(x.id, x.shopId, x.name, x.email, x.cartId));

            CreateMap<TCreateCustomer, Customer>()
                .ConstructUsing(x =>
                    new Customer(0, x.shopId, x.name, x.email, null));
        }
    }
}
