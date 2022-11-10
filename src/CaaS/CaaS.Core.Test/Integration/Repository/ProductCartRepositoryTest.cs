using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Repository;
using CaaS.Core.Test.Util;

namespace CaaS.Core.Test.Integration.Repository
{
    public class ProductCartRepositoryTest
    {
        private IProductCartRepository sut;
        [OneTimeSetUp]
        public void InitializeSut()
        {
            sut = new ProductCartRepository(Setup.GetTemplateEngine());
        }

        [Test]
        [TestCase(-1, -1)]
        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        public async Task GetProductCartByInvalidProductIdAndCartIdReturnsNull(int productId, int cartId) =>
            Assert.That(await sut.Get(productId, cartId), Is.Null);

        [Test]
        [TestCase(1, 1, (uint)7, 4776.76)]
        [TestCase(2, 2, (uint)3, 4484.15)]
        [TestCase(3, 3, (uint)4, 626.68)]
        public async Task GetProductCartByValidProductIdAndCartIdReturnsProductCart(int productId, int cartId, uint amount, double price)
        {
            var productCart = await sut.Get(productId, cartId);

            BaseAssertions(productCart, cartId, productId, amount);
            Assert.That(productCart!.Price, Is.EqualTo(price));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public async Task GetProductCartsByInvalidCartIdReturnsEmptyProductCartList(int cartId) =>
            Assert.That((await sut.GetByCartId(cartId)).Count, Is.EqualTo(0));

        [Test]
        [TestCase(1, 50)]
        [TestCase(2, 50)]
        [TestCase(15, 50)]
        public async Task GetProductCartsByValidCartIdReturnsProductCartList(int cartId, int count)
        {
            var products = await sut.GetByCartId(cartId);

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count, Is.EqualTo(count));
            Assert.That(products.Any(product => product is null), Is.False);
            Assert.That(products.All(product => product.CartId == cartId), Is.True);
        }

        [Test, Rollback]
        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        [TestCase(-1, -1)]
        public async Task DeleteProductCartWithInvalidCartIdAndProductIdReturnsFalse(int cartId, int productId) =>
            Assert.That(await sut.Delete(productId, cartId), Is.False);

        [Test, Rollback]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        public async Task DeleteProductCartWithValidCartIdAndProductIdReturnsTrue(int cartId, int productId)
        {
            Assert.That(await sut.Delete(productId, cartId), Is.True);
            Assert.That(await sut.Get(productId, cartId), Is.Null);
        }

        [Test, Rollback]
        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        [TestCase(-1, -1)]
        public async Task UpdateAmountWithInvalidCartIdAndProductIdReturnFalse(int cartId, int productId) =>
            Assert.That(await sut.Update(productId, cartId, 1), Is.False);

        [Test, Rollback]
        [TestCase(4, 4, (uint)7)]
        [TestCase(5, 5, (uint)11)]
        [TestCase(6, 6, (uint)17)]
        public async Task UpdateAmountWithValidCartIdAndProductIdReturnsTrue(int cartId, int productId, uint amount)
        {
            Assert.That(await sut.Update(productId, cartId, amount), Is.True);

            var updatedProductCart = await sut.Get(productId, cartId);
            BaseAssertions(updatedProductCart, cartId, productId, amount);
        }

        [Test, Rollback]
        [TestCase(1, 10, (uint) 1, 10.0)]
        [TestCase(3, 30, (uint) 7, 11.3)]
        [TestCase(10, 5, (uint) 3, 10.3)]
        public async Task CreateProductCartWithValidPropertiesReturnsId(int cartId, int productId, uint amount, double price)
        {
            var insertedId = await sut.Create(new ProductCart(productId, cartId, price, amount));
            Assert.That(insertedId, Is.GreaterThan(0));

            var insertedProductCart = await sut.Get(productId, cartId);
            BaseAssertions(insertedProductCart, cartId, productId, amount);
            Assert.That(insertedProductCart!.Price, Is.EqualTo(price));
        }

        [Test, Rollback]
        [TestCase(0, 0, (uint)1, 10.0)]
        [TestCase(-1, -1, (uint)7, 11.3)]
        [TestCase(0, -1, (uint)3, 10.3)]
        [TestCase(-1, 0, (uint)3, 10.3)]
        public void CreateProductCartWithInvalidPropertiesThrowsException(int cartId, int productId, uint amount, double price) =>
            Assert.CatchAsync(async () => await sut.Create(new ProductCart(productId, cartId, price, amount)));


        private void BaseAssertions(ProductCart? cart, int cartId, int productId, uint amount)
        {
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.ProductId, Is.EqualTo(productId));
            Assert.That(cart.CartId, Is.EqualTo(cartId));
            Assert.That(cart.Amount, Is.EqualTo(amount));
        }

    }
}
