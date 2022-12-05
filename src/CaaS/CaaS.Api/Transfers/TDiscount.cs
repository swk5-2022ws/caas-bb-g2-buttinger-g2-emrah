using System.Text.Json.Serialization;

namespace CaaS.Api.Transfers
{
    public record TDiscount([property: JsonRequired] int id, [property: JsonRequired] TDiscountAction discountAction, [property:JsonRequired] TDiscountRule discountRUle);
    public record TDiscountAction([property: JsonRequired] int id, [property: JsonRequired] string Name);
    public record TDiscountRule([property: JsonRequired] int id, [property: JsonRequired] string Name);
}
