using Caas.Core.Common.Ado;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository
{
    public class ProductRepository : AdoRepository, IProductRepository
    {
        public ProductRepository(AdoTemplate adoTemplate) : base(adoTemplate)
        {
            
        }

        public Task<int> Create(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Product>> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
