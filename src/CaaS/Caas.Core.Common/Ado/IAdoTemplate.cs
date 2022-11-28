using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caas.Core.Common.Ado
{
    public interface IAdoTemplate
    {
        Task<IEnumerable<T>> QueryAsync<T>(Func<IDataRecord, T> read, string? joins = null, object? whereExpression = null, bool isSoftDeletionExcluded = true);
        Task<T?> QueryFirstOrDefaultAsync<T>(Func<IDataReader, T> read, string? joins = null, object? whereExpression = null, bool isSoftDeletionExcluded = true);
        Task<bool> DeleteAsync<T>(object? whereExpression = null);
        Task<List<int>?> InsertAsync<T>(object valuesToPass, bool isNewIdNeeded = true);
        Task<int> UpdateAsync<T>(object valuesToUpdate, object? whereExpression = null, bool isSoftDeletionExcluded = true);
    }
}
