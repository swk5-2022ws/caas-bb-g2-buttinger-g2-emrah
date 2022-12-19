using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Api.Util;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Transferrecordes;
using CaaS.Util;
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
               ?? throw ExceptionUtil.ParameterNullException(nameof(shopLogic));
            this.tenantRepository = tenantRepository
              ?? throw ExceptionUtil.ParameterNullException(nameof(tenantRepository));
            this.mapper = mapper
               ?? throw ExceptionUtil.ParameterNullException(nameof(mapper));
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetShopById([FromHeader] Guid appKey, [FromRoute] int id)
        {
            try
            {
                var shop = await shopLogic.Get(appKey, id);
                if (shop == null)
                    return NotFound();

                return Ok(shop);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // do not deliver stack trace to end user
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("api/shops/{id}")]
        public async Task<ActionResult> GetShopsByTenantId([FromRoute] int id)
        {
            if(await tenantRepository.Get(id) == null)
                return NotFound();

            IEnumerable<Shop>? shops = await shopLogic.GetByTenantId(id);

            return Ok(mapper.Map<IEnumerable<Transfers.TShop>>(shops));
        }

        [HttpPost]
        public async Task<ActionResult> CreateShop([FromBody] TCreateShop tShop)
        {
            try
            {

                var shop = mapper.Map<Shop>(tShop);
                var shopId = await shopLogic.Create(shop);
                var newShop = await shopLogic.Get(shop.AppKey, shopId);
                if(newShop == null)
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);

                return CreatedAtAction(
                    actionName: nameof(GetShopById),
                    routeValues: new { shopId },
                    value: mapper.Map<Shop, Transfers.TShop>(newShop));
            }
            catch (ArgumentException ex)
            {
                // do not deliver stack trace to end user
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult> UpdateShop([FromHeader] Guid appKey, [FromBody] Transfers.TShop tShop)
        {
            try
            {
                var isTenantIdWrong = await tenantRepository.Get(tShop.TenantId) == null;
                if (isTenantIdWrong)
                    return NotFound();

                var shop = mapper.Map<Shop>(tShop);
                var isUpdateSuccess = await shopLogic.Update(appKey, shop);
                return isUpdateSuccess ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // do not deliver stack trace to end user
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteShop([FromHeader] Guid appKey, [FromRoute] int id)
        {
            try
            {
                return await shopLogic.Delete(appKey, id) ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // do not deliver stack trace to end user
                return BadRequest(ex.Message);
            }
        }

    }
}
