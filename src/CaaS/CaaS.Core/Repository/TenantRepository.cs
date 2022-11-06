using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CaaS.Core.Repository
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AdoTemplate template;

        public TenantRepository(AdoTemplate adoTemplate)
        {
            this.template = adoTemplate;
        }

        public async Task<Tenant?> Get(int id)
        {
            return await template.QueryFirstOrDefaultAsync(reader =>
            {
                return new Tenant((int)reader[0], (string)reader["Email"], (string)reader["Name"]);
            },
            whereExpression:
                new { Id = id }
            );
        }
    }
}
