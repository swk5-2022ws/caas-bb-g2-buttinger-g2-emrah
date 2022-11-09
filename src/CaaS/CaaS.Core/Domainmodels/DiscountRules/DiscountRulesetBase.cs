using CaaS.Core.Interfaces.Discount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountRules
{
    [Serializable]
    public abstract class DiscountRulesetBase : IDiscountRule, ISerializable
    {
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
        public abstract bool IsQualifiedForDiscount(Cart cart);
    }
}
