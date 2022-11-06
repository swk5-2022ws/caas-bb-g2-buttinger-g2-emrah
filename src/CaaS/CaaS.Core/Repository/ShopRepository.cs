using Caas.Core.Common;
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

        public Task Create(Shop shop)
        {
            throw new NotImplementedException();
        }

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

        public Task Update(Shop shop)
        {
            throw new NotImplementedException();
        }
    }
}
