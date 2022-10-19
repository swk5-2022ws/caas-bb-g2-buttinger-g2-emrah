using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Transferclasses
{
    /// <summary>
    /// Interface for entities which have a soft delete flag.
    /// </summary>
    public interface ISoftDeleteable
    {
        DateTime? Deleted { get; set; }
    }
}
