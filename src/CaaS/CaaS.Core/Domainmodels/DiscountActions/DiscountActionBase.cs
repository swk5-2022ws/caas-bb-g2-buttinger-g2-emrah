using CaaS.Core.Interfaces.Discount;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountActions
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$discriminator")]
    [JsonDerivedType(typeof(FixedValueDiscountAction), nameof(FixedValueDiscountAction))]
    [JsonDerivedType(typeof(TotalPercentageDiscountAction), nameof(TotalPercentageDiscountAction))]
    [Serializable]
    public abstract class DiscountActionBase : IDiscountAction, ISerializable
    {
        /// <summary>
        /// Determines in which order multiple discounts must be applied. Highest priority discounts must be applied first.
        /// </summary>
        public abstract int ApplyPriority { get; }
        public abstract double GetDiscount(Cart cart);
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

        protected enum DiscountActionApplyPriority
        {
            INVALID = 0,
            PERCENTAGE = 1,
            FIXEDVALUE = 2
        }

        public static DiscountActionBase Deserialize(string json)
        {
            return JsonSerializer.Deserialize<DiscountActionBase>(json) ?? throw new ArgumentException($"Could not deserialize {json}");
        }

        public static string Serialize<T>(T toSerialize) where T : DiscountActionBase
        {
            return JsonSerializer.Serialize<DiscountActionBase>(toSerialize) ?? throw new ArgumentException($"Could not deserialize {toSerialize}");
        }
    }
}
