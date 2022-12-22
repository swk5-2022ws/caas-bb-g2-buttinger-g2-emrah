using AutoMapper;
using CaaS.Api.Util;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers
{
    [ApiConventionType(typeof(WebApiConventions))]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountActionController : ControllerBase
    {
        private readonly IDiscountActionLogic discountActionLogic;
        private readonly IMapper mapper;

        public DiscountActionController(IDiscountActionLogic discountActionLogic, IMapper mapper)
        {
            this.discountActionLogic = discountActionLogic;
            this.mapper = mapper;
        }

        [Route("api/discount/actions/types")]
        [HttpGet]
        public async Task<IActionResult> GetDiscountRules()
        {
            var rules = await discountActionLogic.GetRulesets();
            return Ok(rules);
        }

        [Route("api/discount/actions/{id}")]
        [HttpPost]
        public async Task<IActionResult> Create([FromHeader] Guid appKey, [FromBody] DiscountAction discountAction)
        {
            try
            {
                var id = await discountActionLogic.Create(appKey, discountAction);
                return CreatedAtAction(
                actionName: nameof(GetByShopId),
                routeValues: new { discountAction.ShopId },
                value: id);
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

        [Route("api/shop/{id}/actions/rules")]
        [HttpGet]
        public async Task<IActionResult> GetByShopId([FromHeader] Guid appKey, [FromRoute] int id)
        {
            try
            {
                var discounts = await discountActionLogic.GetByShopId(appKey, id);
                return Ok(discounts);
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

        [Route("api/discount/actions/{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromHeader] Guid appKey, [FromRoute] int id)
        {
            try
            {
                var discounts = await discountActionLogic.Delete(appKey, id);
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
    }
}
