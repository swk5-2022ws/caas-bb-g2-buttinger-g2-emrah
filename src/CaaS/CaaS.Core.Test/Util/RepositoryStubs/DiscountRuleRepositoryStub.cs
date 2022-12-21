using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    internal class DiscountRuleRepositoryStub : IDiscountRuleRepository
    {
        private readonly GenericStub<DiscountRule> stub;

        public DiscountRuleRepositoryStub(IDictionary<int, DiscountRule> data) 
        {
            stub = new GenericStub<DiscountRule>(data);        
        }

        public Task<int> Create(DiscountRule action)
        {
            return stub.Create(action);
        }

        public Task<bool> Delete(int id)
        {
            return stub.Delete(id);
        }

        public Task<DiscountRule?> Get(int id)
        {
            return stub.Get(id);
        }

        public Task<IList<DiscountRule>> GetByShopId(int id)
        {
            IList<DiscountRule> byShopId = new List<DiscountRule>();
            foreach (var keyValuePair in stub.KeyValuePairs)
            {
                if (keyValuePair.Value!.ShopId == id && keyValuePair.Value!.ShopId == id)
                    byShopId.Add(keyValuePair.Value);
            }
            return Task.FromResult(byShopId);
        }

        public Task<bool> Update(DiscountRule action)
        {
            return stub.Update(action, action.Id);
        }
    }
}
