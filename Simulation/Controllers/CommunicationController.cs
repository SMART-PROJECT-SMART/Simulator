using Microsoft.AspNetCore.Mvc;
using Simulation.Dto.Communication;
using Simulation.Services.PortManager;

namespace Simulation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunicationController : ControllerBase
    {
        private readonly IPortManager _portManager;

        [HttpPost("switch-ports")]
        public IActionResult SwitchPorts([FromBody] SwitchPortDto dto)
        {
            _portManager.switchPorts(dto.TailId, dto.TargetPort, dto.TargetPort);
            return Ok();
        }
    }
}
