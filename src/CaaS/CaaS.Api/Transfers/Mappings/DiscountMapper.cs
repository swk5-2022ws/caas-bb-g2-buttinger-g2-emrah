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

            CreateMap<DiscountAction, TDiscountAction>()
                .ConstructUsing(x => new TDiscountAction(x.Id, x.Name));

            CreateMap<DiscountRule, TDiscountRule>()
                .ConstructUsing(x => new TDiscountRule(x.Id, x.Name));

            CreateMap<TCreateDiscountAction, DiscountAction>()
                .ConstructUsing(x =>
                new DiscountAction(0, x.ShopId, x.Name, x.ActionObject));

            CreateMap<TCreateDiscountRule, DiscountRule>()
           .ConstructUsing(x =>
           new DiscountRule(0, x.ShopId, x.Name, x.RuleSet));
        }
    }
}
