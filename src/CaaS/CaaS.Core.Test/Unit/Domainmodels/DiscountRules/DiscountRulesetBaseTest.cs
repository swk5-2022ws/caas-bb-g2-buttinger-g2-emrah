using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Unit.Domainmodels.DiscountRules
{
    [Category("Unit")]
    [TestFixture]
    public class DiscountRulesetBaseTest
    {
        [Test]
        public void TestSerializeDeserializeDateDiscountRuleset()
        {
            DateDiscountRuleset sut = new(DateTime.Now, DateTime.Now, DateTime.Now);
            var json = DiscountRulesetBase.Serialize(sut);
            DateDiscountRuleset actual = (DateDiscountRuleset)DiscountRulesetBase.Deserialize(json);
            Assert.Multiple(() =>
            {
                Assert.That(sut.StartDate, Is.EqualTo(actual.StartDate));
                Assert.That(sut.EndDate, Is.EqualTo(actual.EndDate));
                Assert.That(sut.OrderDate, Is.EqualTo(actual.OrderDate));
            });
        }

        [Test]
        public void TestSerializeDeserializeTotalAmountDiscountRuleset()
        {
            TotalAmountDiscountRuleset sut = new(100.0);
            var json = DiscountRulesetBase.Serialize(sut);
            TotalAmountDiscountRuleset actual = (TotalAmountDiscountRuleset)DiscountRulesetBase.Deserialize(json);

            Assert.That(sut.MinimumTotalAmount, Is.EqualTo(actual.MinimumTotalAmount));

        }
    }
}
