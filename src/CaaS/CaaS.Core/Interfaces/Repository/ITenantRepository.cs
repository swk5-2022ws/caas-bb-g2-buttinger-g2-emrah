using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Repository
{
    public interface ITenantRepository
    {
        Task<Tenant?> Get(int id);
        Task<IEnumerable<Tenant>> GetAll();

        Task<bool> Update(Tenant tenant);

        Task<int> Create(Tenant tenant);
    }
}
