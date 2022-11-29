using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    internal class TenantRepositoryStub : ITenantRepository
    {
        private readonly IDictionary<int, Tenant> tenants;

        public TenantRepositoryStub(IDictionary<int, Tenant> tenants)
        {
            this.tenants = tenants;
        }

        public Task<Tenant?> Get(int id)
        {
            tenants.TryGetValue(id, out Tenant? tenant);
            return Task.FromResult(tenant);
        }
    }
}
