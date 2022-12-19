using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Integration.Repository
{
    [Category("Integration")]
    [TestFixture]
    public class DiscountCartRepositoryTest
    {
        private readonly IDiscountCartRepository sut = new DiscountCartRepository(Test.Setup.GetTemplateEngine());


        [TestCase(1, 2)]
        [TestCase(2, 2)]
        [Test]
        public async Task TestGetByCartIdReturnsDiscountCarts(int cartId, int expectedCount)
        {
            IList<Domainmodels.DiscountCart> actual = await sut.GetByCartId(cartId);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Has.Count.EqualTo(expectedCount));
                Assert.That(actual.All(x => x.CartId== cartId), Is.True);
            });
        }

        [Test, Rollback]
        public async Task TestCreateWithValidIdsReturnsNewId()
        {
            bool isCreated = await sut.Create(new Domainmodels.DiscountCart(1, 21));

            var cartDiscounts = await sut.GetByCartId(1);
            Assert.Multiple(() =>
            {
                Assert.That(cartDiscounts.Any(x => x.DiscountId == 21), Is.True);
                Assert.That(isCreated, Is.True);
            });
        }

        [Test, Rollback]
        public void TestCreateWithExistingIdsThrowsException()
        {
            Assert.CatchAsync(async () => await sut.Create(new Domainmodels.DiscountCart(1, 1)));
        }

        [Test, Rollback]
        public async Task TestDeleteWithValidIdsReturnsTrue()
        {
            var isDeleted = await sut.Delete(new Domainmodels.DiscountCart(1, 1));
            Assert.That(isDeleted, Is.True);
        }

        [Test, Rollback]
        public async Task TestDeleteWithInvalidIdsReturnsFalse()
        {
            var isDeleted = await sut.Delete(new Domainmodels.DiscountCart(int.MaxValue, int.MaxValue));
            Assert.That(isDeleted, Is.False);
        }
    }
}
