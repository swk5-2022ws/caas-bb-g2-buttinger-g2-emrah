using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Logic;
using CaaS.Core.Logic;
using CaaS.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace CaaS.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductLogic productLogic;
        private readonly IMapper mapper;
        private readonly ILogger<ProductController> logger;

        public ProductController(IProductLogic productLogic, IMapper mapper, ILogger<ProductController> logger)
        {
            this.productLogic = productLogic
                ?? throw ExceptionUtil.ParameterNullException(nameof(productLogic));
            this.mapper = mapper
                ?? throw ExceptionUtil.ParameterNullException(nameof(mapper));
            this.logger = logger
                ?? throw ExceptionUtil.ParameterNullException(nameof(logger));
        }

        [HttpGet("product/{id}")]
        public async Task<ActionResult<TProduct>> Get(int id, [FromHeader] Guid appKey)
        {
            try
            {
                return Ok(mapper.Map<TProduct>(await productLogic.GetProductById(id, appKey)));
            }
            catch (ArgumentNullException e)
            {
                logger.LogError($"No product with id {id} found.");
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                logger.LogError($"A product for the id {id} was not found due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("shop/{shopId}/products")]
        public async Task<ActionResult<IList<TProduct>>> GetProductsByShopId(int shopId, [FromHeader] Guid appKey, [FromQuery] string? filter = null)
        {
            try
            {
                return Ok(mapper.Map<IList<TProduct>>(await productLogic.GetProductsByShopId(shopId, appKey, filter)));
            }
            catch(ArgumentException e)
            {
                logger.LogError($"No product list for the shop with id {shopId} found due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("product")]
        public async Task<ActionResult> Create([FromBody] TCreateProduct product, [FromHeader] Guid appKey)
        {
            try
            {
                var productId = await productLogic.Create(mapper.Map<Product>(product), appKey);

                logger.LogInformation($"A product with the id {productId} was created.");
                return CreatedAtAction(
                    actionName: nameof(Edit),
                    routeValues: new { productId },
                    value: product);
            }
            catch (ArgumentException e)
            {
                logger.LogError($"Creation of a product was not possible. Due to the following error: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("product")]
        public async Task<ActionResult> Edit([FromBody] TEditProduct product, [FromHeader] Guid appKey)
        {
            try
            {
                var updated = await productLogic.Edit(mapper.Map<Product>(product), appKey);
                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentException e)
            {
                logger.LogError($"An error has occured while trying to edit a product. The error is the following: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("product/{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] Guid appKey)
        {
            try
            {
                var updated = await productLogic.Delete(id, appKey);
                logger.LogInformation($"Product with id {id} was deleted!");
                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentException e)
            {
                logger.LogError($"No products with an ID smaller than 1!");
                return BadRequest(e.Message);
            }
        }
    }
}
