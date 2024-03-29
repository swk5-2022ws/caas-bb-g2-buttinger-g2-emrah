﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountActions
{
    [Serializable]
    public class FixedValueDiscountAction : DiscountActionBase
    {
        public double Value { get; init; }

        [JsonIgnore]
        public override int ApplyPriority => (int) DiscountActionApplyPriority.FIXEDVALUE;
        [JsonIgnore]
        public override int SubApplyPriority => (int)Value;

        public FixedValueDiscountAction() {}

        [JsonConstructor]
        public FixedValueDiscountAction(double value)
        {
            if (value < 0) throw new ArgumentException($"Parameter {nameof(value)} must be > 0.0");
            Value = value;
        }

        public FixedValueDiscountAction(SerializationInfo info, StreamingContext context)
        {
            Value = (double)(info.GetValue(nameof(Value), typeof(double)) ?? throw new SerializationException($"Can not deserialize null value of parameter {nameof(Value)}"));
        }

        public override double GetDiscount(Cart cart)
        {
            if (cart.Price <= Value) return cart.Price;
            return Value;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value), Value, typeof(double));
            info.SetType(typeof(FixedValueDiscountAction));
        }
    }
}