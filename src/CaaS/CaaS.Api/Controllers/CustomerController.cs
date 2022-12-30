using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Api.Util;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CaaS.Api.Controllers
{
    [ApiConventionType(typeof(WebApiConventions))]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerLogic customerLogic;
        private readonly IMapper mapper;

        public CustomerController(ICustomerLogic customerLogic, IMapper mapper)
        {
            this.customerLogic = customerLogic;
            this.mapper = mapper;
        }

        [HttpGet("api/customer/{id}")]
        public async Task<ActionResult> Get([FromHeader] Guid appKey, int id)
        {
            try
            {
                var customer = await customerLogic.Get(appKey, id);
                return Ok(mapper.Map<TCustomer>(customer));
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
                when (ex is ArgumentNullException || ex is ArgumentException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("api/shop/{id}/customers")]
        public async Task<ActionResult> GetByShopId([FromHeader] Guid appKey, int id)
        {
            try
            {
                var customers = await customerLogic.GetByShopId(appKey, id);
                return Ok(mapper.Map<IEnumerable<TCustomer>>(customers));
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
                when (ex is ArgumentNullException || ex is ArgumentException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("api/customer/{id}")]
        public async Task<ActionResult> Delete([FromHeader] Guid appKey, int id)
        {
            try
            {
                bool isDeleted = await customerLogic.Delete(appKey, id);
                
                return isDeleted ?  NoContent() : StatusCode(StatusCodes.Status500InternalServerError);
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
                when (ex is ArgumentNullException || ex is ArgumentException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("api/customer")]
        public async Task<ActionResult> Create([FromHeader] Guid appKey, [FromBody] TCreateCustomer customer)
        {
            try
            {
                var id = await customerLogic.Create(appKey, mapper.Map<Customer>(customer));

                return CreatedAtAction(
                    actionName: nameof(Get),
                    routeValues: new { id },
                    value: customer);
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
                when (ex is ArgumentNullException || ex is ArgumentException)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("api/customer")]
        public async Task<ActionResult> Update([FromHeader] Guid appKey, [FromBody] TCustomer customer)
        {
            try
            {
                var isUpdated = await customerLogic.Update(appKey, mapper.Map<Customer>(customer));
                return isUpdated ? NoContent() : StatusCode(StatusCodes.Status500InternalServerError);
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
                when (ex is ArgumentNullException || ex is ArgumentException)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
