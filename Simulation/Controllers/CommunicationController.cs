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

        public CommunicationController(IPortManager portManager)
        {
            _portManager = portManager;
        }

        [HttpPost("switch-ports")]
        public IActionResult SwitchPorts([FromBody] SwitchPortDto dto)
        {
            _portManager.SwitchPorts(dto.SourcePort, dto.TargetPort);
            return Ok();
        }
    }
}
