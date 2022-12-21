using AutoMapper;
using CaaS.Core.Domainmodels;

namespace CaaS.Api.Transfers.Mappings
{
    public class DiscountMapper : Profile
    {
        public DiscountMapper()
        {
            CreateMap<Discount, TDiscount>()
            .ConstructUsing(x =>
             new TDiscount(x.Id,
             new TDiscountAction(x.DiscountAction!.Id, x.DiscountAction.Name),
             new TDiscountRule(x.DiscountRule!.Id, x.DiscountRule.Name))
            );

            CreateMap<TDiscount, Discount>()
                .ConstructUsing(x =>
                new Discount(x.Id, x.DiscountRule.Id, x.discountAction.Id));
        }
    }
}
