using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util.RepositoryStubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Test.Util.MemoryRepositories;

namespace CaaS.Core.Test.Logic
{
    [Category("Unit")]
    [TestFixture]
    public class DiscountRuleLogicTest
    {
        private IDiscountRuleLogic sut;
        private IDiscountRuleRepository discountRuleRepository;
        private IShopRepository shopRepository;

        private static readonly Guid appKey = Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906");

        [SetUp]
        public void InitializeSut()
        {
            discountRuleRepository = new DiscountRuleRepositoryStub(new Dictionary<int, DiscountRule>()
            {
                {1, new DiscountRule(1, 1, "valid",
                        new DateDiscountRuleset(DateTime.Now.AddMinutes(-5), DateTime.Now.AddMinutes(5), DateTime.Now)) },
                {2, new DiscountRule(2, 1, "valid",
                        new TotalAmountDiscountRuleset(300.0)) },
                {3, new DiscountRule(3, 1, "invalid",
                        new TotalAmountDiscountRuleset(500.0))},
                {4, new DiscountRule(4, 2, "invalid",
                        new TotalAmountDiscountRuleset(500.0))}
            });

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, appKey, "shop") }
            });

            sut = new DiscountRuleLogic(discountRuleRepository, shopRepository);
        }

        [Test]
        public async Task TestGetRulesets()
        {
            var rules = (await sut.GetRulesets()).ToList();

            Assert.That(rules, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task TestGetByShopIdWithValidShopIdReturnsRules()
        {
            var rules = (await sut.GetByShopId(appKey, 1)).ToList();

            Assert.That(rules, Has.Count.EqualTo(3));
        }

        [Test]
        public void TestGetByShopIdWithInvalidShopIdThrowsArgumentException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.GetByShopId(appKey, int.MaxValue));
        }

        [Test]
        public void TestGetByShopIdWithInvalidAppKeyThrowsUnauthorizedException()
        {
            Assert.CatchAsync<UnauthorizedAccessException>(async () => await sut.GetByShopId(Guid.NewGuid(), 1));
        }

        [Test]
        public async Task TestDeleteWithValidRuleIdDeletesShop()
        {
            var isDeleted = await sut.Delete(appKey, 1);

            var discount = await discountRuleRepository.Get(1);

            Assert.Multiple(() =>
            {
                Assert.That(isDeleted, Is.True);
                Assert.That(discount, Is.Null);
            });
        }

        [Test]
        public void TestDeleteWithInvalidRuleIdThrowsArgumentException()
        {
            Assert.CatchAsync<ArgumentException>(async () => await sut.Delete(appKey, int.MaxValue));
        }

        [Test]
        public void TestDeleteWithInvalidAppKeyThrowsArgumentException()
        {
            Assert.CatchAsync<UnauthorizedAccessException>(async () => await sut.Delete(Guid.NewGuid(), 1));
        }

        [Test]
        public async Task TestCreateWithValidDiscountRuleReturnsId()
        {
            DiscountRule rule = new(1, 1, "SWK beste", new TotalAmountDiscountRuleset(50));

            int id = await sut.Create(appKey, rule);

            var actual = await discountRuleRepository.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(id, Is.AtLeast(1));
                Assert.That(rule, Is.EqualTo(actual));
            });
        }

        [Test]
        public void TestCreateWithInvalidAppKeyThrowsUnauthorizedException()
        {
            DiscountRule rule = new(1, 1, "SWK beste", new TotalAmountDiscountRuleset(50));

            Assert.CatchAsync<UnauthorizedAccessException>(async () => await sut.Create(Guid.NewGuid(), rule));
        }

        [Test]
        public void TestCreateWithNullThrowsArgumentNullException()
        {
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.Create(appKey, null!));
        }

    }
}
