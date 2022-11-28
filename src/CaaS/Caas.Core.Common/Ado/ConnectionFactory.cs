using System.Configuration.Provider;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace Caas.Core.Common.Ado
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly DbProviderFactory dbProviderFactory;

        public ConnectionFactory(IConfiguration config)
        {
            var connectionConfig = config.GetSection("ConnectionStrings");
            string connectionString = connectionConfig["caas-db"];
            string providerName = connectionConfig["ProviderName"];

            ConnectionString = connectionString;
            ProviderName = providerName;
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
            dbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public ConnectionFactory(string connectionString, string providerName)
        {
            ConnectionString = connectionString;
            ProviderName = providerName;
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
            dbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public string ConnectionString { get; init; }

        public string ProviderName { get; init; }

        public async Task<DbConnection> CreateConnectionAsync()
        {
            var connection = dbProviderFactory.CreateConnection() ?? throw new InvalidOperationException("DbConnection could not be established!");
            connection.ConnectionString = ConnectionString;
            await connection.OpenAsync();
            return connection;
        }
    }
}
