using AutoMapper;
using CaaS.Api.Controllers;
using CaaS.Api.Transfers;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Test;
using CaaS.Core.Test.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Api.Test.Controllers
{
    public class ProductControllerTest
    {
        private ProductController sut;

        [OneTimeSetUp]
        public void InitializeSut()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<ProductController> logger = loggerFactory.CreateLogger<ProductController>();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(ProductController));
            });
            var mapper = mockMapper.CreateMapper();


            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());
            IProductRepository productRepository = new ProductRepository(Setup.GetTemplateEngine());
            IProductLogic productLogic = new ProductLogic(productRepository, shopRepository);

            sut = new ProductController(productLogic, mapper, logger);
        }

        [Test, Rollback]
        public async Task TestCreateProductWithValidProductReturnsCreatedAtActionResult()
        {
            TCreateProduct createProduct = new TCreateProduct(1, "Test", "", "Test", 10);
            CreatedAtActionResult actionResult = (CreatedAtActionResult)await sut.Create(createProduct, new Guid("a82724ba-ced5-32e8-9ada-17b06d427906"));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(201));
        }

        [Test, Rollback]
        public async Task TestCreateProductWithInvalidShopIdReturnsBadRequest()
        {
            TCreateProduct createProduct = new TCreateProduct(-1, "Test", "", "Test", 10);
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Create(createProduct, new Guid("a82724ba-ced5-32e8-9ada-17b06d427906"));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        public async Task TestCreateProductWithIncorrectAppKeyReturnsBadRequest()
        {
            TCreateProduct createProduct = new TCreateProduct(0, "Test", "", "Test", 10);
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Create(createProduct, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }


        [Test, Rollback]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(double.MinValue)]
        public async Task TestCreateProductWithToLowPriceReturnsBadRequest(double price)
        {
            TCreateProduct createProduct = new TCreateProduct(-1, "Test", "", "Test", price);
            BadRequestObjectResult actionResult = (BadRequestObjectResult)await sut.Create(createProduct, new Guid("a82724ba-ced5-32e8-9ada-17b06d427906"));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public async Task TestEditProductWithInvalidIdReturnsBadRequest(int id)
        {
            TEditProduct editProduct = new TEditProduct(id, "Test", "", "Test", 10);
            var actionResult = (BadRequestObjectResult)await sut.Edit(editProduct, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        public async Task TestEditProductWithInvalidAppKeyReturnsBadRequest(int id)
        {
            TEditProduct editProduct = new TEditProduct(2, "Test", "", "Test", 10);
            var actionResult = (BadRequestObjectResult)await sut.Edit(editProduct, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase(2, "37f3184c-0dcd-3a58-975f-803648fcc73b")]
        [TestCase(4, "f741eef1-5d30-3868-96b5-fdc400963bc0")]
        [TestCase(5, "8f26f620-9957-3251-8002-d593fad0003a")]
        public async Task TestEditProductWithValidIdReturnsNoContent(int id, string appKey)
        {
            TEditProduct editProduct = new TEditProduct(id, "Test", "", "Test", 10);
            var actionResult = (NoContentResult)await sut.Edit(editProduct, new Guid(appKey));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(204));
        }

        [Test, Rollback]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MinValue)]
        public async Task TestDeleteProductWithIdBelowZeroReturnsBadRequest(int id)
        {
            var actionResult = (BadRequestObjectResult)await sut.Delete(id, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(5)]
        public async Task TestDeleteProductWithInvalidAppKeyReturnsBadRequest(int id)
        {
            var actionResult = (BadRequestObjectResult)await sut.Delete(id, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase(int.MaxValue)]
        [TestCase(10001)]
        [TestCase(100000)]
        [TestCase(1)]
        public async Task TestDeleteProductWithIdOutOfUpperRangeOrAlreadyDeletedReturnsBadRequest(int id)
        {
            var actionResult = (BadRequestObjectResult)await sut.Delete(id, Guid.NewGuid());
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(400));
        }

        [Test, Rollback]
        [TestCase(2, "37f3184c-0dcd-3a58-975f-803648fcc73b")]
        [TestCase(4, "f741eef1-5d30-3868-96b5-fdc400963bc0")]
        [TestCase(5, "8f26f620-9957-3251-8002-d593fad0003a")]
        public async Task TestDeleteProductWithValidIdReturnsNoContent(int id, string appKey)
        {
            var actionResult = (NoContentResult)await sut.Delete(id, new Guid(appKey));
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(204));
        }

        [Test]
        [TestCase(2, "37f3184c-0dcd-3a58-975f-803648fcc73b", "Reverse-engineered bi-directional function")]
        [TestCase(4, "f741eef1-5d30-3868-96b5-fdc400963bc0", "Cross-platform fresh-thinking algorithm")]
        [TestCase(5, "8f26f620-9957-3251-8002-d593fad0003a", "Adaptive value-added frame")]
        public async Task TestGetProductWithValidIdReturnsOk(int id, string appKey, string label)
        {
            var actionResult = await sut.Get(id, new Guid(appKey));
            
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (OkObjectResult)actionResult.Result;
            TProduct? product = (TProduct?)result!.Value;

            Assert.That(product, Is.Not.Null);
            Assert.That(product.Id, Is.EqualTo(id));
            Assert.That(product.Label, Is.EqualTo(label));

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        [TestCase(1, "a82724ba-ced5-32e8-9ada-17b06d427906")]
        [TestCase(3, "21de65da-b9f6-309f-973c-7fcbc36192cc")]
        [TestCase(9, "6a9cff90-89ab-3daa-a80b-ae964bae2211")]
        public async Task TestGetProductWithInvalidIdReturnsNotFound(int id, string appKey)
        {
            var actionResult = await sut.Get(id, new Guid(appKey));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Value, Is.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            Assert.That(((NotFoundObjectResult)actionResult.Result).StatusCode, Is.EqualTo(404));
        }

        [Test]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(5)]
        public async Task TestGetProductWithInvalidAppKeyReturnsBadRequest(int id)
        {
            var actionResult = await sut.Get(id, Guid.NewGuid());

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            Assert.That(((BadRequestObjectResult)actionResult.Result!).StatusCode, Is.EqualTo(400));
        }

        [Test]
        [TestCase(1, "a82724ba-ced5-32e8-9ada-17b06d427906", 236)]
        [TestCase(2, "37f3184c-0dcd-3a58-975f-803648fcc73b", 246)]
        [TestCase(3, "21de65da-b9f6-309f-973c-7fcbc36192cc", 231)]
        public async Task TestGetProductsWithValidShopIdReturnsOk(int id, string appKey, int count)
        {
            var actionResult = await sut.GetProductsByShopId(id, new Guid(appKey));

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (OkObjectResult)actionResult.Result;
            var products = (List<TProduct>?)result!.Value;

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count, Is.EqualTo(count));

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        [TestCase(1, "a82724ba-ced5-32e8-9ada-17b06d427906", "well", 2)]
        [TestCase(2, "37f3184c-0dcd-3a58-975f-803648fcc73b", "engineered", 13)]
        [TestCase(3, "21de65da-b9f6-309f-973c-7fcbc36192cc", "buffered", 4)]
        public async Task TestGetProductsWithValidFilterReturnsOk(int id, string appKey, string filter, int count)
        {
            var actionResult = await sut.GetProductsByShopId(id, new Guid(appKey), filter);

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            var result = (OkObjectResult)actionResult.Result;
            var products = (List<TProduct>?)result!.Value;

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count, Is.EqualTo(count));

            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(5)]
        public async Task TestGetProductsWithInvalidAppKeyReturnsBadRequest(int id)
        {
            var actionResult = await sut.GetProductsByShopId(id, Guid.NewGuid());

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            Assert.That(((BadRequestObjectResult)actionResult.Result!).StatusCode, Is.EqualTo(400));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(200000)]
        [TestCase(int.MinValue)]
        public async Task TestGetProductsWithInvalidIdReturnsBadRequest(int id)
        {
            var actionResult = await sut.GetProductsByShopId(id, Guid.NewGuid());

            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.Result, Is.Not.Null);

            Assert.That(((BadRequestObjectResult)actionResult.Result!).StatusCode, Is.EqualTo(400));
        }
    }
}
