using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Caas.Core.Common
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
                BuildQueryCommand<T>(cmd, joins, whereExpression);
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
                BuildQueryCommand<T>(cmd, joins, whereExpression);
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
            await using (var cmd = connection.CreateCommand())
            {
                var commandText = $"DELETE FROM {typeof(T).Name} ";
                AddWhereExpression(cmd, commandText, whereExpression);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        /// <summary>
        /// Allows the insert of objects to the Table specified by the generic type T
        /// </summary>
        /// <typeparam name="T">The table type</typeparam>
        /// <param name="valuesToPass">The values that should be inserted. Pass a list to insert multiple elements</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync<T>(object valuesToPass)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using (var cmd = connection.CreateCommand())
            {
                var tableName = typeof(T).Name;
                if (valuesToPass is IEnumerable)
                {
                    foreach (var value in (IEnumerable)valuesToPass)
                    {
                        BuildInsertCommand(cmd, tableName, value);
                    }
                }
                else
                {
                    BuildInsertCommand(cmd, tableName, valuesToPass);
                }
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateAsync<T>(object valuesToUpdate, object? whereExpression = null)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using (var cmd = connection.CreateCommand())
            {
                BuildUpdateCommand(cmd, typeof(T).Name, valuesToUpdate, whereExpression);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        private void BuildUpdateCommand(DbCommand command, string tableName, object valuesToUpdate, object? whereExpression = null)
        {
            var commandText = $"UPDATE {tableName} SET ";
            var parameters = ParametersToDictinary(valuesToUpdate, false);
            foreach (var param in parameters)
            {
                commandText += $"{param.Key} = @{param.Key}";
                AddParam(command, param.Value, param.Key);
            }

            AddWhereExpression(command, commandText, whereExpression, parameters.Select(param => param.Key).ToList(), false);

        }

        /// <summary>
        /// Creates the correct insert command using a prepared statement.
        /// </summary>
        /// <param name="command">The command that is executed</param>
        /// <param name="tableName">The table name the insert is called on.</param>
        /// <param name="valuesToPass">The values object passed. This must map to a single line of an insert statement.</param>
        private void BuildInsertCommand(DbCommand command, string tableName, object valuesToPass)
        {
            var commandText = $"INSERT INTO {tableName} (";
            var parameters = ParametersToDictinary(valuesToPass, false);
            var valuesText = $"VALUES(";
            foreach (var param in parameters)
            {
                commandText += param.Key;
                valuesText += $"@{param.Key}";
                AddParam(command, param.Value, param.Key);
            }
            commandText += $") {valuesText});";
            command.CommandText += commandText;
        }

        /// <summary>
        /// Builds the select query DbCommand
        /// </summary>
        /// <typeparam name="T">The Type of the main search type</typeparam>
        /// <param name="command">The DbCommand</param>
        /// <param name="joins">joins for other tables as string</param>
        /// <param name="whereExpression">the where expressions used to create filters as SQL</param>
        private void BuildQueryCommand<T>(DbCommand command, string? joins = null, object? whereExpression = null)
        {
            string commandText = $"SELECT * FROM {typeof(T).Name} t ";

            if (!string.IsNullOrEmpty(joins))
            {
                commandText += joins;
            }

            AddWhereExpression(command, commandText, whereExpression);
        }

        /// <summary>
        /// adds the where expression to the given commandText as well as
        /// creates the parameters on the given command.
        /// </summary>
        /// <param name="command">the used command where the parameters are added</param>
        /// <param name="commandText">the command text that needs to be extended</param>
        /// <param name="whereExpression">the object containing the whereExpression structure</param>
        /// <param name="alreadyIncludedSqlParameterNames">The sql parameter names that were already included during a process that happened before the where expression was added</param>
        /// <param name="addTableNameSpace">Determines wether or not a namespace for the initial table should be set. Defaults to true</param>
        /// <returns></returns>
        private void AddWhereExpression(DbCommand command, string commandText, object? whereExpression = null, IList<string>? alreadyIncludedSqlParameterNames = null, bool addTableNameSpace = true)
        {
            if (whereExpression is null)
            {
                return;
            }
            const string WHERE = "WHERE";
            const string AND = "AND";
            var parameterDictionary = ParametersToDictinary(whereExpression, addTableNameSpace);

            foreach (var param in parameterDictionary)
            {
                var sqlParameterName = GetAvailableParameterName(param.Key, alreadyIncludedSqlParameterNames);
                commandText += $" {(command.Parameters.Count == 0 ? WHERE : AND)} {param.Key} = @{sqlParameterName}";
                AddParam(command, param.Value, sqlParameterName);
            }
            command.CommandText = commandText;
        }

        private string GetAvailableParameterName(string parameterName, IList<string>? alreadyIncludedNames = null)
        {
            if(alreadyIncludedNames == null || alreadyIncludedNames.Count == 0 || !alreadyIncludedNames.Any(name => name == parameterName))
            {
                return parameterName;
            }

            return $"{parameterName} {alreadyIncludedNames.Count(name => name.StartsWith(parameterName))}";

        }

        /// <summary>
        /// Creates a dictionary of parameters from the given whereExpression.
        /// </summary>
        /// <param name="properties">The object for the properties structure</param>
        /// <param name="addTableNameSpace">Determines wether or not a namespace for the initial table should be set. Defaults to true</param>
        /// <returns></returns>
        private Dictionary<string, object?> ParametersToDictinary(object properties, bool addTableNameSpace = true)
        {
            var parameterDictionary = new Dictionary<string, object?>();
            foreach (var property in properties.GetType().GetProperties())
            {
                if (property.PropertyType.IsPrimitive)
                {
                    SetPropertyToDictionary(parameterDictionary, property, properties, addTableNameSpace ? "t." : "");
                    continue;
                }


                object? joinedProperties = property.GetValue(properties, null);
                var joinedTableName = GetPropertyName(property);

                if(joinedProperties == null)
                {
                    continue;
                }

                foreach (var namedProperty in joinedProperties.GetType().GetProperties())
                {
                    SetPropertyToDictionary(parameterDictionary, namedProperty, joinedProperties, $"{joinedTableName}.");
                }
            }
            return parameterDictionary;
        }

        /// <summary>
        /// Creates a new KeyValuePair in the given dictionary
        /// </summary>
        /// <param name="dictionary">the dictionary</param>
        /// <param name="property">the property used to create</param>
        /// <param name="instance">the instance where the value of the property is stored</param>
        /// <param name="baseName">additional Name added to the property</param>
        private void SetPropertyToDictionary(Dictionary<string, object?> dictionary, PropertyInfo property, object instance, string? baseName = null) => 
            dictionary[baseName + GetPropertyName(property)] = property.GetValue(instance, null);

        /// <summary>
        /// Retrieves the Property Name either by its name or by an DisplayName set to the property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private string? GetPropertyName(PropertyInfo propertyInfo)
        {
            var attribute = (DisplayAttribute?)propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

            if (attribute is null)
            {
                return propertyInfo.Name;
            }

            return attribute.Name;
        }

        /// <summary>
        /// Adds a parameter to a DbCommand
        /// </summary>
        /// <param name="command">the DbCommand where the parameter is set</param>
        /// <param name="parameterName">the name of the parameter that should be stored in the dbCommand</param>
        /// <param name="value">the value of the parameter that should be stored in the dbCommand</param>
        private void AddParam(DbCommand command, object? value, string parameterName)
        {
            DbParameter dbParameter = command.CreateParameter();
            dbParameter.ParameterName = $"@{parameterName}";
            dbParameter.Value = value;
            command.Parameters.Add(dbParameter);
        }
    }
}