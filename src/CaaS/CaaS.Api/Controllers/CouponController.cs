using AutoMapper;
using CaaS.Core.Interfaces.Logic;
using CaaS.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponLogic couponLogic;
        private readonly IMapper mapper;
        private readonly ILogger<ProductController> logger;

        public CouponController(ICouponLogic couponLogic, IMapper mapper, ILogger<ProductController> logger)
        {
            this.couponLogic = couponLogic
                ?? throw ExceptionUtil.ParameterNullException(nameof(couponLogic));
            this.mapper = mapper
                ?? throw ExceptionUtil.ParameterNullException(nameof(mapper));
            this.logger = logger
                ?? throw ExceptionUtil.ParameterNullException(nameof(logger));
        }
    }
}
