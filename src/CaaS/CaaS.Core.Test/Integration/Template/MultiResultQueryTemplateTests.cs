using CaaS.Core.Domainmodels;
using System.Data;

namespace CaaS.Core.Test.Integration.Template
{
    public class MultiResultQueryTemplateTests
    {
        [Test]
        [TestCase(1, 25, new int[] { 1, 21, 41, 61, 81, 101, 121, 141, 161, 181, 201, 221, 241, 261, 281, 301, 321, 341, 361, 381, 401, 421, 441, 461, 481 })]
        [TestCase(2, 25, new int[] { 2, 22, 42, 62, 82, 102, 122, 142, 162, 182, 202, 222, 242, 262, 282, 302, 322, 342, 362, 382, 402, 422, 442, 462, 482 })]
        [TestCase(3, 25, new int[] { 3, 23, 43, 63, 83, 103, 123, 143, 163, 183, 203, 223, 243, 263, 283, 303, 323, 343, 363, 383, 403, 423, 443, 463, 483 })]
        public async Task SelectMultipleCustomersByShopIdReturnCustomers(int shopId, int customerCount, int[] customerIds)
        {
            var customers = await Setup.GetTemplateEngine().QueryAsync(ReadToCustomer, whereExpression: new
            {
                ShopId = shopId
            }, isSoftDeletionExcluded: false);

            Assert.IsNotNull(customers);
            Assert.That(customers.Count, Is.EqualTo(customerCount));
            Assert.That(customers.Select(c => c.Id).ToArray(), Is.EqualTo(customerIds));
        }

        [Test]
        [TestCase(1, 1, new int[] { 1 })]
        [TestCase(2, 1, new int[] { 2 })]
        [TestCase(3, 1, new int[] { 3 })]
        public async Task SelectMultipleCustomersByCustomerIdReturnSingleCustomer(int customerId, int customerCount, int[] customerIds)
        {
            var customers = await Setup.GetTemplateEngine().QueryAsync(ReadToCustomer, whereExpression: new
            {
                Id = customerId
            });

            Assert.IsNotNull(customers);
            Assert.That(customers.Count, Is.EqualTo(customerCount));
            Assert.That(customers.Select(c => c.Id).ToArray(), Is.EqualTo(customerIds));
        }

        private Customer ReadToCustomer(IDataRecord reader) => new Customer((int)reader["Id"], (int)reader["ShopId"], (string)reader["Name"], (string)reader["Email"]);
    }
}
