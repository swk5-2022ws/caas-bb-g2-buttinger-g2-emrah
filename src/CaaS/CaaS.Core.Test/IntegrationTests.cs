using Caas.Core.Common;
using CaaS.Core.Domainmodels;
using NUnit.Framework;

namespace CaaS.Core.Test
{
    public class IntegrationTests
    {
        private const string CONNECTION_STRING = "server=127.0.0.1;uid=root;pwd=mypass123;database=caas";
        private const string PROVIDER_NAME = "MySql.Data.MySqlClient";
        private IConnectionFactory ConnectionFactory { get; init; }
        private AdoTemplate TemplatingEngine { get; init; }

        public IntegrationTests()
        {
            ConnectionFactory = new ConnectionFactory(CONNECTION_STRING, PROVIDER_NAME);
            TemplatingEngine = new AdoTemplate(ConnectionFactory);
        }

        [Test]
        public async Task SelectSingleCartByIdReturnsCart()
        {
            var cart = await TemplatingEngine.QueryFirstOrDefaultAsync(reader => new Cart((int)reader["Id"], (string)reader["SessionId"])
            {
                CustomerId = (int)reader["CustomerId"]
            }, whereExpression: new {
                Id = 1
            });

            Assert.IsNotNull(cart);
            Assert.That(cart.Id, Is.EqualTo(1));
            Assert.That(cart.SessionId, Is.EqualTo("7ee2dcbd-8e42-366d-9919-b96d65afd844"));
            Assert.IsNotNull(cart.CustomerId);
            Assert.That(cart.CustomerId.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task SelectSingleCartByIdWithCustomerReturnsCart()
        {
            var cart = await TemplatingEngine.QueryFirstOrDefaultAsync(reader => {
                var c =  new Cart((int)reader[0], (string)reader["SessionId"])
                {
                    CustomerId = (int)reader["CustomerId"],
                    Customer = new Customer((int)reader[3], (int)reader["ShopId"], (string)reader["Name"], (string)reader["Email"])
                };
                return c;
            },
            joins: "JOIN Customer c ON c.Id = t.Id",
            whereExpression: new
            {
                Id = 1
            });

            Assert.IsNotNull(cart);
            Assert.That(cart.Id, Is.EqualTo(1));
            Assert.That(cart.SessionId, Is.EqualTo("7ee2dcbd-8e42-366d-9919-b96d65afd844"));
            Assert.IsNotNull(cart.CustomerId);
            Assert.That(cart.CustomerId.Value, Is.EqualTo(1));
            Assert.IsNotNull(cart.Customer);
            Assert.That(cart.Customer.Id, Is.EqualTo(1));
            Assert.That(cart.Customer.Name, Is.EqualTo("Alexa Adams II"));
            Assert.That(cart.Customer.Email, Is.EqualTo("hilll.mireya@example.com"));
        }

        [Test]
        public async Task SelectSingleCartByCustomerNameWithCustomerReturnsCart()
        {
            var cart = await TemplatingEngine.QueryFirstOrDefaultAsync(reader => {
                var c = new Cart((int)reader[0], (string)reader["SessionId"])
                {
                    CustomerId = (int)reader["CustomerId"],
                    Customer = new Customer((int)reader[3], (int)reader["ShopId"], (string)reader["Name"], (string)reader["Email"])
                };
                return c;
            },
            joins: "JOIN Customer c ON c.Id = t.Id",
            whereExpression: new
            {
                c = new
                {
                    Name = "Alexa Adams II"
                }
            });

            Assert.IsNotNull(cart);
            Assert.That(cart.Id, Is.EqualTo(1));
            Assert.That(cart.SessionId, Is.EqualTo("7ee2dcbd-8e42-366d-9919-b96d65afd844"));
            Assert.IsNotNull(cart.CustomerId);
            Assert.That(cart.CustomerId.Value, Is.EqualTo(1));
            Assert.IsNotNull(cart.Customer);
            Assert.That(cart.Customer.Id, Is.EqualTo(1));
            Assert.That(cart.Customer.Name, Is.EqualTo("Alexa Adams II"));
            Assert.That(cart.Customer.Email, Is.EqualTo("hilll.mireya@example.com"));
        }

    }
}
