using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;

namespace CaaS.Core.Test.Integration.Repository
{
    public class CustomerRepositoryTest
    {
        private ICustomerRepository sut;
        [OneTimeSetUp]
        public void InitializeSut()
        {
            sut = new CustomerRepository(Setup.GetTemplateEngine());
        }

        [Test]
        [TestCase(1, 1, "hilll.mireya@example.com", "Alexa Adams II")]
        [TestCase(2, 2, "mweber@example.net", "Mr. Narciso Klein")]
        [TestCase(3, 3, "jgreenfelder@example.net", "Colby Wiza")]
        public async Task GetCustomerByIdWithValidIdReturnsCustomer(int id, int shopId, string email, string name)
        {
            var customer = await sut.Get(id);

            Assert.That(customer, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(customer.ShopId, Is.EqualTo(shopId));
                Assert.That(customer.Email, Is.EqualTo(email));
                Assert.That(customer.Name, Is.EqualTo(name));
            });
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(501)]
        public async Task GetCustomerByIdWithInvalidValidIdReturnsNull(int id) => 
            Assert.That(await sut.Get(id), Is.Null);

        [Test]
        [TestCase(1, 11)]
        [TestCase(2, 15)]
        [TestCase(3, 11)]
        public async Task GetCustomersByShopIdWithValidShopIdReturnsCustomerList(int shopId, int listCount)
        {
            var customers = await sut.GetAllByShopId(shopId);

            Assert.That(customers, Is.Not.Null);
            Assert.That(customers.Count, Is.EqualTo(listCount));
            Assert.That(customers.Any(customer => customer is null), Is.False);
            Assert.That(customers.All(customer => customer.ShopId == shopId), Is.True);
            Assert.That(customers.All(customer => customer.Deleted is null), Is.True);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(21)]
        public async Task GetCustomersByShopIdWithInvalidShopIdReturnsEmptyList(int shopId)
        {
            var customers = await sut.GetAllByShopId(shopId);
            Assert.That(customers, Is.Not.Null);
            Assert.That(customers.Count, Is.EqualTo(0));
        }

        [Test, Rollback]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(7, 7)]
        public async Task DeleteCustomerByValidIdReturnsTrue(int id, int shopId)
        {
            var customersPreDelete = await sut.GetAllByShopId(shopId);
            var deleted = await sut.Delete(id);
            var customersAfterDelete = await sut.GetAllByShopId(shopId);
            
            Assert.That(deleted, Is.True);
            Assert.That(customersPreDelete.Count, Is.EqualTo(customersAfterDelete.Count + 1));

            var deletedCustomer = await sut.Get(id);
            Assert.That(deletedCustomer, Is.Null);
        }

        [Test, Rollback]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(501)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task DeleteCustomerByInvalidIdReturnsFalse(int id) =>
            Assert.That(await sut.Delete(id), Is.False);

        [Test, Rollback]
        [TestCase(6, 6)]
        [TestCase(9, 9)]
        [TestCase(10, 10)]
        public async Task DeleteCustomerByDeletedIdReturnsFalse(int id, int shopId)
        {
            var customersPreDelete = await sut.GetAllByShopId(shopId);
            var deleted = await sut.Delete(id);
            var customersAfterDelete = await sut.GetAllByShopId(shopId);

            Assert.That(deleted, Is.False);
            Assert.That(customersPreDelete.Count, Is.EqualTo(customersAfterDelete.Count));

            Assert.That(await sut.Get(id), Is.Null);
        }

        [Test, Rollback]
        [TestCase(1, "testMail", "name", 1)]
        [TestCase(2, "testMail2", "name2", 2)]
        [TestCase(3, "testMail3", "name3", 3)]
        public async Task CreateCustomerWithValidValuesReturnsId(int shopId, string email, string name, int cartId)
        {
            var previousCustomerCount = (await sut.GetAllByShopId(shopId)).Count;
            var insertedId = await sut.Create(new Domainmodels.Customer(0, shopId, name, email, cartId));
            var afterwardsCustomerCount = (await sut.GetAllByShopId(shopId)).Count;

            Assert.That(insertedId, Is.GreaterThan(0));
            Assert.That(afterwardsCustomerCount, Is.EqualTo(previousCustomerCount + 1));

            var insertedCustomer = await sut.Get(insertedId);
            Assert.That(insertedCustomer, Is.Not.Null);
            Assert.That(insertedCustomer.Id, Is.EqualTo(insertedId));
            Assert.That(insertedCustomer.ShopId, Is.EqualTo(shopId));
            Assert.That(insertedCustomer.Email, Is.EqualTo(email));
            Assert.That(insertedCustomer.Name, Is.EqualTo(name));
        }

        [Test, Rollback]
        [TestCase(-1, "testMail", "name", 1)]
        [TestCase(0, "testMail2", "name2", 2)]
        [TestCase(int.MaxValue, "testMail3", "name3", 3)]
        [TestCase(int.MinValue, "testMail3", "name3", 4)]
        public void CreateCustomerWithInValidValuesReturnsException(int shopId, string email, string name, int cartId) =>
            Assert.CatchAsync(async () => await sut.Create(new Customer(0, shopId, name, email, cartId)));

        [Test, Rollback]
        [TestCase(1, -1, "testMail", "name", 1)]
        [TestCase(2, 0, "testMail2", "name2", 2)]
        [TestCase(3, int.MaxValue, "testMail3", "name3", 3)]
        [TestCase(4, int.MinValue, "testMail3", "name3", 4)]
        public void UpdateCustomerWithInValidValuesReturnsException(int id, int shopId, string email, string name, int cartId) =>
            Assert.CatchAsync(async () => await sut.Update(new Customer(id, shopId, name, email, cartId)));

        [Test, Rollback]
        [TestCase(1, 1, "testMail", "name", 1)]
        [TestCase(2, 2, "testMail2", "name2", 2)]
        [TestCase(3, 3, "testMail3", "name3", 3)]
        public async Task UpdateCustomerWithValidValuesReturnsTrue(int id, int shopId, string email, string name, int cartId)
        {
            var updated = await sut.Update(new Domainmodels.Customer(id, shopId, name, email, cartId));

            Assert.That(updated, Is.True);

            var updatedCustomer = await sut.Get(id);
            Assert.That(updatedCustomer, Is.Not.Null);
            Assert.That(updatedCustomer.Id, Is.EqualTo(id));
            Assert.That(updatedCustomer.ShopId, Is.EqualTo(shopId));
            Assert.That(updatedCustomer.Email, Is.EqualTo(email));
            Assert.That(updatedCustomer.Name, Is.EqualTo(name));
        }
    }
}
