using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    internal class DiscountCartRepositoryStub : IDiscountCartRepository
    {
        IList<DiscountCart> discountCarts;

        public DiscountCartRepositoryStub(IList<DiscountCart> discountCarts)
        {
            this.discountCarts = discountCarts;
        }

        public Task<bool> Create(DiscountCart discountCart)
        {
            if (discountCarts.Any(x => x.CartId == discountCart.CartId && x.DiscountId == discountCart.DiscountId))
                throw new Exception("Element already exists.");
            discountCarts.Add(discountCart);

            return Task.FromResult(true);
        }

        public Task<bool> Delete(DiscountCart discountCart)
        {
            var toRemove = discountCarts.FirstOrDefault(x => x.DiscountId == discountCart.DiscountId
            && x.CartId == discountCart.CartId);

            if (toRemove == null) return Task.FromResult(false);

            discountCarts.Remove(toRemove);
            return Task.FromResult(true);
        }

        public Task<IList<DiscountCart>> GetByCartId(int cartId)
            => Task.FromResult(
                discountCarts
                    .Where(x => x.CartId == cartId)
                    .ToList() as IList<DiscountCart>
                );
    }
}
