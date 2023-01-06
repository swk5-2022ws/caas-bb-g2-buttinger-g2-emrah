using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Logic;
using CaaS.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CaaS.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartLogic cartLogic;
        private readonly IMapper mapper;
        private readonly ILogger<CartController> logger;

        public CartController(ICartLogic cartLogic, IMapper mapper, ILogger<CartController> logger)
        {
            this.cartLogic = cartLogic
                ?? throw ExceptionUtil.ParameterNullException(nameof(cartLogic));
            this.mapper = mapper
                ?? throw ExceptionUtil.ParameterNullException(nameof(mapper));
            this.logger = logger
                ?? throw ExceptionUtil.ParameterNullException(nameof(logger));
        }

        [HttpPost("cart")]
        public async Task<ActionResult> Create([FromHeader] Guid appKey)
        {
            try
            {
                var sessionId = await cartLogic.Create();

                logger.LogInformation($"A Cart with the sessionId {sessionId} was created.");
                return CreatedAtAction(
                    actionName: nameof(Get),
                    routeValues: new { sessionId },
                    value: new { SessionId = sessionId });
            }
            catch (ArgumentException e)
            {
                logger.LogError($"Creation of a coupon was not possible. Due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }
        
        [HttpPost("customer/{customerId}/cart")]
        public async Task<ActionResult> Create(int customerId, [FromHeader] Guid appKey)
        {
            try
            {
                var sessionId = await cartLogic.CreateCartForCustomer(customerId, appKey);

                logger.LogInformation($"A Cart with the sessionId {sessionId} was created and already referenced to the customer with id {customerId}");
                return CreatedAtAction(
                    actionName: nameof(Get),
                    routeValues: new { sessionId },
                    value: null);
            }
            catch (ArgumentException e)
            {
                logger.LogError($"Creation of a coupon was not possible. Due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("cart/{sessionId}/product/{productId}")]
        public async Task<ActionResult> ReferenceProduct(string sessionId, int productId, [FromQuery] uint? amount, [FromHeader] Guid appKey)
        {
            try
            {
                return (await cartLogic.ReferenceProductToCart(sessionId, productId, appKey, amount)) ? NoContent() : NotFound();
            }
            catch (ArgumentException e)
            {
                logger.LogError($"Creation of a coupon was not possible. Due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }
        
        [HttpPut("customer/{customerId}/cart/{sessionId}")]
        public async Task<ActionResult> ReferenceCustomer(int customerId, string sessionId, [FromHeader] Guid appKey)
        {
            try
            {
                return (await cartLogic.ReferenceCustomerToCart(customerId, sessionId, appKey)) ? NoContent() : NotFound();
            }
            catch (ArgumentException e)
            {
                logger.LogError($"Creation of a coupon was not possible. Due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("cart/{sessionId}")]
        public async Task<ActionResult<TCart>> Get(string sessionId, [FromHeader] Guid appKey)
        {
            try
            {
                return Ok(mapper.Map<TCart>(await cartLogic.Get(sessionId, appKey)));
            }
            catch (ArgumentException e)
            {
                logger.LogError($"Retrieval of a cart was not possible due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("cart/{sessionId}/product/{productId}")]
        public async Task<ActionResult> Delete(string sessionId, int productId, [FromQuery] uint? amount, [FromHeader] Guid appKey)
        {
            try
            {
                return await cartLogic.DeleteProductFromCart(sessionId, productId, appKey, amount) ? NoContent() : NotFound();
            }
            catch (ArgumentException e)
            {
                logger.LogError($"No coupons with such key!");
                return BadRequest(e.Message);
            }
        }
    }
}
