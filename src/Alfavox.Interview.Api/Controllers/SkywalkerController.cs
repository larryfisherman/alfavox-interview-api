using Alfavox.Interview.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace alfavox_interview_api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SkywalkerController : ControllerBase
    {
        private readonly ISkywalkerService _skywalkerService;

        public SkywalkerController(ISkywalkerService skywalkerService)
        {
            _skywalkerService = skywalkerService; 
        }

        [HttpGet("skywalker")]
        public async Task<IActionResult> GetSkywalkerData()
        {
           return Ok(await _skywalkerService.GetSkywalkerData());
        }

    }
}
