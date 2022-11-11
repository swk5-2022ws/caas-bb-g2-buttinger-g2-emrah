using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Domainmodels;
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
    internal class DiscountRuleRepositoryTest
    {
        private IDiscountRuleRepository sut;

        [OneTimeSetUp]
        public void InitializeSut()
        {
            sut = new DiscountRuleRepository(Setup.GetTemplateEngine());
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test, Rollback]
        public async Task TestGetWithValidIdReturnsDiscountRule(int id)
        {
            var rule = await sut.Get(id);
            Assert.That(rule, Is.Not.Null);
            Assert.That(rule.Id, Is.EqualTo(id));
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

        [Test, Rollback]
        public async Task TestCreateWIthValidDiscountRuleCreatesDiscountRule()
        {
            int shopId = 1;
            string name = "rule";
            string rule = DiscountRulesetBase.Serialize(new TotalAmountDiscountRuleset(100));
            DiscountRule? DiscountRule = new(0, shopId, name, rule);

            var id = await sut.Create(DiscountRule);
            DiscountRule = await sut.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(id, Is.AtLeast(1));
                Assert.That(DiscountRule, Is.Not.Null);
                Assert.That(DiscountRule?.ShopId, Is.EqualTo(shopId));
                Assert.That(DiscountRule?.Name, Is.EqualTo(name));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Assert.That(DiscountRulesetBase.Serialize(DiscountRule.RuleObject), Is.EqualTo(rule));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            });
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test, Rollback]
        public async Task TestUpdateWithValidDiscountRuleUpdatesDiscountRule(int id)
        {
            string name = "name";
            var percentagerule = new TotalAmountDiscountRuleset(500.0);
            DiscountRule? DiscountRule = await sut.Get(id) ?? throw new ArgumentNullException($"Could not find DiscountRule for id {id}");
            DiscountRule.Name = name;
            DiscountRule.RuleObject = percentagerule;
            await sut.Update(DiscountRule);
            DiscountRule = await sut.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(DiscountRule, Is.Not.Null);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Assert.That(DiscountRule.Id, Is.EqualTo(id));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                Assert.That(DiscountRule?.Ruleset, Is.EqualTo(DiscountRulesetBase.Serialize(percentagerule)));
                Assert.That(DiscountRule?.Name, Is.EqualTo(name));
            });
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [Test, Rollback]
        public async Task TestDeleteWithValidIdDeletesDiscount(int id)
        {
            DiscountRule? discountExists = await sut.Get(id);
            var actual = await sut.Delete(id);
            DiscountRule? discount = await sut.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(discountExists, Is.Not.Null);
                Assert.That(discount, Is.Null);
            });
        }
    }
}
