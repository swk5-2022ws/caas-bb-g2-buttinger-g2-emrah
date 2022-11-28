using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;

namespace CaaS.Core.Repository
{
    public class OrderRepository : AdoRepository, IOrderRepository
    {
        public OrderRepository(IAdoTemplate template) : base(template) { }

        public async Task<int> Create(Cart cart) =>
               (await template.InsertAsync<Order>(new Order(0, cart.Id, 0, DateTime.Now)))?.ElementAt(0) ?? 0;

        public async Task<Order?> Get(int id) =>
            await template.QueryFirstOrDefaultAsync(OrderMapping.ReadOrderOnly, whereExpression: new
            {
                Id = id
            });

        public async Task<IList<Order>> GetOrdersByCustomerId(int id)
        {
            ICartRepository cartRepository = new CartRepository(template);
            var cart = await cartRepository.GetByCustomer(id);

            if (cart is null) return new List<Order>();

            return await GetOrdersByCartId(cart.Id);
        }

        public async Task<IList<Order>> GetOrdersByShopId(int id)
        {
            ICustomerRepository customerRepository = new CustomerRepository(template);
            var customers = await customerRepository.GetAllByShopId(id);

            if (customers.Count == 0) return new List<Order>();

            var cartIds = customers.Where(customer => customer.CartId.HasValue).Select(customer => customer.CartId!.Value).ToList();
            return (IList<Order>)await template.QueryAsync(OrderMapping.ReadOrderOnly, whereExpression: new
            {
                CartId = cartIds
            });
        }

        public async Task<IList<Order>> GetOrdersByCartId(int id) =>
            (IList<Order>)await template.QueryAsync(OrderMapping.ReadOrderOnly, whereExpression: new
            {
                CartId = id
            });
    }
}
