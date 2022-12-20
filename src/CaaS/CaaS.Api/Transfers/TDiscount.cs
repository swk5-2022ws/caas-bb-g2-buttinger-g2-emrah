﻿using System.Text.Json.Serialization;

namespace CaaS.Api.Transfers
{
    public record TDiscount([property: JsonRequired] int Id, [property: JsonRequired] TDiscountAction discountAction, [property:JsonRequired] TDiscountRule DiscountRule);
    public record TDiscountAction([property: JsonRequired] int Id, [property: JsonRequired] string Name);
    public record TDiscountRule([property: JsonRequired] int Id, [property: JsonRequired] string Name);
}
