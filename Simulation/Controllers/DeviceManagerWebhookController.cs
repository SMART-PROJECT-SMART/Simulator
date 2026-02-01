using Microsoft.AspNetCore.Mvc;
using Simulation.Dto.DeviceManager;
using Simulation.Services.UAVChangeHandlers;

namespace Simulation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceManagerWebhookController : ControllerBase
    {
        private readonly UAVChangeHandlerFactory _handlerFactory;

        public DeviceManagerWebhookController(UAVChangeHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        [HttpPost("uav-changed")]
        public async Task<ActionResult> UAVChanged([FromBody] UAVChangedNotificationDto notification, CancellationToken cancellationToken)
        {
            IUAVChangeHandler handler = _handlerFactory.CreateHandler(notification.Operation);
            await handler.HandleUAVChangeAsync(notification.TailId, cancellationToken);
            return Ok();
        }
    }
}
