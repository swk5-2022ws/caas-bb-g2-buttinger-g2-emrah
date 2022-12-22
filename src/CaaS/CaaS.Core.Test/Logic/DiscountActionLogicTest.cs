using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util.MemoryRepositories;
using CaaS.Core.Test.Util.RepositoryStubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaaS.Core.Domainmodels.DiscountActions;

namespace CaaS.Core.Test.Logic
{
    [Category("Unit")]
    [TestFixture]
    public class DiscountActionLogicTest
    {
        private DiscountActionLogic sut;
        private IDiscountActionRepository discountActionRepository;
        private IShopRepository shopRepository;

        private static readonly Guid appKey = Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906");

        [SetUp]
        public void InitializeSut()
        {
            discountActionRepository = new DiscountActionRepositoryStub(new Dictionary<int, DiscountAction>()
            {
                {1, new DiscountAction(1, 1, "valid", new FixedValueDiscountAction(100.0))},
                {2, new DiscountAction(3, 1, "invalid", new FixedValueDiscountAction(100.0))},
                {3, new DiscountAction(3, 1, "invalid", new FixedValueDiscountAction(100.0))},
            });

            shopRepository = new ShopRepositoryStub(new Dictionary<int, Shop>()
            {
                {1, new Shop(1, 1, appKey, "shop") }
            });

            sut = new DiscountActionLogic(discountActionRepository, shopRepository);
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

            var discount = await discountActionRepository.Get(1);

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
            DiscountAction action = new(1, 1, "SWK beste", new TotalPercentageDiscountAction(0.5d));

            int id = await sut.Create(appKey, action);

            var actual = await discountActionRepository.Get(id);
            Assert.Multiple(() =>
            {
                Assert.That(id, Is.AtLeast(1));
                Assert.That(action, Is.EqualTo(actual));
            });
        }

        [Test]
        public void TestCreateWithInvalidAppKeyThrowsUnauthorizedException()
        {
            DiscountAction action = new(1, 1, "SWK beste", new TotalPercentageDiscountAction(0.5d));

            Assert.CatchAsync<UnauthorizedAccessException>(async () => await sut.Create(Guid.NewGuid(), action));
        }

        [Test]
        public void TestCreateWithNullThrowsArgumentNullException()
        {
            Assert.CatchAsync<ArgumentNullException>(async () => await sut.Create(appKey, null!));
        }
    }
}
