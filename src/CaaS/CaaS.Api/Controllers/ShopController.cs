using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Api.Util;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Tls;
using System.Linq.Expressions;
using System.Net;

namespace CaaS.Api.Controllers
{
    [ApiConventionType(typeof(WebApiConventions))]
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IShopLogic shopLogic;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public ShopController(IShopLogic shopLogic, IMapper mapper, ILogger<ShopController> logger)
        {
            this.shopLogic = shopLogic
                ?? throw new ArgumentNullException($"Parameter {nameof(shopLogic)} is null.");
            this.mapper = mapper
                ?? throw new ArgumentNullException($"Parameter {nameof(mapper)} is null");
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetShopById()
        {
            return Ok();
        }


        [HttpPost]
        public async Task<ActionResult> CreateShop([FromBody] TCreateShop shop)
        {
            try
            {   
                var shopId = await shopLogic.Create(mapper.Map<Shop>(shop));
                var newShop = shop with { Id = shopId };
                var createdShop = shop;

                return CreatedAtAction(
                    actionName: nameof(GetShopById),
                    routeValues: new { shopId },
                    value: createdShop);
            }
            catch (ArgumentException ex)
            {
                // do not deliver stack trace to end user
                return BadRequest(ex.Message);
            }
        }

    }
}
