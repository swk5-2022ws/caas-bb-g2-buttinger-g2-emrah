using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Interfaces.Discount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountRules
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$discriminator")]
    [JsonDerivedType(typeof(DateDiscountRuleset), nameof(DateDiscountRuleset))]
    [JsonDerivedType(typeof(TotalAmountDiscountRuleset), nameof(TotalAmountDiscountRuleset))]
    [Serializable]
    public abstract class DiscountRulesetBase : IDiscountRule, ISerializable
    {
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
        public abstract bool IsQualifiedForDiscount(Cart cart);

        public static DiscountRulesetBase Deserialize(string json)
        {
            return JsonSerializer.Deserialize<DiscountRulesetBase>(json) ?? throw new ArgumentException($"Could not deserialize {json}");
        }

        public static string Serialize<T>(T toSerialize) where T : DiscountRulesetBase
        {
            return JsonSerializer.Serialize<DiscountRulesetBase>(toSerialize) ?? throw new ArgumentException($"Could not deserialize {toSerialize}");
        }
    }
}
