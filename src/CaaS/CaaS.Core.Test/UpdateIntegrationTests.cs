using CaaS.Core.Domainmodels;
using System.Data;
using System.Transactions;

namespace CaaS.Core.Test
{
    public class UpdateIntegrationTests
    {
        [Test]
        [TestCase(1, "Test")]
        [TestCase(2, "Test 1")]
        [TestCase(3, "Test 3")]
        public async Task UpdateShopNameByShopId(int shopId, string label)
        {
            using(var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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

                Assert.IsNotNull(shop);
                Assert.That(shop.Label, Is.EqualTo(label));
            }
        }

        [Test]
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

                Assert.IsNotNull(shop);
                Assert.That(shop.Label, Is.EqualTo(label));
                Assert.That(shop.AppKey, Is.EqualTo(appKey));
            }
        }

        private Shop ReadToShop(IDataRecord reader) =>
            new Shop((int)reader["Id"], (int)reader["TenantId"], (Guid)reader["AppKey"], (string)reader["Label"]);
    }
}
