using CaaS.Core.Domainmodels;
using CaaS.Core.Test.Util;
using System.Data;
using System.Transactions;

namespace CaaS.Core.Test.Integration.Template
{
    [Category("Integration")]
    [TestFixture]
    public class UpdateTemplateTests
    {
        [Test, Rollback]
        [TestCase(1, "Test")]
        [TestCase(2, "Test 1")]
        [TestCase(3, "Test 3")]
        public async Task UpdateShopNameByShopId(int shopId, string label)
        {
            var whereExpression = new
            {
                Id = shopId
            };
            Assert.That(await Setup.GetTemplateEngine().UpdateAsync<Shop>(new
            {
                Label = label
            }, whereExpression), Is.EqualTo(1));

            var shop = await Setup.GetTemplateEngine().QueryFirstOrDefaultAsync(ReadToShop, whereExpression: whereExpression);
            BaseShopAssertion(shop, label);
        }

        [Test, Rollback]
        [TestCase(1, "Test")]
        [TestCase(2, "Test 1")]
        [TestCase(3, "Test 3")]
        public async Task UpdateLabelAndAppKeyByShopId(int shopId, string label)
        {
            var appKey = Guid.NewGuid();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var whereExpression = new
                {
                    Id = shopId
                };
                Assert.That(await Setup.GetTemplateEngine().UpdateAsync<Shop>(new
                {
                    Label = label,
                    AppKey = appKey
                }, whereExpression), Is.EqualTo(1));

                var shop = await Setup.GetTemplateEngine().QueryFirstOrDefaultAsync(ReadToShop, whereExpression: whereExpression);

                BaseShopAssertion(shop, label);
                Assert.That(shop?.AppKey, Is.EqualTo(appKey));
            }
        }

        private void BaseShopAssertion(Shop? shop, string label)
        {
            Assert.IsNotNull(shop);
            Assert.That(shop.Label, Is.EqualTo(label));
        }

        private Shop ReadToShop(IDataRecord reader) =>
            new Shop((int)reader["Id"], (int)reader["TenantId"], (Guid)reader["AppKey"], (string)reader["Label"]);
    }
}
