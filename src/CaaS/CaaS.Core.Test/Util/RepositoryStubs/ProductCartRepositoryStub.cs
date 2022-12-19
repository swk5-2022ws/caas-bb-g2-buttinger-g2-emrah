using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    public class ProductCartRepositoryStub: IProductCartRepository
    {
        private readonly IDictionary<(int, int), ProductCart> productCarts;

        public ProductCartRepositoryStub(IDictionary<(int, int), ProductCart> productCarts)
        {
            this.productCarts = productCarts;
        }

        public Task<int> Create(ProductCart productCart)
        {
            var added = productCarts.TryAdd((productCart.ProductId, productCart.CartId), productCart);           
            return Task.FromResult(added ? 1 : 0);
        }

        public Task<bool> Delete(int productId, int cartId)
        {
            if (productCarts.TryGetValue((productId, cartId), out ProductCart? productCart))
            {
                productCarts.Remove((productId, cartId));
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<ProductCart?> Get(int productId, int cartId)
        {
            productCarts.TryGetValue((productId, cartId), out ProductCart? püroductCart);
            return Task.FromResult(püroductCart);
        }

        public Task<IList<ProductCart>> GetByCartId(int id) =>
            Task.FromResult((IList<ProductCart>)productCarts.Where(x => x.Key.Item2 == id).Select(y => y.Value).ToList());

        public Task<bool> Update(int productId, int cartId, uint amount)
        {
            if (productCarts.ContainsKey((productId, cartId)))
            {
                productCarts[(productId, cartId)].Amount = amount;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
