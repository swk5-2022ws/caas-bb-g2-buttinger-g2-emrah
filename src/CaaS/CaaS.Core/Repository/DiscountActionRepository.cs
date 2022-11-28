using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;
using CaaS.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository
{
    public class DiscountActionRepository : AdoRepository, IDiscountActionRepository
    {
        public DiscountActionRepository(IAdoTemplate template) : base(template) { }

        public async Task<int> Create(DiscountAction action) =>
             (await template.InsertAsync<DiscountAction>(action, true))?.ElementAt(0) ?? 0;

        public async Task<bool> Delete(int id) => 
            await template.DeleteAsync<DiscountAction>(new
        {
            Id = id
        });

        public async Task<DiscountAction?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(DiscountActionMapping.ReadDiscountActionMappingOnly, whereExpression: new
                    {
                        Id = id
                    });

        public async Task<IList<DiscountAction>> GetByShopId(int id) =>
             (IList<DiscountAction>)await template.QueryAsync(DiscountActionMapping.ReadDiscountActionMappingOnly,
            whereExpression: new { ShopId = id });

        public async Task<bool> Update(DiscountAction action) =>
            await template.UpdateAsync<DiscountAction>(action, whereExpression: new
            {
                Id = action.Id
            }) == 1;
    }
}
