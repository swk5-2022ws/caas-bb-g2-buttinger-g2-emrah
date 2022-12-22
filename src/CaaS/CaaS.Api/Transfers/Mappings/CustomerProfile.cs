using AutoMapper;
using CaaS.Core.Domainmodels;

namespace CaaS.Api.Transfers.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, TCustomer>().ReverseMap();
            CreateMap<TCreateCustomer, Customer>();
        }
    }
}
