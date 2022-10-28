using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Drawing;
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
        public async Task<IEnumerable<T>> QueryAsync<T>(Func<IDataRecord, T> read, string joins = null, object whereExpression = null)
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
        public async Task<T> QueryFirstOrDefaultAsync<T>(Func<IDataRecord, T> read, string joins = null, object whereExpression = null)
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
        public async Task<bool> DeleteAsync<T>(object whereExpression = null)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using (var cmd = connection.CreateCommand())
            {
                var commandText = $"DELETE FROM {typeof(T).Name} ";
                AddWhereExpression(cmd, ref commandText, whereExpression); 
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        /// <summary>
        /// Builds the select query DbCommand
        /// </summary>
        /// <typeparam name="T">The Type of the main search type</typeparam>
        /// <param name="command">The DbCommand</param>
        /// <param name="joins">joins for other tables as string</param>
        /// <param name="whereExpression">the where expressions used to create filters as SQL</param>
        private void BuildQueryCommand<T>(DbCommand command, string joins = null, object whereExpression = null)
        {
            string commandText = $"SELECT * FROM {typeof(T).Name} t ";

            if (!string.IsNullOrEmpty(joins))
            {
                commandText += joins;
            }

            AddWhereExpression(command, ref commandText, whereExpression);
            command.CommandText = commandText;
        }

        /// <summary>
        /// adds the where expression to the given commandText as well as
        /// creates the parameters on the given command.
        /// </summary>
        /// <param name="command">the used command where the parameters are added</param>
        /// <param name="commandText">the command text that needs to be extended</param>
        /// <param name="whereExpression">the object containing the whereExpression structure</param>
        /// <returns></returns>
        private void AddWhereExpression(DbCommand command, ref string commandText, object whereExpression = null)
        {
            if (whereExpression is null)
            {
                return;
            }
            const string WHERE = "WHERE";
            const string AND = "AND";
            var parameterDictionary = ParametersToDictinary(whereExpression);

            foreach (var param in parameterDictionary)
            {
                commandText += $" {(command.Parameters.Count == 0 ? WHERE : AND)} {param.Key} = @{param.Key}";
                AddParam(command, param);
            }
        }

        /// <summary>
        /// Creates a dictionary of parameters from the given whereExpression.
        /// </summary>
        /// <param name="whereExpression">The object for the whereExpression structure</param>
        /// <returns></returns>
        private Dictionary<string, object> ParametersToDictinary(object whereExpression)
        {
            var parameterDictionary = new Dictionary<string, object>();
            foreach (var property in whereExpression.GetType().GetProperties())
            {
                if (property.GetType().IsValueType)
                {
                    SetPropertyToDictionary(parameterDictionary, property, whereExpression, "t");
                    continue;
                }

                var propertyName = GetPropertyName(property);
                foreach (var namedProperty in property.GetType().GetProperties())
                {
                    SetPropertyToDictionary(parameterDictionary, namedProperty, whereExpression, propertyName);
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
        private void SetPropertyToDictionary(Dictionary<string, object> dictionary, PropertyInfo property, object instance, string? baseName = null)
        {
            dictionary[baseName + GetPropertyName(property)] = property.GetValue(instance, null);
        }

        /// <summary>
        /// Retrieves the Property Name either by its name or by an DisplayName set to the property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private string GetPropertyName(PropertyInfo propertyInfo)
        {
            var attribute = (DisplayAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

            if (attribute is null)
            {
                return propertyInfo.GetType().Name;
            }

            return attribute.Name;
        }

        /// <summary>
        /// Adds a parameter to a DbCommand
        /// </summary>
        /// <param name="command">the DbCommand where the parameter is set</param>
        /// <param name="param">KeyValuePair for the parameter with the parameter name and its value</param>
        private void AddParam(DbCommand command, KeyValuePair<string, object> param)
        {
            DbParameter dbParameter = command.CreateParameter();
            dbParameter.ParameterName = $"@{param.Key}";
            dbParameter.Value = param.Value;
            command.Parameters.Add(dbParameter);
        }
    }
}