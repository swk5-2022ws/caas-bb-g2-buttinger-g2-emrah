using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Util;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponLogic couponLogic;
        private readonly IMapper mapper;
        private readonly ILogger<CouponController> logger;

        public CouponController(ICouponLogic couponLogic, IMapper mapper, ILogger<CouponController> logger)
        {
            this.couponLogic = couponLogic
                ?? throw ExceptionUtil.ParameterNullException(nameof(couponLogic));
            this.mapper = mapper
                ?? throw ExceptionUtil.ParameterNullException(nameof(mapper));
            this.logger = logger
                ?? throw ExceptionUtil.ParameterNullException(nameof(logger));
        }

        [HttpPost("coupon")]
        public async Task<ActionResult> Create([FromBody] TCreateCoupon coupon, [FromHeader] Guid appKey)
        {
            try
            {
                var couponId = await couponLogic.Create(mapper.Map<Coupon>(coupon), appKey);

                logger.LogInformation($"A coupon with the id {couponId} was created.");
                return CreatedAtAction(
                    actionName: nameof(GetList),
                    routeValues: new { couponId },
                    value: coupon);
            }
            catch (ArgumentException e)
            {
                logger.LogError($"Creation of a coupon was not possible. Due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("shop/{shopId}/coupons")]
        public async Task<ActionResult<IList<TCoupon>>> GetList(int shopId, [FromHeader] Guid appKey)
        {
            try
            {
                return Ok(mapper.Map<IList<TCoupon>>(await couponLogic.GetCoupons(shopId, appKey)));
            }
            catch (ArgumentException e)
            {
                logger.LogError($"No coupon list for the shop with id {shopId} found due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("coupon/{couponKey}")]
        public async Task<ActionResult> Delete(string couponKey, [FromHeader] Guid appKey)
        {
            try
            {
                var updated = await couponLogic.Delete(couponKey, appKey);
                logger.LogInformation($"Product with Key {couponKey} was deleted!");
                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentException e)
            {
                logger.LogError($"No coupons with such key!");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("cart/{cartId}/coupon/{couponKey}")]
        public async Task<ActionResult> Apply(int cartId, string couponKey, [FromHeader] Guid appKey)
        {
            try
            {
                var updated = await couponLogic.Apply(couponKey, cartId, appKey);
                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentException e)
            {
                logger.LogError($"An error has occured while trying to apply a coupon to a cart. The error is the following: {e.Message}");
                return BadRequest(e.Message);
            }
            catch(TypeAccessException e)
            {
                logger.LogError($"Tried to match a coupon with a product from a different shop !! CartId: {cartId} CouponKey: {couponKey}. ErrorMessage: {e.Message}");
                return BadRequest(e.Message);
            }
        }
    }
}
