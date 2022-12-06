using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;
using CaaS.Core.Transferrecordes;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Integration.Repository
{
    [Category("Integration")]
    [TestFixture]
    public class DiscountRepositoryTest
    {
        private IDiscountRepository sut;
        [OneTimeSetUp]
        public void InitializeSut()
        {
            sut = new DiscountRepository(Setup.GetTemplateEngine());
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test, Rollback]
        public async Task TestGetWithValidIdReturnsDiscount(int discountId)
        {
            var discount = await sut.Get(discountId);
            Assert.That(discount, Is.Not.Null);
            Assert.That(discount.Id, Is.EqualTo(discountId));
        }

        [TestCase(1, 5)]
        [TestCase(2, 5)]
        [TestCase(3, 5)]
        [Test, Rollback]
        public async Task TestGetByShopIdWithValidIdReturnsDiscount(int shopId, int expectedCount)
        {
            var discounts = await sut.GetByShopId(shopId);
            Assert.That(discounts, Has.Count.EqualTo(expectedCount));
        }

        [TestCase(1, 1)]
        [TestCase(21, 21)]
        [TestCase(41, 61)]
        [Test, Rollback]
        public async Task TestCreateWIthValidDiscountCreatesDiscount(int actionId, int ruleId)
        {
            Discount? discount = new(0, actionId, ruleId);
            var id = await sut.Create(discount);
            discount = await sut.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(id, Is.AtLeast(1));
                Assert.That(discount, Is.Not.Null);
                Assert.That(discount?.ActionId, Is.EqualTo(actionId));
                Assert.That(discount?.RuleId, Is.EqualTo(ruleId));
            });
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test, Rollback]
        public async Task TestUpdateWithValidDiscountUpdatesDiscount(int id)
        {
            Discount? discount = await sut.Get(id) ?? throw new ArgumentNullException($"No discount with id {id} found");
            discount.ActionId = 42;
            discount.RuleId = 42;
            await sut.Update(discount);
            discount = await sut.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(discount, Is.Not.Null);
                Assert.That(discount?.Id, Is.EqualTo(id));
                Assert.That(discount?.ActionId, Is.EqualTo(42));
                Assert.That(discount?.RuleId, Is.EqualTo(42));
            });
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test, Rollback]
        public async Task TestDeleteWithValidIdDeletesDiscount(int id)
        {
            Discount? discountExists = await sut.Get(id);
            var actual = await sut.Delete(id);
            Discount? discount = await sut.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(discountExists, Is.Not.Null);
                Assert.That(discount, Is.Null);
            });
        }
    }
}
