using CaaS.Core.Domainmodels;
using CaaS.Core.Test.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Integration.Template
{
    public class ExtendedWhereTemplateTests
    {
        [Test]
        [TestCase(new int[] {1,2,3,6}, 3)]
        [TestCase(new int[] {1,2,3}, 3)]
        [TestCase(new int[] {1,2,3,6,9}, 3)]
        public async Task GetCustomerListWithoutDeleted(int[] ids, int nonDeletedCount)
        {
            var customers = await Setup.GetTemplateEngine().QueryAsync(ReadToCustomer, whereExpression: new
            {
                Id = ids
            });

            Assert.That(customers.Count(), Is.EqualTo(nonDeletedCount));
        }

        [Test, Rollback]
        [TestCase(1, "Test")]
        [TestCase(2, "Test 1")]
        [TestCase(3, "Test 3")]
        [TestCase(6, "Test 6", true)]
        public async Task UpdateCustomerNameById(int id, string name, bool isDeleted = false)
        {
            var whereExpression = new
            {
                Id = id
            };
            Assert.That(await Setup.GetTemplateEngine().UpdateAsync<Customer>(new
            {
                Name = name
            }, whereExpression), Is.EqualTo(!isDeleted ? 1 : 0));

            var customer = await Setup.GetTemplateEngine().QueryFirstOrDefaultAsync(ReadToCustomer, whereExpression: whereExpression, isSoftDeletionExcluded: false);

            Assert.IsNotNull(customer);
            Assert.That(customer.Id, Is.EqualTo(id));

            if (!isDeleted)
            {
                Assert.That(customer.Name, Is.EqualTo(name));
            }
            else
            {
                Assert.That(customer.Name, Is.Not.EqualTo(name));
            }
            Assert.That(customer.Id, Is.EqualTo(id));
        }

        #region DataReader
        private Customer ReadToCustomer(IDataRecord reader) => new Customer((int)reader["Id"], (int)reader["ShopId"], (string)reader["Name"], (string)reader["Email"]);
        #endregion DataReader
    }
}
