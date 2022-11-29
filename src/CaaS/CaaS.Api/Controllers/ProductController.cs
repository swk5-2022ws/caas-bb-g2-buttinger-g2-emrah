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

        public ProductController(IProductLogic productLogic, IMapper mapper)
        {
            this.productLogic = productLogic
                ?? throw ExceptionUtil.ParameterNullException(nameof(productLogic));
            this.mapper = mapper
                ?? throw ExceptionUtil.ParameterNullException(nameof(mapper));
        }

        [HttpGet("product/{id}")]
        public async Task<ActionResult<TProduct>> Get(int id)
        {
            try
            {
                return Ok(mapper.Map<TProduct>(await productLogic.GetProductById(id)));
            }
            catch (ArgumentNullException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("shop/{shopId}/products")]
        public async Task<ActionResult<IList<TProduct>>> GetProductsByShopId(int shopId, [FromHeader] Guid appKey)
        {
            try
            {
                return Ok(mapper.Map<IList<TProduct>>(await productLogic.GetProductsByShopId(shopId, appKey)));
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("product")]
        public async Task<ActionResult<TCreateProduct>> Create([FromBody] TCreateProduct product, [FromHeader] Guid appKey)
        {
            try
            {
                var productId = await productLogic.Create(mapper.Map<Product>(product), appKey);

                return CreatedAtAction(
                    actionName: nameof(Edit),
                    routeValues: new { productId },
                    value: product);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("product")]
        public async Task<ActionResult<TEditProduct>> Edit([FromBody] TEditProduct product)
        {
            try
            {
                var updated = await productLogic.Edit(mapper.Map<Product>(product));
                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("product/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var updated = await productLogic.Delete(id);
                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
