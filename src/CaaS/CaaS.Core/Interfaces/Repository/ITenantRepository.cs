using CaaS.Core.Transferclasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface ITenantRepository
    {
        TTenant Get(Guid id);
    }
}
