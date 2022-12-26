using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Api.Util;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers
{
    [ApiConventionType(typeof(WebApiConventions))]
    [ApiController]
    public class DiscountRuleController : ControllerBase
    {
        private readonly IDiscountRuleLogic discountRuleLogic;
        private readonly IMapper mapper;

        public DiscountRuleController(IDiscountRuleLogic discountRuleLogic, IMapper mapper)
        {
            this.discountRuleLogic = discountRuleLogic;
            this.mapper = mapper;
        }

        [Route("api/discount/rules/types")]
        [HttpGet]
        public async Task<IActionResult> GetDiscountRules()
        {
            var rules = await discountRuleLogic.GetRulesets();
            return Ok(rules);
        }

        [Route("api/discount/rule")]
        [HttpPost]
        public async Task<IActionResult> Create([FromHeader] Guid appKey, [FromBody] TCreateDiscountRule discountRule)
        {
            try
            {
                var id = await discountRuleLogic.Create(appKey, mapper.Map<DiscountRule>(discountRule));
                return CreatedAtAction(
                actionName: nameof(GetByShopId),
                routeValues: new { id = discountRule.ShopId },
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

        [Route("api/shop/{id}/rules")]
        [HttpGet]
        public async Task<IActionResult> GetByShopId([FromHeader] Guid appKey, [FromRoute] int id)
        {
            try
            {
                var discounts = await discountRuleLogic.GetByShopId(appKey, id);
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

        [Route("api/discount/rule/{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromHeader] Guid appKey, [FromRoute] int id)
        {
            try
            {
                var discounts = await discountRuleLogic.Delete(appKey, id);
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
