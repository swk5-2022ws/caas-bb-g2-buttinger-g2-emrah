﻿using AutoMapper;
using CaaS.Api.Transfers;
using CaaS.Api.Util;
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
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
