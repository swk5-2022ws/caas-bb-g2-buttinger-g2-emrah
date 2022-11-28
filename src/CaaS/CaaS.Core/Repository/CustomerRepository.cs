using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository.Mappings;

namespace CaaS.Core.Repository
{
    public class CustomerRepository : AdoRepository, ICustomerRepository
    {
        public CustomerRepository(IAdoTemplate template): base(template) { }
        public async Task<int> Create(Customer customer) =>
            (await template.InsertAsync<Customer>(customer))?.ElementAt(0) ?? 0;

        public async Task<bool> Delete(int id)
        {
            Customer? customer = await Get(id);
            if (customer == null)
                return false;

            customer.Deleted = DateTime.Now;
            return await Update(customer);
        }

        public async Task<Customer?> Get(int id) => 
            await template.QueryFirstOrDefaultAsync(CustomerMapping.ReadCustomerOnly, whereExpression: new
            {
                Id = id
            });

        public async Task<IList<Customer>> GetAllByShopId(int shopId) => 
            (IList<Customer>)await template.QueryAsync(CustomerMapping.ReadCustomerOnly, whereExpression: new
            {
                ShopId = shopId
            });

        public async Task<bool> Update(Customer customer) =>
            (await template.UpdateAsync<Customer>(customer, whereExpression: new { Id = customer.Id })) == 1;

    }
}
