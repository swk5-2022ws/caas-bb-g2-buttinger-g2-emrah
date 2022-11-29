using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Api.Util;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
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
        private readonly ITenantRepository tenantRepository;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        // TODO tenantLogic not repository
        public ShopController(IShopLogic shopLogic, ITenantRepository tenantRepository, IMapper mapper, ILogger<ShopController> logger)
        {
            this.shopLogic = shopLogic
                ?? throw new ArgumentNullException($"Parameter {nameof(shopLogic)} is null.");
            this.tenantRepository = tenantRepository
                ?? throw new ArgumentNullException($"Parameter {nameof(tenantRepository)} is null"); 
            this.mapper = mapper
                ?? throw new ArgumentNullException($"Parameter {nameof(mapper)} is null");
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetShopById([FromRoute] int id)
        {
            var shop = await shopLogic.Get(id);
            if (shop == null)
                return NotFound();

            return Ok(shop);
        }

        [HttpPost]
        public async Task<ActionResult> CreateShop([FromBody] TCreateShop shop)
        {
            try
            {   
                var shopId = await shopLogic.Create(mapper.Map<Shop>(shop));
                var newShop = await shopLogic.Get(shopId);
                if(newShop == null)
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);

                return CreatedAtAction(
                    actionName: nameof(GetShopById),
                    routeValues: new { shopId },
                    value: mapper.Map<Shop, TShop>(newShop));
            }
            catch (ArgumentException ex)
            {
                // do not deliver stack trace to end user
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult> UpdateShop([FromBody] TShop shop)
        {
            try
            {
                var isShopExist = shopLogic.Get(shop.Id) != null;
                if (!isShopExist)
                    return NotFound();

                var isTenantExist = tenantRepository.Get(shop.TenantId) != null;
                if (!isTenantExist)
                    return NotFound();

                var isUpdateSuccess = await shopLogic.Update(mapper.Map<Shop>(shop));
                return isUpdateSuccess ?  NoContent() : new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            catch (ArgumentException ex)
            {
                // do not deliver stack trace to end user
                return BadRequest(ex.Message);
            }
        }

    }
}
