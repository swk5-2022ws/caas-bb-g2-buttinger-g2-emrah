using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Integration.Repository
{
    [Category("Integration")]
    [TestFixture]
    public class TenantRepositoryTest
    {
        private readonly ITenantRepository sut = new TenantRepository(Test.Setup.GetTemplateEngine());

        [Test]
        [TestCase(1, "yoberbrunner@example.org", "Prof. Cathy Anderson PhD")]
        [TestCase(2, "zhuel@example.com", "Mr. Martin Ebert")]
        [TestCase(3, "nmitchell@example.org","Melissa Purdy")]
        public async Task TestGetWithValidIdReturnsValidTenant(int id, string email, string name)
        {
            Domainmodels.Tenant? tenant = await sut.Get(id);
            Assert.That(tenant, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(tenant.Id, Is.EqualTo(id));
                Assert.That(tenant.Email, Is.EqualTo(email));
                Assert.That(tenant.Name, Is.EqualTo(name));
            });
        }

        [Test]
        public async Task TestGetWithInvalidIdReturnsNull()
        {
            int id = int.MaxValue;
            Domainmodels.Tenant? tenant = await sut.Get(id);
            Assert.That(tenant, Is.Null);
        }
    }
}
