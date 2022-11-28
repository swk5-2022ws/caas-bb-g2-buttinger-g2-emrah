using Caas.Core.Common.Ado;
using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;

namespace CaaS.Core.Repository
{
    public class TenantRepository : AdoRepository, ITenantRepository
    {
        public TenantRepository(IAdoTemplate adoTemplate) : base(adoTemplate)
        {

        }

        public async Task<Tenant?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(reader => 
                new Tenant(reader.GetIntByName(nameof(Tenant.Id)), reader.GetStringByName(nameof(Tenant.Email)), reader.GetStringByName(nameof(Tenant.Name))),
                whereExpression:
                    new { Id = id }
             );
    }
}
