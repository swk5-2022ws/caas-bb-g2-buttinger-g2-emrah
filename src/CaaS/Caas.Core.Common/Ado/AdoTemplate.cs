using CaaS.Util;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Caas.Core.Common.Ado
{
    public class AdoTemplate
    {
        private readonly IConnectionFactory connectionFactory;

        /// <summary>
        /// Constructor for AdoTemplate 
        /// </summary>
        /// <param name="connectionFactory">Dependency Injected connection factory</param>
        public AdoTemplate(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Selects Elements from the database into a list of the passed object
        /// </summary>
        /// <typeparam name="T">Generic object used to select the elements</typeparam>
        /// <param name="read">The conversion function for the reader to the dedicated generic element</param>
        /// <param name="joins">Additional joins as string text</param>
        /// <param name="whereExpression">object used to create the where expressions</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(Func<IDataRecord, T> read, string? joins = null, object? whereExpression = null, bool isSoftDeletionExcluded = true)
        {
            var list = new List<T>();
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using (var cmd = connection.CreateCommand())
            {
                AdoBuilder.BuildQueryCommand<T>(cmd, joins, whereExpression, isSoftDeletionExcluded);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(read(reader));
                }
            }
            return list;
        }

        /// <summary>
        /// Retrieves an element from the database
        /// </summary>
        /// <typeparam name="T">The generic type of the object</typeparam>
        /// <param name="read">The conversion of the ADO.NET object to the generic Type</param>
        /// <param name="joins">The Joins needed for that object</param>
        /// <param name="whereExpression">An object that creates where expressions</param>
        /// <returns></returns>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(Func<IDataReader, T> read, string? joins = null, object? whereExpression = null, bool isSoftDeletionExcluded = true)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using (var cmd = connection.CreateCommand())
            {
                AdoBuilder.BuildQueryCommand<T>(cmd, joins, whereExpression, isSoftDeletionExcluded);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    return read(reader);
                }
            }
            return default;
        }

        /// <summary>
        /// Deletes objects by the specified type and a given filter from the Database.
        /// </summary>
        /// <typeparam name="T">The specified type</typeparam>
        /// <param name="whereExpression">The given filter</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<T>(object? whereExpression = null)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using var cmd = connection.CreateCommand();
            var commandText = $"DELETE FROM {typeof(T).Name} ";
            AdoBuilder.AddWhereExpression(cmd, commandText, whereExpression, addTableNameSpace: false);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        /// <summary>
        /// Allows the insert of objects to the Table specified by the generic type T
        /// </summary>
        /// <typeparam name="T">The table type</typeparam>
        /// <param name="valuesToPass">The values that should be inserted. Pass a list to insert multiple elements</param>
        /// <returns>The new id or 0 if the insert failed</returns>
        public async Task<List<int>?> InsertAsync<T>(object valuesToPass, bool isNewIdNeeded = true)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using var cmd = connection.CreateCommand();
            var tableName = typeof(T).Name;

            if (valuesToPass is IEnumerable enumerable)
            {
                var results = new List<int>();
                var alreadyIncludedParameterNames = new List<string>();
                foreach (var value in enumerable)
                {
                    alreadyIncludedParameterNames.AddRange(AdoBuilder.BuildInsertCommand(cmd, tableName, value, alreadyIncludedParameterNames));

                    if (isNewIdNeeded)
                    {
                        var res = await cmd.ExecuteScalarAsync();
                        results.Add(res == null ? 0 : (int)res);
                    }
                    else
                    {
                        var res = await cmd.ExecuteNonQueryAsync();
                        results.Add(res);
                    }
                }

                return results;
            }
            else
            {
                AdoBuilder.BuildInsertCommand(cmd, tableName, valuesToPass);
                if (isNewIdNeeded)
                {
                    var result = await cmd.ExecuteScalarAsync();
                    return result == null ? null : new List<int>() { (int)result };
                }
                else
                {
                    var result = await cmd.ExecuteNonQueryAsync();
                    return result == 0 ? null : new List<int>() { result };
                }
            }
        }

        /// <summary>
        /// Allows the update of objects to the table specified by the generic object T
        /// </summary>
        /// <typeparam name="T">Table to update</typeparam>
        /// <param name="valuesToUpdate">values that should be set</param>
        /// <param name="whereExpression">where expression that should be used on the table update</param>
        /// <param name="isSoftDeletionExcluded">excludes the soft deleted rows if soft deletion is possible and this flag is set.</param>
        /// <returns>The amount of rows updated by the command.</returns>
        public async Task<int> UpdateAsync<T>(object valuesToUpdate, object? whereExpression = null, bool isSoftDeletionExcluded = true)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using var cmd = connection.CreateCommand();
            AdoBuilder.BuildUpdateCommand<T>(cmd, valuesToUpdate, whereExpression, isSoftDeletionExcluded);
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}