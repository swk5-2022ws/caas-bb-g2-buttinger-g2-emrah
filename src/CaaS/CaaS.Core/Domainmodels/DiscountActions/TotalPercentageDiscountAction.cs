using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountActions
{
    [Serializable]
    public class TotalPercentageDiscountAction : DiscountActionBase
    {
        public float Percentage { get; init; }

        public override int ApplyPriority => (int) DiscountActionApplyPriority.PERCENTAGE;

        public TotalPercentageDiscountAction(float percentage)
        {
            if (percentage < 0 || percentage > 1.0) throw new ArgumentException($"Parameter {nameof(percentage)} must be > 0.0 and <= 1.0");
            Percentage = percentage;
        }

        public TotalPercentageDiscountAction(SerializationInfo info, StreamingContext context)
        {
            Percentage = (float) (info.GetValue(nameof(Percentage), typeof(float)) ?? 0.0);
        }

        public override double GetDiscount(Cart cart)
        {
            return cart.Price * Percentage;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Percentage), Percentage, typeof(float));
        }
    }
}
