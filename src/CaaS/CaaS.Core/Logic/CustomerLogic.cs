using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic.Util;
using CaaS.Core.Repository;
using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic
{
    public class CustomerLogic : ICustomerLogic
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IShopRepository shopRepository;

        public CustomerLogic(ICustomerRepository customerRepository, IShopRepository shopRepository)
        {
            this.customerRepository = customerRepository;
            this.shopRepository = shopRepository;
        }

        public async Task<int> Create(Guid appKey, Customer customer)
        {
            if (customer is null) throw ExceptionUtil.ParameterNullException(nameof(customer));
            if (customer.Id != 0) throw ExceptionUtil.ReferenceException(nameof(customer.Id));
            await Check.Shop(shopRepository, customer.ShopId, appKey);
            var customerId = await customerRepository.Create(customer);
            return customerId;
        }

        public async Task<bool> Delete(Guid appKey, int id)
        {
            await Check.Customer(shopRepository, customerRepository, id, appKey);
            return await customerRepository.Delete(id);
        }

        public async Task<Customer> Get(Guid appKey, int id)
        {
            await Check.Customer(shopRepository, customerRepository, id, appKey);
            return await customerRepository.Get(id) ?? throw ExceptionUtil.NoSuchIdException(nameof(id));
        }

        public async Task<IEnumerable<Customer>> GetByShopId(Guid appKey, int shopId)
        {
            await Check.Shop(shopRepository, shopId, appKey);
            return await customerRepository.GetAllByShopId(shopId);
        }

        public async Task<bool> Update(Guid appKey, Customer customer)
        {
            if (customer is null) throw ExceptionUtil.ParameterNullException(nameof(customer));
            await Check.Customer(shopRepository, customerRepository, customer.Id, appKey);
            return await customerRepository.Update(customer);
        }
    }
}
