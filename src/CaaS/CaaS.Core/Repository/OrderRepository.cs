using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;

namespace CaaS.Core.Repository
{
    public class OrderRepository : AdoRepository, IOrderRepository
    {
        public OrderRepository(AdoTemplate template) : base(template) { }

        public async Task<int> Create(Cart cart) =>
               (await template.InsertAsync<Cart>(cart))?.ElementAt(0) ?? 0;

        public async Task<Order?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(OrderMapping.ReadOrderOnly, whereExpression: new
            {
                Id = id
            });

        public async Task<IList<Order>> GetOrdersByCustomerId(int id)
        {
            ICartRepository cartRepository = new CartRepository(template);
            var cart = cartRepository.GetByCustomer(id);

            if (cart is null) return new List<Order>();

            return await GetOrdersByCartId(cart.Id);
        }

        public async Task<IList<Order>> GetOrdersByShopId(int id)
        {
            ICustomerRepository customerRepository = new CustomerRepository(template);
            var customers = await customerRepository.GetAllByShopId(id);

            return (IList<Order>)template.QueryAsync(OrderMapping.ReadOrderOnly, whereExpression: new
            {
                CartId = customers.Where(customer => customer.CartId.HasValue).Select(customer => customer.CartId!.Value).ToList()
            });
        }

        public async Task<IList<Order>> GetOrdersByCartId(int id) =>
            (IList<Order>)await template.QueryAsync(OrderMapping.ReadOrderOnly, whereExpression: new
            {
                CartId = id
            });
    }
}
