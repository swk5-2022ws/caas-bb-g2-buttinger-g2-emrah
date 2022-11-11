using CaaS.Core.Domainmodels.DiscountActions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Unit.Domainmodels.DiscountActions
{
    [Category("Unit")]
    [TestFixture]
    public class DiscountActionBaseTest
    {
        [Test]
        public void TestSerializeDeserializeFixedValueDiscountAciton()
        {
            FixedValueDiscountAction sut = new(100.0);
            var json = DiscountActionBase.Serialize(sut);
            FixedValueDiscountAction actual = (FixedValueDiscountAction) DiscountActionBase.Deserialize(json);

            Assert.That(sut.Value, Is.EqualTo(actual.Value));
            Assert.That(sut.ApplyPriority, Is.EqualTo(actual.ApplyPriority));
        }

        [Test]
        public void TestSerializeDeserializeTotalPercentageDiscountAciton()
        {
            TotalPercentageDiscountAction sut = new(0.5);
            var json = DiscountActionBase.Serialize(sut);
            TotalPercentageDiscountAction actual = (TotalPercentageDiscountAction)DiscountActionBase.Deserialize(json);

            Assert.That(sut.Percentage, Is.EqualTo(actual.Percentage));
            Assert.That(sut.ApplyPriority, Is.EqualTo(actual.ApplyPriority));
        }
    }
}
