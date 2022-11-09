using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountRules
{
    /// <summary>
    /// A DiscountRuleset which triggers, if the price of a cart exceeds a defined value.
    /// </summary>
    [Serializable]
    public class TotalAmountDiscountRuleset : DiscountRulesetBase
    {
        public double MinimumTotalAmout { get; init; }

        public TotalAmountDiscountRuleset(double minimumTotalAmout)
        {
            if (minimumTotalAmout < 0) throw new ArgumentException($"Parameter {nameof(minimumTotalAmout)} can not be negative.");

            MinimumTotalAmout = minimumTotalAmout;
        }

        public TotalAmountDiscountRuleset(SerializationInfo info, StreamingContext context)
        {
            MinimumTotalAmout = (double)(info.GetValue(nameof(MinimumTotalAmout), typeof(float)) 
                ?? throw new SerializationException($"Can not deserialize null value of parameter {nameof(MinimumTotalAmout)}"));
        }

        public override bool IsQualifiedForDiscount(Cart cart)
        {
            return cart?.Price >= MinimumTotalAmout;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(MinimumTotalAmout), MinimumTotalAmout, typeof(double));
        }
    }
}
