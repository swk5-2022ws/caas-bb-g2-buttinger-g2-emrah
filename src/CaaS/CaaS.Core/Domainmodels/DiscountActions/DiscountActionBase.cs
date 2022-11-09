using CaaS.Core.Interfaces.Discount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountActions
{
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
    }
}
