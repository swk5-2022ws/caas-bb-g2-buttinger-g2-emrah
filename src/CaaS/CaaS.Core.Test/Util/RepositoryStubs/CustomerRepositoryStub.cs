using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    public class CustomerRepositoryStub: ICustomerRepository
    {
        IDictionary<int, Customer> customers;
        public CustomerRepositoryStub(IDictionary<int, Customer> customers)
        {
            this.customers = customers;
        }

        public Task<int> Create(Customer customer)
        {
            var id = customers.Keys.Max() + 1;
            customers.Add(id, customer);
            return Task.FromResult(id);
        }

        public Task<bool> Delete(int id)
        {
            if (customers.TryGetValue(id, out Customer? customer))
            {
                customers.Remove(id);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<Customer?> Get(int id)
        {
            customers.TryGetValue(id, out Customer? customer);
            return Task.FromResult(customer);
        }

        public Task<IList<Customer>> GetAllByShopId(int shopId) =>
            Task.FromResult((IList<Customer>)customers.Values.Where(x => x.ShopId == shopId).ToList());

        public Task<bool> Update(Customer customer)
        {
            if (customers.ContainsKey(customer.Id))
            {
                customers[customer.Id] = customer;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
