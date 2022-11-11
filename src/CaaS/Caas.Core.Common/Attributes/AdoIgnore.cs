using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caas.Core.Common.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)
]
    public class AdoIgnoreAttribute : System.Attribute
    {
        public AdoIgnoreAttribute()
        {
        }
    }
}
