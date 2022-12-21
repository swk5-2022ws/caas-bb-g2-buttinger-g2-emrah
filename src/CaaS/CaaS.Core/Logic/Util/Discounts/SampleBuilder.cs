using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic.Util.Discounts
{
    internal static class SampleBuilder
    {
        /// <summary>
        /// Constructs all sub types of the passed type argument. CAUTION: subclasses need a parameterless constructor
        /// </summary>
        public static IEnumerable<T> BuildSamples<T>() where T : class, ISerializable
        {
            var types = ReflectionUtil.GetSubTypes<T>();
            foreach (var type in types)
            {
                yield return (T)Activator.CreateInstance(type)!;
            }
        }
    }
}
