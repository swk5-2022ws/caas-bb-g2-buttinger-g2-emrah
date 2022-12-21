using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountRules
{
    /// <summary>
    /// A DiscountRuleset which triggers, if the price of a cart exceeds a defined value.
    /// </summary>
    [Serializable]
    public class TotalAmountDiscountRuleset : DiscountRulesetBase
    {
        public double MinimumTotalAmount { get; set; }

        public TotalAmountDiscountRuleset() { }

        [JsonConstructor]
        public TotalAmountDiscountRuleset(double minimumTotalAmount)
        {
            if (minimumTotalAmount < 0) throw new ArgumentException($"Parameter {nameof(minimumTotalAmount)} can not be negative.");
            MinimumTotalAmount = minimumTotalAmount;
        }

        public override bool IsQualifiedForDiscount(Cart cart)
        {
            return cart?.Price >= MinimumTotalAmount;
        }

        public TotalAmountDiscountRuleset(SerializationInfo info, StreamingContext context)
        {
            MinimumTotalAmount = (double)(info.GetValue(nameof(MinimumTotalAmount), typeof(double)) 
                ?? throw new SerializationException($"Can not deserialize null value of parameter {nameof(MinimumTotalAmount)}"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(MinimumTotalAmount), MinimumTotalAmount, typeof(double));
        }
    }
}
