using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Test.Integration.Repository
{
    public class ProductRepositoryTest
    {
        private readonly IProductRepository sut = new ProductRepository(Test.Setup.GetTemplateEngine());


        [Test]
        //[TestCase(1, 1, "Re-contextualized well-modulated paradigm", "Eum pariatur molestiae molestias et natus nobis. Nostrum repudiandae in quaerat et officiis. Ut in iste tempore. Aut perferendis nesciunt totam iusto nisi quibusdam. Autem ratione ut asperiores et quae.", 641.59, "https://loremflickr.com/640/480/", "1988-09-09 23:10:48")]
        //[TestCase(3, 3, "Optimized methodical knowledgeuser", "Atque fugiat quia sit. Ullam voluptatem odit sed est ea eligendi. Alias laudantium dicta ut iusto cumque pariatur ut. Enim et explicabo consequatur et et ut quod.", 385.37, "https://loremflickr.com/640/480/", "1976-07-03 05:00:44")]
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
        public async Task TestGetWithInvalidIdReturnsNull()
        {
            //int id = int.MaxValue;
            //Shop? shop = await sut.Get(id);
            //Assert.That(shop, Is.Null);
        }
    }
}
