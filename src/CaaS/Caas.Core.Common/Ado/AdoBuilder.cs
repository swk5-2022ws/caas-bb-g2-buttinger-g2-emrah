﻿using CaaS.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caas.Core.Common.Ado
{
    public static class AdoBuilder
    {
        internal static string GetAvailableParameterName(string parameterName, IList<string>? alreadyIncludedNames = null)
        {
            if (alreadyIncludedNames == null || alreadyIncludedNames.Count == 0 || !alreadyIncludedNames.Any(name => name == parameterName))
            {
                return parameterName;
            }

            return $"{parameterName}{alreadyIncludedNames.Count(name => name.StartsWith(parameterName))}";
        }

        /// <summary>
        /// Creates a dictionary of parameters from the given whereExpression.
        /// </summary>
        /// <param name="properties">The object for the properties structure</param>
        /// <param name="addTableNameSpace">Determines wether or not a namespace for the initial table should be set. Defaults to true</param>
        /// <returns></returns>
        internal static Dictionary<string, object?> ParametersToDictinary(object properties, bool addTableNameSpace = true)
        {
            var parameterDictionary = new Dictionary<string, object?>();
            foreach (var property in properties.GetType().GetProperties())
            {
                if (ReflectionUtil.IsSystemType(property))
                {
                    SetPropertyToDictionary(parameterDictionary, property, properties, addTableNameSpace ? "t." : "");
                    continue;
                }


                object? joinedProperties = property.GetValue(properties, null);
                var joinedTableName = ReflectionUtil.GetPropertyName(property);

                if (joinedProperties == null)
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
        internal static void SetPropertyToDictionary(Dictionary<string, object?> dictionary, PropertyInfo property, object instance, string? baseName = null) =>
            dictionary[baseName + ReflectionUtil.GetPropertyName(property)] = property.GetValue(instance, null);

        /// <summary>
        /// Adds a parameter to a DbCommand
        /// </summary>
        /// <param name="command">the DbCommand where the parameter is set</param>
        /// <param name="parameterName">the name of the parameter that should be stored in the dbCommand</param>
        /// <param name="value">the value of the parameter that should be stored in the dbCommand</param>
        internal static void AddParam(DbCommand command, object? value, string parameterName)
        {
            DbParameter dbParameter = command.CreateParameter();
            dbParameter.ParameterName = $"@{parameterName}";
            dbParameter.Value = value;
            command.Parameters.Add(dbParameter);
        }

        internal static void BuildUpdateCommand(DbCommand command, string tableName, object valuesToUpdate, object? whereExpression = null)
        {
            var commandText = $"UPDATE {tableName} SET ";
            var parameters = AdoBuilder.ParametersToDictinary(valuesToUpdate, false);
            foreach (var param in parameters)
            {
                commandText += $"{param.Key} = @{param.Key},";
                AdoBuilder.AddParam(command, param.Value, param.Key);
            }
            commandText = commandText.Remove(commandText.Length - 1);

            AddWhereExpression(command, commandText, whereExpression, parameters.Select(param => param.Key).ToList(), false);

        }

        /// <summary>
        /// Creates the correct insert command using a prepared statement.
        /// </summary>
        /// <param name="command">The command that is executed</param>
        /// <param name="tableName">The table name the insert is called on.</param>
        /// <param name="valuesToPass">The values object passed. This must map to a single line of an insert statement.</param>
        internal static void BuildInsertCommand(DbCommand command, string tableName, object valuesToPass)
        {
            var commandText = $"INSERT INTO {tableName} (";
            var parameters = AdoBuilder.ParametersToDictinary(valuesToPass, false);
            var valuesText = $"VALUES(";

            var keys = parameters.Keys;
            var parameterCount = parameters.Count;
            for (int i = 0; i < parameterCount; i++)
            {
                var key = keys.ElementAt(i);
                commandText += key;
                if (i + 1 != parameterCount)
                    commandText += ",";
                valuesText += $"@{key}";
                if (i + 1 != parameterCount)
                    valuesText += ",";
                AdoBuilder.AddParam(command, parameters[key], key);

            }
            commandText += $") {valuesText});";
            // fetch the max id if available, so we can return the newly inserted id
            if (keys.Contains("Id"))
                commandText += $"SELECT MAX(Id) FROM {tableName};";
            command.CommandText += commandText;
        }

        /// <summary>
        /// Builds the select query DbCommand
        /// </summary>
        /// <typeparam name="T">The Type of the main search type</typeparam>
        /// <param name="command">The DbCommand</param>
        /// <param name="joins">joins for other tables as string</param>
        /// <param name="whereExpression">the where expressions used to create filters as SQL</param>
        internal static void BuildQueryCommand<T>(DbCommand command, string? joins = null, object? whereExpression = null)
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
        /// <param name="addTableNameSpace">Determines whether or not a namespace for the initial table should be set. Defaults to true</param>
        /// <returns></returns>
        internal static void AddWhereExpression(DbCommand command, string commandText, object? whereExpression = null, IList<string>? alreadyIncludedSqlParameterNames = null, bool addTableNameSpace = true)
        {
            if (whereExpression is null)
            {
                return;
            }
            const string WHERE = "WHERE";
            const string AND = "AND";
            var parameterDictionary = AdoBuilder.ParametersToDictinary(whereExpression, addTableNameSpace);

            bool useWhere = true;
            foreach (var param in parameterDictionary)
            {
                var sqlParameterName = AdoBuilder.GetAvailableParameterName(param.Key, alreadyIncludedSqlParameterNames);
                commandText += $" {(useWhere ? WHERE : AND)} {param.Key} = @{sqlParameterName}";
                useWhere = false;
                AdoBuilder.AddParam(command, param.Value, sqlParameterName);
            }
            command.CommandText = commandText;
        }
    }
}