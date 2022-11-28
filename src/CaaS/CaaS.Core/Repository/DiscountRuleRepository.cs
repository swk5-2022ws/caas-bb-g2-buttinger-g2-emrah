using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository
{
    public class DiscountRuleRepository : AdoRepository, IDiscountRuleRepository
    {
        public DiscountRuleRepository(IAdoTemplate template) : base(template) { }

        public async Task<int> Create(DiscountRule action) =>
                    (await template.InsertAsync<DiscountRule>(action, true))?.ElementAt(0) ?? 0;

        public async Task<bool> Delete(int id) =>
            await template.DeleteAsync<DiscountRule>(new
            {
                Id = id
            });

        public async Task<DiscountRule?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(DiscountRuleMapping.ReadDiscountMappingOnly, whereExpression: new
            {
                Id = id
            });

        public async Task<IList<DiscountRule>> GetByShopId(int id) =>
             (IList<DiscountRule>)await template.QueryAsync(DiscountRuleMapping.ReadDiscountMappingOnly,
            whereExpression: new { ShopId = id });

        public async Task<bool> Update(DiscountRule action) =>
            await template.UpdateAsync<DiscountRule>(action, whereExpression: new
            {
                Id = action.Id
            }) == 1;
    }
}
