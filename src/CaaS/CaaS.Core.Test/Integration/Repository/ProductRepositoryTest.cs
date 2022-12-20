using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;
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
    public class ProductRepositoryTest
    {
        private readonly IProductRepository sut = new ProductRepository(Test.Setup.GetTemplateEngine());


        [Test]
        [TestCase(2, 2, "Reverse-engineered bi-directional function", "Facilis ut assumenda nihil enim a dolor repellat. Distinctio vel quia voluptas et aperiam omnis. Repellendus quibusdam fuga et sit quod sed.", 660.75, "https://loremflickr.com/640/480/")]
        [TestCase(4, 4, "Cross-platform fresh-thinking algorithm", "Culpa reprehenderit excepturi quia voluptas eos dolor quisquam quibusdam. Explicabo et quo sed perspiciatis sit alias cupiditate. Qui occaecati quisquam aut. Ea eligendi maiores adipisci.", 492.23, "https://loremflickr.com/640/480/")]
        [TestCase(5, 5, "Adaptive value-added frame", "Rerum velit qui omnis perferendis officiis neque. Assumenda laborum maiores rem vel similique eveniet facere. Maxime qui perspiciatis voluptate fuga. Occaecati laudantium quia doloremque nostrum animi consequuntur.", 489.67, "https://loremflickr.com/640/480/")]
        [TestCase(6, 6, "Object-based client-server definition", "Ut aperiam repellendus quam placeat ut tempore doloremque. Atque omnis quae eaque aliquam beatae. Quae aliquid quidem optio et ut et est inventore. Rerum voluptates autem facere voluptates repellat vitae sint.", 38.16, "https://loremflickr.com/640/480/")]
        public async Task TestGetByIdWithValidIdReturnsProduct(int id, int shopId, string label, string description, double price, string imageUrl)
        {
            Product? product = await sut.Get(id);

            Assert.NotNull(product);
            Assert.Multiple(() =>
            {
                Assert.That(product.Id, Is.EqualTo(id));
                Assert.That(product.ShopId, Is.EqualTo(shopId));
                Assert.That(product.Description, Is.EqualTo(description));
                Assert.That(product.ImageUrl, Is.EqualTo(imageUrl));
                Assert.That(product.Label, Is.EqualTo(label));
                Assert.That(product.Price, Is.EqualTo(price));
                Assert.That(product.Deleted, Is.Null);
            });
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        public async Task TestGetByIdWithValidIdForDeletedProductsReturnsNull(int id)
        {
            Product? product = await sut.Get(id);
            Assert.That(product, Is.Null);
        }

        [Test]
        public async Task TestGetByIsdWithValidIdsReturnsProducts()
        {
            var products = await sut.Get(new List<int>() { 2, 4, 5, 6});

            Assert.NotNull(products);
            Assert.That(products.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task TestGetByIdsWithValidIdsForDeletedProductsReturnsEmptyProducts()
        {
            var products = await sut.Get(new List<int> { 1, 3});
            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCase(1, 236)]
        [TestCase(2, 246)]
        [TestCase(3, 231)]
        public async Task TestGetByShopIdWithValidShopIdReturnProducts(int shopId, int count)
        {
            IList<Product> products = await sut.GetByShopId(shopId);

            Assert.Multiple(() =>
            {
                Assert.That(products.Count, Is.EqualTo(count));
                foreach(var product in products)
                {
                    Assert.That(product, Is.Not.Null);
                    Assert.That(product.ShopId, Is.EqualTo(shopId));
                    Assert.That(product.Deleted, Is.Null);
                }
            });
        }

        [Test]
        public async Task TestGetByShopIdWithInvalidShopIdReturnEmptyList()
        {
            IList<Product> products = await sut.GetByShopId(int.MaxValue);
            Assert.Multiple(() =>
            {
                Assert.That(products, Is.Not.Null);
                Assert.That(products.Count, Is.EqualTo(0));
            });
        }

        [Test]
        [TestCase(1, "well", 2)]
        [TestCase(2, "Reverse", 5)]
        [TestCase(3, "Optimized", 2)]
        public async Task TestGetByShopIdWithFilterWithValidParamsReturnProducts(int shopId, string filter, int count)
        {
            IList<Product> products = await sut.GetByShopIdWithFilter(shopId, filter);

            Assert.Multiple(() =>
            {
                Assert.That(products.Count, Is.EqualTo(count));
                foreach (var product in products)
                {
                    Assert.That(product, Is.Not.Null);
                    Assert.That(product.ShopId, Is.EqualTo(shopId));
                    Assert.That(product.Deleted, Is.Null);
                }
            });
        }

        [Test]
        public async Task TestGetByShopIdWithFilterInvalidShopIdReturnEmptyList()
        {
            IList<Product> products = await sut.GetByShopIdWithFilter(int.MaxValue, "ANY");
            Assert.Multiple(() =>
            {
                Assert.That(products, Is.Not.Null);
                Assert.That(products.Count, Is.EqualTo(0));
            });
        }


        [TestCase(1, "Description 1", "http://test.org", "Label 1", 100.0)]
        [TestCase(1, "", "", "", 0.0)]
        [TestCase(1, "", "", "", -1.0)]
        [Test, Rollback]
        public async Task TestCreateWithValidShopCreatesShop(
            int shopId, string description, string imageUrl, string label, double price)
        {
            Product product = new(0, shopId, description, imageUrl, label, price);
            int id = await sut.Create(product);
            product = await sut.Get(id) ?? throw new Exception($"Could not fetch created shop");

            Assert.Multiple(() =>
            {
                Assert.That(id, Is.AtLeast(1));
                Assert.That(product, Is.Not.Null);
                Assert.That(product.ShopId, Is.EqualTo(shopId));
                Assert.That(product.Description, Is.EqualTo(description));
                Assert.That(product.ImageUrl, Is.EqualTo(imageUrl));
                Assert.That(product.Label, Is.EqualTo(label));
                Assert.That(product.Price, Is.EqualTo(price));
                Assert.That(product.Deleted, Is.Null);
            });
        }

        [Test, Rollback]
        public void TestCreateWithInvalidShopIdThrowsException()
        {
            Product product = new(0, int.MaxValue, "Description", "ImageUrl", "Label", 100.0);
            Assert.CatchAsync(async () => await sut.Create(product));
        }

        [TestCase(2, 2, "new label", "new description", 1000.0, "http://test.new")]
        [TestCase(4, 4, "new label", "new description", double.MinValue, "http://test.new")]
        [TestCase(5, 5, "", "", 0.0, "")]
        [TestCase(6, 6, "new label", "new description", double.MaxValue, "http://test.new")]
        [Test, Rollback]
        public async Task TestUpdateWithValidValuesUpdatesShop(int id, int shopId, string label, string description, double price, string url)
        {
            Product product = await sut.Get(id) ?? throw new Exception($"Shop with id {id} not found.");
            product.Label = label;
            product.Description = description;
            product.Price = price;
            product.ImageUrl = url;

            bool isUpdateSuccess = await sut.Update(product);

            product = await sut.Get(id) ?? throw new Exception($"Shop with id {id} not found after update.");

            Assert.Multiple(() =>
            {
                Assert.That(isUpdateSuccess, Is.True);
                Assert.That(product, Is.Not.Null);
                Assert.That(product.ShopId, Is.EqualTo(shopId));
                Assert.That(product.Description, Is.EqualTo(description));
                Assert.That(product.ImageUrl, Is.EqualTo(url));
                Assert.That(product.Label, Is.EqualTo(label));
                Assert.That(product.Price, Is.EqualTo(price));
                Assert.That(product.Deleted, Is.Null);
            });
        }

        [Test, Rollback]
        public async Task TestUpdateWithNewShopReturnFalse()
        {
            Product product = new(0, 0, "", "", "", 0);
            bool isUpdateSuccess = await sut.Update(product);
            Assert.That(isUpdateSuccess, Is.False);
        }

        [Test, Rollback]
        public async Task TestDeleteWithValidShopDeletesShop()
        {
            int productId = 2;
            Product? product = await sut.Get(productId);

            await sut.Delete(productId);

            Product? refetchedProduct = await sut.Get(productId);

            Assert.Multiple(() =>
            {
                Assert.That(product, Is.Not.Null);
                Assert.That(refetchedProduct, Is.Null);
            });
        }
    }

}
