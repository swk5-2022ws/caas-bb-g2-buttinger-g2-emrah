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

        public Task<int> Create(Tenant tenant)
        {
            throw new NotImplementedException();
        }

        public Task<Tenant?> Get(int id)
        {
            tenants.TryGetValue(id, out Tenant? tenant);
            return Task.FromResult(tenant);
        }

        public Task<IEnumerable<Tenant>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Tenant tenant)
        {
            throw new NotImplementedException();
        }
    }
}
