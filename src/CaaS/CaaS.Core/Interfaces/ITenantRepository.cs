using CaaS.Core.Transferinterfacees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces
{
    public interface ITenantRepository
    {
        TTenant Get(Guid id);
    }
}
