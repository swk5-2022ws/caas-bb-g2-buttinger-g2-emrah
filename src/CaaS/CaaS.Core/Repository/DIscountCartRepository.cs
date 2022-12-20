using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Discount;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository
{
    public class DiscountCartRepository : AdoRepository, IDiscountCartRepository
    {
        public DiscountCartRepository(IAdoTemplate adoTemplate) : base(adoTemplate)
        {
        }

        public async Task<bool> Create(DiscountCart discountCart)
        => (await template.InsertAsync<DiscountCart>(discountCart, false))?.ElementAt(0) > 0;

        public async Task<bool> Delete(DiscountCart discountCart) =>
            await template.DeleteAsync<DiscountCart>(new
            {
                CartId = discountCart.CartId,
                DiscountId = discountCart.DiscountId
            });

        public async Task<IList<DiscountCart>> GetByCartId(int cartId) =>
            (IList<DiscountCart>)await template.QueryAsync(CartDiscountMapping.Read, whereExpression: new
            {
                CartId = cartId
            });
    }
}
