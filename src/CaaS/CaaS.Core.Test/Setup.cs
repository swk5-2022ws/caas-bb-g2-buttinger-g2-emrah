using Caas.Core.Common;

namespace CaaS.Core.Test
{
    public static class Setup
    {
        private const string CONNECTION_STRING = "server=127.0.0.1;uid=root;pwd=mypass123;database=caas";
        private const string PROVIDER_NAME = "MySql.Data.MySqlClient";

        private static AdoTemplate? _templateEngine;
        public static AdoTemplate GetTemplateEngine()
        {
            if (_templateEngine is null)
            {
                _templateEngine = new AdoTemplate(new ConnectionFactory(CONNECTION_STRING, PROVIDER_NAME));
            }
            return _templateEngine;
        }
    }
}
