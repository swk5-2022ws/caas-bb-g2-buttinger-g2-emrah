using CaaS.Core.Domainmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Util.RepositoryStubs
{
    internal class GenericStub<T>
    {
        private readonly IDictionary<int, T> data;

        public GenericStub(IDictionary<int, T> data)
        {
            this.data = data;
        }

        public KeyValuePair<int, T>[] KeyValuePairs => data.ToArray();

        public Task<T?> Get(int id)
        {
            data.TryGetValue(id, out T? item);

            return Task.FromResult(item);
        }

        public Task<int> Create(T item)
        {
            var id = data.Keys.Max() + 1;
            data.Add(id, item);

            return Task.FromResult(id);
        }

        public Task<bool> Delete(int id)
        {
            if (data.TryGetValue(id, out T? discount))
            {
                data.Remove(id);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> Update(T item, int id) // we could implement a IdAble interface for all entities with an id
        {
            if (data.ContainsKey(id))
            {
                data[id] = item;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
