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

        public async Task<IEnumerable<Tenant>> GetAll() =>
                        await template.QueryAsync(reader =>
                new Tenant(reader.GetIntByName(nameof(Tenant.Id)), reader.GetStringByName(nameof(Tenant.Email)), reader.GetStringByName(nameof(Tenant.Name)))

             );

        public async Task<bool> Update(Tenant tenant) => (await template.UpdateAsync<Tenant>(tenant, new { Id = tenant.Id })) > 0;

        public async Task<int> Create(Tenant tenant) => (await template.InsertAsync<Tenant>(tenant))?.ElementAt(0) ?? 0;
    }
}
