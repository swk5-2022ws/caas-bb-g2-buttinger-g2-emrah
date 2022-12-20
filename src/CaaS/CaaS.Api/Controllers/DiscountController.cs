using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Api.Util;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Interfaces.Repository;
using CaaS.Core.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CaaS.Api.Controllers
{
    [ApiConventionType(typeof(WebApiConventions))]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountLogic discountLogic;
        private readonly ITenantRepository tenantRepository;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        // TODO tenantLogic not repository
        public DiscountController(IDiscountLogic discountLogic, ITenantRepository tenantRepository, IMapper mapper, ILogger<DiscountController> logger)
        {
            this.discountLogic = discountLogic
                ?? throw new ArgumentNullException($"Parameter {nameof(discountLogic)} is null.");
            this.tenantRepository = tenantRepository
                ?? throw new ArgumentNullException($"Parameter {nameof(tenantRepository)} is null");
            this.mapper = mapper
                ?? throw new ArgumentNullException($"Parameter {nameof(mapper)} is null");
            this.logger = logger;
        }

        [HttpGet("api/cart/{id}/discounts")]
        public async Task<ActionResult> GetDiscountsForCart([FromHeader] Guid appKey, int id)
        {
            try
            {
                var discounts = await discountLogic.GetAvailableDiscountsByCartId(appKey, id);
                var tDiscounts = mapper.Map<IEnumerable<TDiscount>>(discounts);

                return Ok(tDiscounts);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
                when (ex is UnauthorizedAccessException || ex is ArgumentException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("api/cart/{id}/discounts")]
        public async Task<ActionResult> AddDiscountsToCart([FromHeader] Guid appKey, [FromRoute] int id,
            [FromBody] IList<int> discountIds)
        {
            try
            {
                await discountLogic.AddDiscountsToCart(appKey, id, discountIds);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
                when (ex is ArgumentException || ex is ArgumentNullException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("api/discount/{id}")]
        public async Task<ActionResult> DeleteDiscount([FromHeader] Guid appKey, [FromRoute] int id)
        {
            try
            {
                await discountLogic.Delete(appKey, id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("api/discount/{id}")]
        public async Task<ActionResult> Get([FromHeader] Guid appKey, [FromRoute] int id)
        {
            try
            {
                var discount = await discountLogic.Get(appKey, id);
                var tDiscounts = mapper.Map<TDiscount>(discount);
                return Ok(tDiscounts);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("api/discount")]
        public async Task<ActionResult> CreateDiscount([FromHeader] Guid appKey, [FromBody] TDiscount discount)
        {
            try
            {
                Domainmodels.Discount discountBo = mapper.Map<Discount>(discount);
                int id = await discountLogic.Create(appKey, discountBo);

                return CreatedAtAction(
                    actionName: nameof(Get),
                    routeValues: new { id },
                    value: discount);

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}