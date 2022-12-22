using AutoMapper;
using CaaS.Api.Controllers;
using CaaS.Core.Domainmodels.DiscountRules;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using CaaS.Core.Repository;
using CaaS.Core.Test;
using CaaS.Core.Test.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaaS.Api.Transfers;

namespace CaaS.Api.Test.Controllers
{
    [Category("Integration")]
    [TestFixture]
    public class CustomerControllerTest
    {
        private CustomerController sut;
        private Guid appKey = Guid.Parse("a82724ba-ced5-32e8-9ada-17b06d427906");

        [SetUp]
        public void Init()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(CustomerController));
            });
            var mapper = mockMapper.CreateMapper();

            ICustomerRepository customerRepository = new CustomerRepository(Setup.GetTemplateEngine());
            IShopRepository shopRepository = new ShopRepository(Setup.GetTemplateEngine());

            ICustomerLogic customerLogic = new CustomerLogic(customerRepository, shopRepository);

            sut = new CustomerController(customerLogic, mapper);
        }

        // get
        [Test, Rollback]
        public async Task TestGetWithValidCustomerIdReturnsNoContent()
        {
            OkObjectResult result = (OkObjectResult)await sut.Get(appKey, 1);

            TCustomer customer = (TCustomer)result.Value!;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(customer.id, Is.EqualTo(1));
            });

        }

        [Test, Rollback]
        public async Task TestGetWithInvalidCustomerIdReturnsBadRequestObjectResult()
        {
            BadRequestObjectResult result = (BadRequestObjectResult)await sut.Get(appKey, int.MaxValue);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestGetWithValidAppKeyReturnsBadRequestObjectResult()
        {
            BadRequestObjectResult result = (BadRequestObjectResult)await sut.Get(Guid.NewGuid(), 1);
            Assert.That(result, Is.Not.Null);
        }

        // getbyshopid
        [Test, Rollback]
        public async Task TestGetByShopIdWithValidCustomerIdReturnsNoContent()
        {
            OkObjectResult result = (OkObjectResult)await sut.GetByShopId(appKey, 1);

            IList<TCustomer> values = ((IEnumerable<TCustomer>)result.Value!).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(values, Has.Count.EqualTo(11));
            });

        }

        [Test, Rollback]
        public async Task TestGetByShopIdWithInvalidCustomerIdReturnsBadRequestObjectResult()
        {
            BadRequestObjectResult result = (BadRequestObjectResult)await sut.GetByShopId(appKey, int.MaxValue);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestGetByShopIdWithValidAppKeyReturnsUnauthorizedResult()
        {
            UnauthorizedResult result = (UnauthorizedResult)await sut.GetByShopId(Guid.NewGuid(), 1);
            Assert.That(result, Is.Not.Null);
        }

        // create
        [Test, Rollback]
        public async Task TestCreateWithValidCustomerReturnsOkObjectResult()
        {
            TCreateCustomer customer = new(1, "nu", "neu");

            CreatedAtActionResult result = (CreatedAtActionResult)await sut.Create(appKey, customer);
            Assert.That(result, Is.Not.Null);
        }


        [Test, Rollback]
        public async Task TestCreateWithInvalidAppKeyReturnsUnauthorizedResult()
        {
            TCreateCustomer customer = new(1, "nu", "neu");

            UnauthorizedResult result = (UnauthorizedResult)await sut.Create(Guid.NewGuid(), customer);

            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestCreateWithInvalidShopIdReturnsUnauthorizedResult()
        {
            TCreateCustomer customer = new(int.MaxValue, "nu", "neu");

            BadRequestObjectResult result = (BadRequestObjectResult)await sut.Create(appKey, customer);

            Assert.That(result, Is.Not.Null);
        }

        // update
        [Test, Rollback]
        public async Task TestUpdateWithValidCustomerReturnsOkObjectResult()
        {
            TCustomer customer = new(1, 1, "nu", "neu");

            NoContentResult result = (NoContentResult)await sut.Update(appKey, customer);
            Assert.That(result, Is.Not.Null);

        }

        [Test, Rollback]
        public async Task TestUpdateWithInvalidAppKeyReturnsBadRequestObjectResult()
        {
            TCustomer customer = new(1, 1, "nu", "neu");

            BadRequestObjectResult result = (BadRequestObjectResult)await sut.Update(Guid.NewGuid(), customer);

            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestUpdateWithInvalidShopIdReturnsBadRequestObjectResult()
        {
            TCustomer customer = new(1, int.MaxValue, "nu", "neu");

            BadRequestObjectResult result = (BadRequestObjectResult)await sut.Update(appKey, customer);

            Assert.That(result, Is.Not.Null);
        }


        // delete
        [Test, Rollback]
        public async Task TestDeleteWithValidCustomerIdReturnsNoContent()
        {
            NoContentResult result = (NoContentResult)await sut.Delete(appKey, 1);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestDeleteWithInvalidCustomerIdReturnsBadRequestObjectResult()
        {
            BadRequestObjectResult result = (BadRequestObjectResult)await sut.Delete(appKey, int.MaxValue);
            Assert.That(result, Is.Not.Null);
        }

        [Test, Rollback]
        public async Task TestDeleteWithValidAppKeyReturnsBadRequestObjectResult()
        {
            BadRequestObjectResult result = (BadRequestObjectResult)await sut.Delete(Guid.NewGuid(), 1);
            Assert.That(result, Is.Not.Null);
        }


    }
}
