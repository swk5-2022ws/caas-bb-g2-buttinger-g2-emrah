using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    internal class OrderRepositoryStub : IOrderRepository
    {
        private readonly IDictionary<int, Order> orders;

        public OrderRepositoryStub(IDictionary<int, Order> orders)
        {
            this.orders = orders;
        }

        public Task<int> Create(Cart cart, double discount)
        {
            var id = orders.Keys.Max() + 1;
            orders.Add(id, new Order(id, cart.Id, discount, DateTime.Now));

            return Task.FromResult(id);
        }

        public Task<Order?> Get(int id)
        {
            orders.TryGetValue(id, out Order? item);
            return Task.FromResult(item);
        }

        public Task<IList<Order>> GetOrdersByCartId(int id) => Task.FromResult((IList<Order>)orders.Values.Where(x => x.CartId == id).ToList());

        public Task<IList<Order>> GetOrdersByCustomerId(int id) => Task.FromResult((IList<Order>) orders.Values.Where(x => x.Cart != null && x.Cart.CustomerId == id).ToList());

        public Task<IList<Order>> GetOrdersByShopId(int id) => Task.FromResult((IList<Order>)orders.Values.Where(x => x.Cart != null && x.Cart.Customer != null && 
                                                                                                                      x.Cart.Customer.ShopId == id).ToList());
    }
}
