using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountActions
{
    [Serializable]
    public class FixedValueDiscountAction : DiscountActionBase
    {
        public float Value { get; init; }

        public override int ApplyPriority => (int) DiscountActionApplyPriority.FIXEDVALUE;

        public FixedValueDiscountAction(float value)
        {
            if (value < 0) throw new ArgumentException($"Parameter {nameof(value)} must be > 0.0");
            Value = value;
        }

        public FixedValueDiscountAction(SerializationInfo info, StreamingContext context)
        {
            Value = (float)(info.GetValue(nameof(Value), typeof(float)) ?? throw new SerializationException($"Can not deserialize null value of parameter {nameof(Value)}"));
        }

        public override double GetDiscount(Cart cart)
        {
            if (cart.Price <= Value) return cart.Price;
            return cart.Price - Value;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value), Value, typeof(float));
        }
    }
}