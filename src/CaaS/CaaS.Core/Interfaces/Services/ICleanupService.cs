using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Services
{
    public interface ICleanupService
    {
        Task RemoveNotUsedCartsAsync();
    }
}
