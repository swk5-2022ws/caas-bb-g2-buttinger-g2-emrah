﻿using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace Caas.Core.Common
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly DbProviderFactory dbProviderFactory;

        public static IConnectionFactory FromConfiguration(IConfiguration config, string connectionStringConfigName)
        {
            var connectionConfig = config.GetSection("ConnectionStrings").GetSection(connectionStringConfigName);
            string connectionString = connectionConfig["ConnectionString"];
            string providerName = connectionConfig["ProviderName"];

            return new ConnectionFactory(connectionString, providerName);
        }

        public ConnectionFactory(string connectionString, string providerName)
        {
            this.ConnectionString = connectionString;

            this.ProviderName = providerName;

            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);

            this.dbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public string ConnectionString { get; }

        public string ProviderName { get; }

        public async Task<DbConnection> CreateConnectionAsync()
        {
            var connection = dbProviderFactory.CreateConnection() ?? throw new InvalidOperationException("DbConnection could not be established!");
            connection.ConnectionString = this.ConnectionString;
            await connection.OpenAsync();
            return connection;
        }
    }
}
