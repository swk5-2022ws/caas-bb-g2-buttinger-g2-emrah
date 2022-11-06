using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository
{
    public class ShopRepository : IShopRepository
    {
        private readonly AdoTemplate template;

        public ShopRepository(AdoTemplate adoTemplate)
        {
            this.template = adoTemplate;
        }

        public async Task<int> Create(Shop shop) => (await template.InsertAsync<Shop>(shop))?.ElementAt(0) ?? 0;

        public async Task<Shop?> Get(int id)
        {
            return await template.QueryFirstOrDefaultAsync(reader =>
            {
                return new Shop(
                    (int)reader["Id"],
                    (int)reader["TenantId"],
                    (Guid)reader["AppKey"],
                    (string)reader["Label"]);
            },
                whereExpression:
                    new { Id = id }
                );
        }

        public async Task<bool> Update(Shop shop) => (await template.UpdateAsync<Shop>(shop, new { Id = shop.Id } )) > 0;
    }
}
