using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    internal class CartRepositoryStub : ICartRepository
    {
        private readonly IDictionary<int, Cart> carts;

        public CartRepositoryStub(IDictionary<int, Cart> carts)
        {
            this.carts = carts;
        }

        public Task<int> Create(Cart cart)
        {
            var id = carts.Keys.Max() + 1;
            carts.Add(id, cart);

            return Task.FromResult(id);
        }

        public Task<bool> Delete(int id)
        {
            if (carts.TryGetValue(id, out Cart? cart))
            {
                carts.Remove(id);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<Cart?> Get(int id)
        {
            carts.TryGetValue(id, out Cart? cart);
            return Task.FromResult(cart);
        }

        public Task<Cart?> GetByCustomer(int id) =>
            Task.FromResult(carts.Values.FirstOrDefault(x => x.CustomerId == id));

        public Task<Cart?> GetBySession(string sessionId) =>
            Task.FromResult(carts.Values.FirstOrDefault(x => x.SessionId == sessionId));

        // TODO check cartrepository implementation
        public Task<Cart?> GetByCustomerId(int id)
        {
            IList<Cart> cartsByCustomerId = new List<Cart>();
            foreach (var keyValuePair in carts)
            {
                if (keyValuePair.Value.CustomerId == id)
                    cartsByCustomerId.Add(keyValuePair.Value);
            }

            return Task.FromResult(cartsByCustomerId.FirstOrDefault());
        }

        public Task<bool> Update(Cart cart)
        {
            if (carts.ContainsKey(cart.Id))
            {
                carts[cart.Id] = cart;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IList<Cart>> Get(IList<int> ids) => Task.FromResult((IList<Cart>)carts.Values.Where(x => ids.Contains(x.Id)).ToList());
    }
}
