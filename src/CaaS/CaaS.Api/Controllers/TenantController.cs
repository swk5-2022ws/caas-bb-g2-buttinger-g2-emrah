using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantRepository tenantRepository;

        public TenantController(ITenantRepository tenantRepository)
        {
            this.tenantRepository = tenantRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await tenantRepository.GetAll());
        }

        [HttpPost]
        public async Task<ActionResult> Create(Tenant tenant)
        {
            int id = await tenantRepository.Create(tenant);
            tenant.Id = id;

            return CreatedAtAction(nameof(GetAll),
                value: tenant);
        }

        public async Task<ActionResult> Update(Tenant tenant)
        {
            return await tenantRepository.Update(tenant) ? NoContent() : BadRequest();
        }
    }
}
