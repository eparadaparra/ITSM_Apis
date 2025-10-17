using ITSM_Apis.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ITSM_Apis.Controllers
{
    [EnableCors("RulesCors")]
    [ApiController]

    [Route("api/[controller]")]
    public class OldITSMController : ControllerBase
    {
        private readonly IServices _services;

        public OldITSMController(IServices servicesAPI)
        {
            _services = servicesAPI;
        }

        #region Create Release from SR
        [HttpPost]
        [Route("CreateReleaseFromSR")]
        public async Task<IActionResult> CreateReleaseFromSR([FromBody] Object body)
        {
            var result = await _services.SRtoRelease(body);
            
            return StatusCode(StatusCodes.Status200OK, result);
        }
        #endregion

        #region Busca|Crea|Relaciona CI Release from SR
        [HttpPost]
        [Route("CIReleaseFromSR")]
        public async Task<IActionResult> CIReleaseFromSR([FromBody] Object body)
        {
            var result = await _services.CIReleaseFromSR(body);

            return StatusCode(StatusCodes.Status200OK, result);
        }
        #endregion

    }
}
