using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountActions;
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
    public class DiscountActionRepositoryTest
    {
        private IDiscountActionRepository sut;

        [OneTimeSetUp]
        public void InitializeSut()
        {
            sut = new DiscountActionRepository(Setup.GetTemplateEngine());
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test, Rollback]
        public async Task TestGetWithValidIdReturnsDiscountAction(int id)
        {
            var action = await sut.Get(id);
            Assert.That(action, Is.Not.Null);
            Assert.That(action.Id, Is.EqualTo(id));
        }

        [TestCase(1, 10)]
        [TestCase(2, 10)]
        [TestCase(3, 10)]
        [Test, Rollback]
        public async Task TestGetByShopIdWithValidIdReturnsDiscount(int shopId, int expectedCount)
        {
            var discounts = await sut.GetByShopId(shopId);
            Assert.That(discounts, Has.Count.EqualTo(expectedCount));
        }

        [Test, Rollback]
        public async Task TestCreateWIthValidDiscountActionCreatesDiscountAction()
        {
            int shopId = 1;
            string name = "action";
            string action = DiscountActionBase.Serialize(new FixedValueDiscountAction(100));
            DiscountAction? discountAction = new(0, shopId, name, action);

            var id = await sut.Create(discountAction);
            discountAction = await sut.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(id, Is.AtLeast(1));
                Assert.That(discountAction, Is.Not.Null);
                Assert.That(discountAction?.ShopId, Is.EqualTo(shopId));
                Assert.That(discountAction?.Name, Is.EqualTo(name));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Assert.That(DiscountActionBase.Serialize(discountAction.ActionObject), Is.EqualTo(action));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            });
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test, Rollback]
        public async Task TestUpdateWithValidDiscountActionUpdatesDiscountAction(int id)
        {
            string name = "name";
            var percentageAction = new TotalPercentageDiscountAction(0.5);
            DiscountAction? discountAction = await sut.Get(id) ?? throw new ArgumentNullException($"Could not find DiscountAction for id {id}");
            discountAction.Name = name;
            discountAction.ActionObject = percentageAction;
            await sut.Update(discountAction);
            discountAction = await sut.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(discountAction, Is.Not.Null);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Assert.That(discountAction.Id, Is.EqualTo(id));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                Assert.That(discountAction?.Action, Is.EqualTo(DiscountActionBase.Serialize(percentageAction)));
                Assert.That(discountAction?.Name, Is.EqualTo(name));
            });
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test, Rollback]
        public async Task TestDeleteWithValidIdDeletesDiscount(int id)
        {
            DiscountAction? discountExists = await sut.Get(id);
            var actual = await sut.Delete(id);
            DiscountAction? discount = await sut.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(discountExists, Is.Not.Null);
                Assert.That(discount, Is.Null);
            });
        }
    }
}
