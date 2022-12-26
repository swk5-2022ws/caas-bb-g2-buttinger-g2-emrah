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
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderLogic orderLogic;
        private readonly IDiscountLogic discountLogic;
        private readonly IMapper mapper;
        private readonly ILogger<OrderController> logger;

        public OrderController(IOrderLogic orderLogic, IDiscountLogic discountLogic, IMapper mapper, ILogger<OrderController> logger)
        {
            this.orderLogic = orderLogic
                ?? throw ExceptionUtil.ParameterNullException(nameof(orderLogic));
            this.discountLogic = discountLogic
                ?? throw ExceptionUtil.ParameterNullException(nameof(discountLogic));
            this.mapper = mapper
                ?? throw ExceptionUtil.ParameterNullException(nameof(mapper));
            this.logger = logger
                ?? throw ExceptionUtil.ParameterNullException(nameof(logger));
        }

        [HttpGet("customer/{customerId}/orders")]
        public async Task<ActionResult<IList<TOrder>>> GetByCustomer(int customerId, [FromHeader] Guid appKey)
        {
            try
            {
                return Ok(mapper.Map<IList<TOrder>>(await orderLogic.GetByCustomerId(customerId, appKey)));
            }
            catch (ArgumentNullException e)
            {
                logger.LogError($"No order with for given customer id {customerId} found.");
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                logger.LogError($"No orders for the customer with id {customerId} found due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet("shop/{shopId}/orders")]
        public async Task<ActionResult<IList<TOrder>>> GetByShop(int shopId, [FromHeader] Guid appKey)
        {
            try
            {
                return Ok(mapper.Map<IList<TOrder>>(await orderLogic.GetByShopId(shopId, appKey)));
            }
            catch (ArgumentNullException e)
            {
                logger.LogError($"No order with for given shop id {shopId} found.");
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                logger.LogError($"No orders for the shop with id {shopId} found due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("order/{id}")]
        public async Task<ActionResult<TOrder>> Get(int id, [FromHeader] Guid appKey)
        {
            try
            {
                return Ok(mapper.Map<TOrder>(await orderLogic.Get(id, appKey)));
            }
            catch (ArgumentNullException e)
            {
                logger.LogError($"No order with id {id} found.");
                return NotFound(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                logger.LogError($"No order with id {id} found.");
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                logger.LogError($"A order for the id {id} was not found due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("cart/{id}/order")]
        public async Task<ActionResult> Create([FromBody] int id, [FromHeader] Guid appKey)
        {
            try
            {
                var orderId = await orderLogic.Create(id, await discountLogic.GetAvailableDiscountsByCartId(appKey, id), appKey);
                logger.LogInformation($"An order with the id {id} was created.");
                return CreatedAtAction(
                    actionName: nameof(Create),
                    routeValues: new { orderId },
                    value: null);
            }
            catch(ArgumentNullException e)
            {
                logger.LogError($"Could not find an element with the given cart with id {id} due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
            catch(KeyNotFoundException e)
            {
                logger.LogError($"Could not find an element with the given cart with id {id} due to the following error: {e.Message}");
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                logger.LogError($"Creation of an order was not possible due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("order/pay/{id}")]
        public async Task<ActionResult> Pay(int id, [FromHeader] Guid appKey)
        {
            try
            {
                var updated = await orderLogic.Pay(id, appKey);
                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentNullException e)
            {
                logger.LogError($"Could not find an element with the given order id {id} due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                logger.LogError($"Could not find an element with the given order id {id} due to the following error: {e.Message}");
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                logger.LogError($"An error has occured while trying to pay for an order. The error is the following: {e.Message}");
                return BadRequest(e.Message);
            }
        }
    }
}
