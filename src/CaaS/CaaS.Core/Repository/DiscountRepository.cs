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

    public class DiscountRepository : AdoRepository, IDiscountRepository
    {
        public DiscountRepository(IAdoTemplate template) : base(template) { }

        public async Task<Discount?> Get(int id) =>
        await template.QueryFirstOrDefaultAsync(DiscountMapping.ReadDiscountOnly, whereExpression: new
        {
            Id = id
        });

        public async Task<IList<Discount>> GetByShopId(int id) =>
            (IList<Discount>)await template.QueryAsync(DiscountMapping.ReadDiscountWithActionAndRule,
            joins: $"INNER JOIN DiscountRule r on r.Id = t.ActionId " +
                   $"INNER JOIN DiscountAction a on a.Id = t.ActionId ",
            whereExpression: new { a = new { ShopId = id } });


        public async Task<int> Create(Discount discount) =>
            (await template.InsertAsync<Discount>(discount, true))?.ElementAt(0) ?? 0;

        public async Task<bool> Update(Discount item) =>
            await template.UpdateAsync<Discount>(item, whereExpression: new
            {
                Id = item.Id
            }) == 1;

        public async Task<bool> Delete(int id) => await template.DeleteAsync<Discount>(new
        {
            Id = id
        });

    }
}
