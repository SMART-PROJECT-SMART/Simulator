using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simulation.Services.UAVManager;

namespace Simulation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UAVStatusController : ControllerBase
    {
        private readonly IUAVManager _uavManager;
        public UAVStatusController(IUAVManager uavManager)
        {
            _uavManager = uavManager;
        }
        [HttpGet("active-uav")]
        public IActionResult GetActiveUAVs()
        {
            IEnumerable<int> activeTailIds = _uavManager.GetActiveTailIds;
            return Ok(activeTailIds);
        }
    }
}
