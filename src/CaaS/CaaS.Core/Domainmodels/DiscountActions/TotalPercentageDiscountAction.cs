using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountActions
{
    [Serializable]
    public class TotalPercentageDiscountAction : DiscountActionBase
    {
        public double Percentage { get; init; }

        public override int ApplyPriority => (int) DiscountActionApplyPriority.PERCENTAGE;

        [JsonConstructor]
        public TotalPercentageDiscountAction(double percentage)
        {
            if (percentage < 0 || percentage > 1.0) throw new ArgumentException($"Parameter {nameof(percentage)} must be > 0.0 and <= 1.0");
            Percentage = percentage;
        }

        public TotalPercentageDiscountAction(SerializationInfo info, StreamingContext context)
        {
            Percentage = (double) (info.GetValue(nameof(Percentage), typeof(double)) ?? 0.0);
        }

        public override double GetDiscount(Cart cart)
        {
            return cart.Price * Percentage;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Percentage), Percentage, typeof(double));
            info.SetType(typeof(TotalPercentageDiscountAction));
        }
    }
}
