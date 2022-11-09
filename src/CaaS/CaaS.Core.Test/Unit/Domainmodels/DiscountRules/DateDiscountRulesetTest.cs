using CaaS.Core.Domainmodels.DiscountRules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Unit.Domainmodels.DiscountRules
{
    public class DateDiscountRulesetTest
    {
        [TestCase(-1, 1)]
        [TestCase(-10000, 10000)]
        [Test]
        public void TestIsQualifiedForDiscountWithQualifiedDateReturnsTrue(int offsetMinutesStartDate, int offsetMinutesEndDate)
        {
            DateTime now = DateTime.Now;

            var start = now.AddMinutes(offsetMinutesStartDate);
            var end = now.AddMinutes(offsetMinutesEndDate);
            DateDiscountRuleset sut = new(start, end, now);
            bool result = sut.IsQualifiedForDiscount(new Core.Domainmodels.Cart(0, ""));

            Assert.That(result, Is.True);
        }

        [TestCase(-2, -1)]
        [TestCase(+1, +2)]
        [Test]
        public void TestIsQualifiedForDiscountWithInqualifiedDateReturnsFalse(int offsetMinutesStartDate, int offsetMinutesEndDate)
        {
            DateTime now = DateTime.Now;

            var start = now.AddMinutes(offsetMinutesStartDate);
            var end = now.AddMinutes(offsetMinutesEndDate);
            DateDiscountRuleset sut = new(start, end, now);
            bool result = sut.IsQualifiedForDiscount(new Core.Domainmodels.Cart(0, ""));

            Assert.That(result, Is.False);
        }

        [Test]
        public void TestCtorWithWrongStartAndEndDatesThrowsException()
        {
            DateTime now = DateTime.Now;

            var start = now.AddMinutes(+1);
            var end = now;

            Assert.Throws<ArgumentException>(() => new DateDiscountRuleset(start, end, now));
        }
    }
}
