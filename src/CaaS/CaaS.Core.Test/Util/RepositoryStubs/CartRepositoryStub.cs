﻿using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Transferrecordes;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    public class CartRepositoryStub: ICartRepository
    {
        IDictionary<int, Cart> carts;
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

        public Task<bool> Update(Cart cart)
        {
            if (carts.ContainsKey(cart.Id))
            {
                carts[cart.Id] = cart;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
