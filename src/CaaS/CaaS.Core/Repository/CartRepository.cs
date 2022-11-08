using Caas.Core.Common.Ado;
using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository
{
    public class CartRepository : AdoRepository, ICartRepository
    {
        public CartRepository(AdoTemplate adoTemplate) : base(adoTemplate)
        {
        }

        public async Task<bool> Delete(int id) =>
            await template.DeleteAsync<Cart>(new
            {
                Id = id
            });

        public async Task<Cart?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(record =>
                new Cart(record.GetIntByName(nameof(Cart.Id)), record.GetStringByName(nameof(Cart.SessionId)))
                {
                    CustomerId = record.GetNullableIntByName(nameof(Cart.CustomerId)) 
                }, whereExpression: new
                {
                    Id = id
                });
    }
}
