using Caas.Core.Common;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AdoTemplate template;

        public TenantRepository(AdoTemplate adoTemplate)
        {
            this.template = adoTemplate;
        }

        private Tenant MapRowToTenant(IDataRecord row) =>
            new(
                (int)row["Id"],
                (string)row["email"],
                (string)row["name"]
            );

        public async Task<Tenant?> Get(int id)
        {
            return await template.QueryFirstOrDefaultAsync(
                MapRowToTenant,
                whereExpression: new { Id = id });
        }
    }
}
