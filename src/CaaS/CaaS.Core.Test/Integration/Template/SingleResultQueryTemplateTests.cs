using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using System.Data;

namespace CaaS.Core.Test.Integration.Template
{
    [Category("Integration")]
    [TestFixture]
    public class SingleResultQueryTemplateTests
    {
        [Test]
        [TestCase(1, "7ee2dcbd-8e42-366d-9919-b96d65afd844", 1)]
        [TestCase(2, "747c7000-c0b2-330a-930a-1d14e39b1e64", 2)]
        [TestCase(3, "5c662d68-ead5-35fe-af4c-cf4470a8ff3d", 3)]
        public async Task SelectSingleCartByIdReturnsCart(int cartId, string sessionId, int customerId)
        {
            var cart = await Setup.GetTemplateEngine().QueryFirstOrDefaultAsync(reader => new Cart((int)reader["Id"], (string)reader["SessionId"])
            {
                CustomerId = (int)reader["CustomerId"]
            }, whereExpression: new
            {
                Id = cartId
            });

            BasicSingleCartAssertions(cart, cartId, sessionId, customerId);
        }

        [Test]
        [TestCase(1, "7ee2dcbd-8e42-366d-9919-b96d65afd844", 1, 1, "Alexa Adams II", "hilll.mireya@example.com")]
        [TestCase(2, "747c7000-c0b2-330a-930a-1d14e39b1e64", 2, 2, "Mr. Narciso Klein", "mweber@example.net")]
        [TestCase(3, "5c662d68-ead5-35fe-af4c-cf4470a8ff3d", 3, 3, "Colby Wiza", "jgreenfelder@example.net")]
        public async Task SelectSingleCartByIdWithCustomerReturnsCart(int cartId, string sessionId, int customerId, int shopId, string name, string email)
        {
            var cart = await Setup.GetTemplateEngine().QueryFirstOrDefaultAsync(ReadToCartWithCustomer,
            joins: "JOIN Customer c ON c.Id = t.Id",
            whereExpression: new
            {
                Id = cartId
            });

            SingleCartWithCustomerAssertion(cart, cartId, sessionId, customerId, shopId, name, email);
        }

        [Test]
        [TestCase(1, "7ee2dcbd-8e42-366d-9919-b96d65afd844", 1, 1, "Alexa Adams II", "hilll.mireya@example.com")]
        [TestCase(2, "747c7000-c0b2-330a-930a-1d14e39b1e64", 2, 2, "Mr. Narciso Klein", "mweber@example.net")]
        [TestCase(3, "5c662d68-ead5-35fe-af4c-cf4470a8ff3d", 3, 3, "Colby Wiza", "jgreenfelder@example.net")]
        public async Task SelectSingleCartByCustomerNameWithCustomerReturnsCart(int cartId, string sessionId, int customerId, int shopId, string name, string email)
        {
            var cart = await Setup.GetTemplateEngine().QueryFirstOrDefaultAsync(ReadToCartWithCustomer,
            joins: "JOIN Customer c ON c.Id = t.Id",
            whereExpression: new
            {
                c = new
                {
                    Name = name
                }
            });

            SingleCartWithCustomerAssertion(cart, cartId, sessionId, customerId, shopId, name, email);
        }

        private Cart ReadToCartWithCustomer(IDataRecord reader) =>
            new Cart((int)reader[0], (string)reader["SessionId"])
            {
                CustomerId = (int)reader["CustomerId"],
                ModifiedDate = reader.GetNullableDateTimeByName(nameof(Cart.ModifiedDate)),
                Customer = new Customer((int)reader[4], (int)reader["ShopId"], (string)reader["Name"], (string)reader["Email"]),
            };

        private void BasicSingleCartAssertions(Cart? cart, int cartId, string sessionId, int customerId)
        {
            Assert.IsNotNull(cart);
            Assert.That(cart.Id, Is.EqualTo(cartId));
            Assert.That(cart.SessionId, Is.EqualTo(sessionId));
            Assert.IsNotNull(cart.CustomerId);
            Assert.That(cart.CustomerId.Value, Is.EqualTo(customerId));
        }


        private void SingleCartWithCustomerAssertion(Cart? cart, int cartId, string sessionId, int customerId, int shopId, string name, string email)
        {
            BasicSingleCartAssertions(cart, cartId, sessionId, customerId);
            Assert.IsNotNull(cart?.Customer);
            Assert.That(cart.Customer.Id, Is.EqualTo(customerId));
            Assert.That(cart.Customer.ShopId, Is.EqualTo(shopId));
            Assert.That(cart.Customer.Name, Is.EqualTo(name));
            Assert.That(cart.Customer.Email, Is.EqualTo(email));
        }
    }
}
