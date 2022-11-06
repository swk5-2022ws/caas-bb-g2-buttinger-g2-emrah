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
        public async Task<IEnumerable<T>> QueryAsync<T>(Func<IDataRecord, T> read, string? joins = null, object? whereExpression = null)
        {
            var list = new List<T>();
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using (var cmd = connection.CreateCommand())
            {
                AdoBuilder.BuildQueryCommand<T>(cmd, joins, whereExpression);
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
        public async Task<T?> QueryFirstOrDefaultAsync<T>(Func<IDataReader, T> read, string? joins = null, object? whereExpression = null)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using (var cmd = connection.CreateCommand())
            {
                AdoBuilder.BuildQueryCommand<T>(cmd, joins, whereExpression);
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
            AdoBuilder.AddWhereExpression(cmd, commandText, whereExpression);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        /// <summary>
        /// Allows the insert of objects to the Table specified by the generic type T
        /// </summary>
        /// <typeparam name="T">The table type</typeparam>
        /// <param name="valuesToPass">The values that should be inserted. Pass a list to insert multiple elements</param>
        /// <returns>The new id or 0 if the insert failed</returns>
        public async Task<int> InsertAsync<T>(object valuesToPass)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using var cmd = connection.CreateCommand();
            var tableName = typeof(T).Name;
            if (valuesToPass is IEnumerable enumerable)
            {
                foreach (var value in enumerable)
                {
                    AdoBuilder.BuildInsertCommand(cmd, tableName, value);
                }
            }
            else
            {
                AdoBuilder.BuildInsertCommand(cmd, tableName, valuesToPass);
            }

            var result = await cmd.ExecuteScalarAsync();
            return result == null ? 0 : (int)result;
        }

        public async Task<int> UpdateAsync<T>(object valuesToUpdate, object? whereExpression = null)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using var cmd = connection.CreateCommand();
            AdoBuilder.BuildUpdateCommand(cmd, typeof(T).Name, valuesToUpdate, whereExpression);
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}