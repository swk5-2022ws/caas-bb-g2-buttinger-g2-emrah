using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    internal class DiscountActionRepositoryStub : IDiscountActionRepository
    {
        private GenericStub<DiscountAction> _stub;

        public DiscountActionRepositoryStub(IDictionary<int, DiscountAction> data)
        {
            _stub = new GenericStub<DiscountAction>(data);
        }

        public Task<int> Create(DiscountAction action)
        {
            return _stub.Create(action);
        }

        public Task<bool> Delete(int id)
        {
            return _stub.Delete(id);
        }

        public Task<DiscountAction?> Get(int id)
        {
            return _stub.Get(id);
        }

        public Task<IList<DiscountAction>> GetByShopId(int id)
        {
            IList<DiscountAction> byShopId = new List<DiscountAction>();
            foreach (var keyValuePair in _stub.KeyValuePairs)
            {
                if (keyValuePair.Value!.ShopId == id && keyValuePair.Value!.ShopId == id)
                    byShopId.Add(keyValuePair.Value);
            }
            return Task.FromResult(byShopId);
        }

        public Task<bool> Update(DiscountAction action)
        {
            return _stub.Update(action, action.Id);
        }
    }
}
