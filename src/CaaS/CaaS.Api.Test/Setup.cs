using Caas.Core.Common;
using Caas.Core.Common.Ado;
using Microsoft.Extensions.Configuration;

namespace CaaS.Core.Test
{
    public static class Setup
    {
        private const string CONNECTION_STRING = "server=db;uid=root;pwd=mypass123;database=caas";
        private const string PROVIDER_NAME = "MySql.Data.MySqlClient";

        private static AdoTemplate? _templateEngine;
        public static AdoTemplate GetTemplateEngine()
        {
            if (_templateEngine != null) return _templateEngine;

            string environment = Environment.GetEnvironmentVariable("environment")
                ?? "development";

            var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: false)
                .AddJsonFile($"appSettings.{environment}.json", optional: true)
                .Build();
            var connectionString = config.GetSection("ConnectionStrings")?["caas-db"] ?? CONNECTION_STRING;
            Console.Error.WriteLine($"Using connection string {connectionString}");

            _templateEngine = new AdoTemplate(new ConnectionFactory(connectionString, PROVIDER_NAME));
            return _templateEngine;
        }
    }
}
