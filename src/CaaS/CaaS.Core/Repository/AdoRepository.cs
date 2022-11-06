using Caas.Core.Common.Ado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Repository
{
    public abstract class AdoRepository
    {
        protected readonly AdoTemplate template;

        public AdoRepository(AdoTemplate adoTemplate)
        {
            this.template = adoTemplate;
        }
    }
}
